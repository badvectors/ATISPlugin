using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;

namespace ATISPlugin
{
    // A division's own METAR API, set in ATISPresets.xml, e.g. VATPAC's, which has new reports minutes before
    // VATSIM's feed. It's used alongside vatSys, not instead: whichever has the newer report wins, so a source that
    // is slow, down or missing an airport just leaves the ATIS on vatSys's METAR.
    public class MetarSourceConfig
    {
        // {ICAO} is replaced by the airports asked for, comma separated, e.g. https://example.org/metar/{ICAO}
        [XmlAttribute]
        public string Url { get; set; }

        // ICAO prefixes the source covers, comma separated (e.g. "Y"). Empty means every airport.
        [XmlAttribute]
        public string Airports { get; set; }

        // Airports per request, as the API allows.
        [XmlAttribute]
        public int MaxPerRequest { get; set; } = 4;

        // Seconds between requests, as often as the API's data changes.
        [XmlAttribute]
        public int Interval { get; set; } = 300;

        public bool Covers(string icao)
        {
            if (string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(icao)) return false;

            var prefixes = (Airports ?? string.Empty).Split(',').Select(x => x.Trim().ToUpper()).Where(x => x.Length > 0).ToList();

            return !prefixes.Any() || prefixes.Any(x => icao.Trim().ToUpper().StartsWith(x));
        }

        // The site's name, to say where a METAR came from.
        public string Name
        {
            get
            {
                try
                {
                    return new Uri(Url.Replace("{ICAO}", "X")).Host.Replace("www.", string.Empty);
                }
                catch
                {
                    return "the METAR source";
                }
            }
        }
    }

    // The latest report from a division's METAR source for one airport.
    public class SourceMetar
    {
        public string ICAO { get; set; }

        // As vatSys gives METARs: the station first, no METAR or SPECI before it, no remarks after it.
        public string Text { get; set; }

        public bool IsSpeci { get; set; }

        public DateTime ObservedUtc { get; set; }

        public string Source { get; set; }
    }

    public static class MetarSource
    {
        // Long enough for the API to answer, short enough not to hold anything up.
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        // Older than this, a report is out of date (a station that's stopped reporting), so it isn't used.
        public static readonly TimeSpan MaxAge = TimeSpan.FromMinutes(90);

        private static readonly HttpClient Client = CreateClient();

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();

            try
            {
                var version = typeof(MetarSource).Assembly.GetName().Version;

                client.DefaultRequestHeaders.UserAgent.ParseAdd($"ATISPlugin/{version.Major}.{version.Minor} (+https://github.com/badvectors/ATISPlugin)");
            }
            catch
            {
                // Sent without one.
            }

            return client;
        }

        // The last report from the source, by airport.
        private static readonly ConcurrentDictionary<string, SourceMetar> Latest = new ConcurrentDictionary<string, SourceMetar>(StringComparer.OrdinalIgnoreCase);

        // When each airport was last asked for, so it's asked at the source's interval and no more often.
        private static readonly ConcurrentDictionary<string, DateTime> LastAsked = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        // Set by the plugin: the source for an airport (from the presets), or null for none. Null off the live
        // network (Sweatbox, a private server), where METARs come from vatSys alone, as that server sets them.
        public static Func<string, MetarSourceConfig> SourceFor { get; set; } = icao => null;

        // Set by the plugin: called (off the GUI thread) with each airport whose report is newer than before.
        public static Action<SourceMetar> Received { get; set; }

        // The source's latest report for the airport, while it's recent enough to use; null otherwise.
        public static SourceMetar Find(string icao, DateTime utcNow)
        {
            if (string.IsNullOrWhiteSpace(icao) || !Latest.TryGetValue(icao.Trim(), out var metar)) return null;

            // One read on the live network isn't used after moving to another server.
            if (!IsUsed(icao)) return null;

            return utcNow - metar.ObservedUtc <= MaxAge ? metar : null;
        }

        // Even when forced (Load METAR pressed again and again), an airport isn't asked for more often than this.
        private static readonly TimeSpan MinInterval = TimeSpan.FromSeconds(30);

        // Asks the source for the airports due, in as few requests as it allows. With force (a new ATIS, Load METAR)
        // it asks sooner than the source's interval, and passes on the report it already has, as an ATIS just
        // created hasn't had it. Silent: a request that fails leaves those airports on vatSys's METAR.
        public static Task Refresh(IEnumerable<string> airports, bool force = false)
        {
            try
            {
                var now = DateTime.UtcNow;

                var wanted = (airports ?? Enumerable.Empty<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToUpper()).Distinct()
                    .Select(x => new { ICAO = x, Source = SourceFor(x) })
                    .Where(x => x.Source != null)
                    .ToList();

                var requests = new List<Task>();

                if (force)
                {
                    foreach (var known in wanted.Select(x => Find(x.ICAO, now)).Where(x => x != null))
                    {
                        requests.Add(Task.Run(() => Announce(known)));
                    }
                }

                var due = wanted
                    .Where(x => !LastAsked.TryGetValue(x.ICAO, out var asked)
                        || now - asked >= (force ? MinInterval : TimeSpan.FromSeconds(Math.Max(60, x.Source.Interval) - 5)))
                    .ToList();

                foreach (var source in due.GroupBy(x => x.Source))
                {
                    var size = Math.Max(1, source.Key.MaxPerRequest);

                    var icaos = source.Select(x => x.ICAO).ToList();

                    for (var i = 0; i < icaos.Count; i += size)
                    {
                        var batch = icaos.Skip(i).Take(size).ToList();

                        foreach (var icao in batch) LastAsked[icao] = now;

                        requests.Add(Task.Run(() => Fetch(source.Key, batch)));
                    }
                }

                return Task.WhenAll(requests);
            }
            catch
            {
                return Task.CompletedTask;
            }
        }

