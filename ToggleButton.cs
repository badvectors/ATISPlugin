using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using vatsys;
using System;

namespace ATISPlugin
{
    internal class ToggleButton : CheckBox
    {
        private Color _DisabledForeColor = Colours.GetColour(Colours.Identities.NonInteractiveText);

        private Color _BackColor = Colours.GetColour(Colours.Identities.WindowBackground);

        private Color _ForeColor = Colours.GetColour(Colours.Identities.InteractiveText);

        private string _SubText = "";

        private Font _SubFont = SystemFonts.DefaultFont;

        private IContainer? components;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color DisabledForeColor
        {
            get
            {
                return _DisabledForeColor;
            }
            set
            {
                _DisabledForeColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get
            {
                return _BackColor;
            }
            set
            {
                _BackColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get
            {
                return _ForeColor;
            }
            set
            {
                _ForeColor = value;
            }
        }

        public string SubText
        {
            get
            {
                return _SubText;
            }
            set
            {
                _SubText = value;
                Invalidate();
            }
        }

        public Font SubFont
        {
            get
            {
                return _SubFont;
            }
            set
            {
                _SubFont = value;
                Invalidate();
            }
        }

        public ToggleButton()
        {
            InitializeComponent();
            if (!Colours.Loaded)
            {
                //Colours.LoadColours();
            }

            Font = MMI.eurofont_sml;
            TextAlign = ContentAlignment.MiddleCenter;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            using (SolidBrush solidBrush = new SolidBrush(BackColor))
            {
                if (!base.Checked)
                {
                    pe.Graphics.FillRectangle(solidBrush, base.ClientRectangle);
                    DrawText(pe.Graphics);
                    ControlPaint.DrawBorder3D(pe.Graphics, base.ClientRectangle, Border3DStyle.Raised);
                }
                else
                {
                    solidBrush.Color = Colours.GetColour(Colours.Identities.WindowButtonDepressed);
                    pe.Graphics.FillRectangle(solidBrush, base.ClientRectangle);
                    DrawText(pe.Graphics);
                    ControlPaint.DrawBorder3D(pe.Graphics, base.ClientRectangle, Border3DStyle.Sunken);
                }
            }
        }

        private void DrawText(Graphics g)
        {
            Brush? brush = null;
            using ((!base.Enabled) ? (brush = new HatchBrush(HatchStyle.Percent50, DisabledForeColor, BackColor)) : (brush = new SolidBrush(ForeColor)))
            {
                using (StringFormat stringFormat = new StringFormat())
                using (StringFormat stringFormat2 = new StringFormat())
                using (StringFormat stringFormat3 = new StringFormat())
                {
                    if (_SubText == "")
                    {
                        stringFormat.LineAlignment = StringAlignment.Center;
                        stringFormat.Alignment = StringAlignment.Center;
                        g.DrawString(Text, Font, brush, base.ClientRectangle, stringFormat);
                        return;
                    }

                    stringFormat2.LineAlignment = StringAlignment.Far;
                    stringFormat2.Alignment = StringAlignment.Center;
                    stringFormat3.LineAlignment = StringAlignment.Near;
                    stringFormat3.Alignment = StringAlignment.Center;
                    Rectangle rectangle = new Rectangle(base.ClientRectangle.Left, base.ClientRectangle.Top, base.ClientRectangle.Width, (int)(ClientRectangle.Height * 0.55));
                    g.DrawString(Text, Font, brush, rectangle, stringFormat2);
                    Rectangle rectangle2 = new Rectangle(base.ClientRectangle.Left, rectangle.Bottom, base.ClientRectangle.Width, base.ClientRectangle.Height - rectangle.Height);
                    g.DrawString(_SubText, _SubFont, brush, rectangle2, stringFormat3);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            Refresh();
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            Refresh();
            base.OnMouseUp(mevent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }
    }
}
