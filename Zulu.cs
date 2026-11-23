using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using vatsys;

namespace ATISPlugin
{
    public class Zulu
    {
        public static ZuluInfo GetInfo(string icao)
        {
            var zuluInfos = Plugin.ZuluInfo.Where(x => x.ICAO == icao);

            if (!zuluInfos.Any()) return null;

            if (zuluInfos.Count() == 1)
            {
                return zuluInfos.First();
            }

            var now = DateTime.UtcNow.TimeOfDay;

            foreach (var zulu in zuluInfos)
            {
                if (!TryParseTime(zulu.StartUtc, out var start) || !TryParseTime(zulu.EndUtc, out var end)) continue;

                // The end time covers its whole minute, so 1259 runs until 1300 and 2359 until midnight.
                end += TimeSpan.FromMinutes(1);

                var inWindow = start <= end ? now >= start && now < end : now >= start || now < end;

                if (inWindow) return zulu;
            }

            // An entry without times applies whenever none of the timed ones do.
            return zuluInfos.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.StartUtc) && string.IsNullOrWhiteSpace(x.EndUtc));
        }

        // Where a Zulu ATIS tells pilots who to contact.
        public const string ContactPlaceholder = "{STATION} {FREQ}";

        // "STATION FREQ" for the first of the airport's contacts that's online, or null when none are, or the airport
        // lists none. A contact is online when a controller in range (or this one) is logged on with its callsign, or
        // is transmitting on its frequency: its own, or one extended to it on AFV, such as ML_APP covering AV_APP.
        public static string FindContact(ZuluInfo info)
        {
            if (info?.Contacts == null || !info.Contacts.Any() || !Network.IsConnected) return null;

            var online = OnlineATCs();

            foreach (var contact in info.Contacts)
            {
                if (string.IsNullOrWhiteSpace(contact?.Callsign) || string.IsNullOrWhiteSpace(contact.Station)) continue;

                var station = contact.Station.Trim().ToUpper();

                var contactFrequency = ContactFrequency(contact);

                var pattern = "^" + Regex.Escape(contact.Callsign.Trim()).Replace("\\*", ".*") + "$";

                foreach (var atc in online)
                {
                    if (!Regex.IsMatch(atc.Callsign, pattern, RegexOptions.IgnoreCase)) continue;

                    var frequencies = Transmitting(atc);

                    // The contact's own frequency if they're on it, otherwise their primary.
                    var frequency = contactFrequency != null && frequencies.Any(x => SameFrequency(x, contactFrequency.Value))
                        ? contactFrequency
                        : frequencies.Cast<int?>().FirstOrDefault();

                    if (frequency == null) continue;

                    return $"{station} {KhzToString(frequency.Value)}";
                }

                if (contactFrequency == null) continue;

                if (online.Any(atc => Transmitting(atc).Any(x => SameFrequency(x, contactFrequency.Value))))
                {
                    return $"{station} {KhzToString(contactFrequency.Value)}";
                }
            }

            return null;
        }

        // A frequency that can be picked by hand for the Zulu contact.
        public class PickableFrequency
        {
            // As listed in the editor.
            public string Label;

            // As said on the ATIS, for {STATION} and {FREQ}.
            public string Station;
            public string Frequency;
        }

        // The frequencies being transmitted on: this controller's first (as named in its VSCS), then every other
        // controller's in range, named by the profile's sector on that frequency, or by the controller's callsign.
        public static List<PickableFrequency> PickableFrequencies()
        {
            var list = new List<PickableFrequency>();

            var taken = new List<int>();

            foreach (var frequency in Plugin.Frequencies)
            {
                var label = string.IsNullOrWhiteSpace(frequency.FriendlyName) ? frequency.Name : frequency.FriendlyName;

                if (string.IsNullOrWhiteSpace(label) || list.Any(x => x.Label == label)) continue;

                var khz = ToKhz(frequency.Frequency);

                list.Add(new PickableFrequency { Label = label, Station = label.Trim().ToUpper(), Frequency = KhzToString(khz) });

                taken.Add(khz);
            }

            if (!Network.IsConnected) return list;

            foreach (var atc in OnlineATCs())
            {
                if (atc.Callsign == Network.Me?.Callsign) continue;

                foreach (var khz in Transmitting(atc))
                {
                    if (taken.Any(x => SameFrequency(x, khz))) continue;

                    var sector = SectorFor(atc.Callsign, khz);

                    var name = sector != null ? Regex.Replace(sector.FullName, @"\s*\(.*?\)", string.Empty).Trim() : atc.Callsign;

                    var label = $"{name} {KhzToString(khz)}";

                    if (list.Any(x => x.Label == label)) continue;

                    list.Add(new PickableFrequency { Label = label, Station = name.ToUpper(), Frequency = KhzToString(khz) });

                    taken.Add(khz);
                }
            }

            return list;
        }

        // The controllers vatSys has in range, and this one.
        private static List<NetworkATC> OnlineATCs()
        {
            var online = (Network.GetOnlineATCs ?? new List<NetworkATC>()).Where(x => x?.Callsign != null).ToList();

            var me = Network.Me;

            if (me?.Callsign != null && !online.Any(x => x.Callsign == me.Callsign)) online.Add(me);

            return online;
        }

        // What a controller is transmitting on, in kHz, primary first. vatSys gets all of them, extended ones
        // included, from the controller's network position updates.
        private static List<int> Transmitting(NetworkATC atc) => (atc.Frequencies ?? new int[0])
            .Where(x => x > 0)
            .Select(x => ToKhz((uint)x))
            .Where(x => !SameFrequency(x, ObserverKhz))
            .Distinct()
            .ToList();

        // The contact's frequency in kHz: as given in the config, otherwise the profile's sector with its callsign.
        private static int? ContactFrequency(ZuluContact contact)
        {
            if (!string.IsNullOrWhiteSpace(contact.Frequency) &&
                double.TryParse(contact.Frequency.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var mhz) && mhz > 100)
            {
                return (int)Math.Round(mhz * 1000);
            }

            if (contact.Callsign.Contains("*")) return null;

            var sector = Sectors().FirstOrDefault(x => x.Frequency > 0 &&
                string.Equals(x.Callsign, contact.Callsign.Trim(), StringComparison.OrdinalIgnoreCase));

            return sector == null ? (int?)null : ToKhz(sector.Frequency);
        }

        // The sector a controller's frequency belongs to: one with their callsign first, then any on it.
        private static SectorsVolumes.Sector SectorFor(string callsign, int khz)
        {
            var sectors = Sectors()
                .Where(x => x.Frequency > 0 && !string.IsNullOrWhiteSpace(x.FullName) && SameFrequency(ToKhz(x.Frequency), khz))
                .ToList();

            return sectors.FirstOrDefault(x => string.Equals(x.Callsign, callsign, StringComparison.OrdinalIgnoreCase))
                ?? sectors.FirstOrDefault();
        }

        private static IEnumerable<SectorsVolumes.Sector> Sectors() =>
            (SectorsVolumes.Sectors ?? new List<SectorsVolumes.Sector>()).Where(x => x != null).ToList();

        // Frequencies come FSD style (24500 for 124.5), in kHz (124500) or in Hz (124500000).
        private static int ToKhz(uint frequency) =>
            frequency >= 1000000U ? (int)(frequency / 1000U) : frequency >= 100000U ? (int)frequency : (int)frequency + 100000;

        // A 25 kHz channel can come as 118.92 or 118.925.
        private static bool SameFrequency(int a, int b) => Math.Abs(a - b) <= 5;

        private static string KhzToString(int khz) => (khz / 1000.0).ToString("0.0##", CultureInfo.InvariantCulture);

        // Used by observers and controllers not yet on a frequency.
        private const int ObserverKhz = 199998;

        // The Zulu text with the contact filled in, or the placeholders left for the controller when there's none.
        public static string Fill(string text, string contact) =>
            (text ?? string.Empty).Replace(ContactPlaceholder, contact ?? ContactPlaceholder);

        private static bool TryParseTime(string hhmm, out TimeSpan time)
        {
            return TimeSpan.TryParseExact(hhmm ?? string.Empty, "hhmm", CultureInfo.InvariantCulture, out time);
        }
    }
}