        private static async Task Fetch(MetarSourceConfig source, List<string> icaos)
        {
            try
            {
                var url = source.Url.Replace("{ICAO}", string.Join(",", icaos.Select(Uri.EscapeDataString)));

                string body;

                using (var cancel = new CancellationTokenSource(Timeout))
                using (var response = await Client.GetAsync(url, cancel.Token).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode) return;

                    body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                var now = DateTime.UtcNow;

                foreach (var metar in Parse(body, source.Name, now).Where(x => icaos.Contains(x.ICAO)))
                {
                    if (Latest.TryGetValue(metar.ICAO, out var previous) && previous.ObservedUtc >= metar.ObservedUtc) continue;

                    Latest[metar.ICAO] = metar;

                    if (now - metar.ObservedUtc <= MaxAge) Announce(metar);
                }
            }
            catch
            {
                // Those airports stay on vatSys's METAR until the next request.
            }
        }

        // Whether the airport has a source now (false off the live network).
        public static bool IsUsed(string icao)
        {
            try
            {
                return SourceFor(icao.Trim().ToUpper()) != null;
            }
            catch
            {
                return false;
            }
        }

        private static void Announce(SourceMetar metar)
        {
            try
            {
                // An answer arriving after leaving the live network isn't used.
                if (!IsUsed(metar.ICAO)) return;

                Received?.Invoke(metar);
            }
            catch
            {
                // The plugin reports its own errors.
            }
        }

        // The API's answer: [{ "icao": "YSSY", "kind": "SPECI", "report": "SPECI YSSY 200735Z ...", "isStale": false }].
        // Anything it can't read (another shape, a stale or unreadable report) is left out.
        public static List<SourceMetar> Parse(string body, string source, DateTime utcNow)
        {
            var output = new List<SourceMetar>();

            try
            {
                foreach (var item in JArray.Parse(body).OfType<JObject>())
                {
                    try
                    {
                        if (item.Value<bool?>("isStale") == true) continue;

                        var report = item.Value<string>("report");

                        var text = Clean(report);

                        if (text == null) continue;

                        var icao = text.Substring(0, 4);

                        var claimed = item.Value<string>("icao");

                        if (!string.IsNullOrWhiteSpace(claimed) && !string.Equals(claimed.Trim(), icao, StringComparison.OrdinalIgnoreCase)) continue;

                        var observed = ObservedUtc(text, utcNow);

                        if (observed == null) continue;

                        var kind = item.Value<string>("kind");

                        output.Add(new SourceMetar
                        {
                            ICAO = icao,
                            Text = text,
                            IsSpeci = string.Equals(kind, "SPECI", StringComparison.OrdinalIgnoreCase) || report.TrimStart().StartsWith("SPECI ", StringComparison.OrdinalIgnoreCase),
                            ObservedUtc = observed.Value,
                            Source = source
                        });
                    }
                    catch
                    {
                        // Skip this one.
                    }
                }
            }
            catch
            {
                // Not the expected answer.
            }

            return output;
        }

        // A report as vatSys gives it: "METAR YSSY 200800Z ... RMK RF00.0/000.0" as "YSSY 200800Z ...". Null when it
        // doesn't start with a station and a time.
        public static string Clean(string report)
        {
            if (string.IsNullOrWhiteSpace(report)) return null;

            var text = Regex.Replace(report, "\\s+", " ").Trim().ToUpper();

            text = Regex.Replace(text, "^(METAR|SPECI) (COR )?", string.Empty);

            var remarks = text.IndexOf(" RMK", StringComparison.Ordinal);

            if (remarks > 0) text = text.Substring(0, remarks);

            text = text.TrimEnd('=', ' ');

            return Regex.IsMatch(text, "^[A-Z0-9]{4} \\d{6}Z( |$)") ? text : null;
        }

        // The observation time from the report's DDHHMMZ group: the most recent such time not after now (with a
        // little allowance for clocks), so a report from the 30th read on the 1st is last month's.
        public static DateTime? ObservedUtc(string metar, DateTime utcNow)
        {
            if (string.IsNullOrWhiteSpace(metar)) return null;

            var match = Regex.Match(metar, "(?:^|\\s)(\\d{2})(\\d{2})(\\d{2})Z(?:\\s|$)");

            if (!match.Success) return null;

            var day = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var hour = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            var minute = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            if (day < 1 || day > 31 || hour > 23 || minute > 59) return null;

            var latest = utcNow.AddMinutes(30);

            for (var months = 0; months <= 2; months++)
            {
                var month = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-months);

                if (day > DateTime.DaysInMonth(month.Year, month.Month)) continue;

                var observed = new DateTime(month.Year, month.Month, day, hour, minute, 0, DateTimeKind.Utc);

                if (observed <= latest) return observed;
            }

            return null;
        }
    }
}
