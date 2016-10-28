using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Controls
{
    public class CtlSplitter : SplitContainer
    {
        private static Brush _fill = Brushes.LightSlateGray;
        private static Pen _outline = Pens.LightSlateGray;
        private static Pen _outline2 = Pens.LightSlateGray;
        private static Pen _dots = new Pen( Color.DarkSlateGray, 2 );

        public CtlSplitter()
        {                                                                   
            this.SetStyle(ControlStyles.ResizeRedraw, true);      
            SplitterWidth = 6;
        }

        static CtlSplitter()
        {
            _dots.DashStyle = DashStyle.Dot;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int SplitterWidth
        {
            get
            {
                return base.SplitterWidth;
            }
            private set
            {
                base.SplitterWidth = value;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // NA
        }             

        protected override void OnPaint(PaintEventArgs e)
        {
            Draw( e.Graphics, this.Orientation == Orientation.Horizontal, this.SplitterRectangle, this.SplitterDistance );
        }

        public static void Draw( Graphics g, bool horizontal, Rectangle rectangle, int offset )
        {                        
            int gripLineWidth = 15; //9;

            if (horizontal)
            {
                int w = rectangle.Height;
                g.FillRectangle( _fill, rectangle.X, offset, rectangle.Width, w );
                g.DrawLine( _outline, rectangle.X, offset, rectangle.Width, offset );
                g.DrawLine( _outline2, rectangle.X, offset + w - 1, rectangle.Width, offset + w - 1 );
                g.DrawLine( _dots, ((rectangle.Width / 2) - (gripLineWidth / 2)), offset + w / 2, ((rectangle.Width / 2) + (gripLineWidth / 2)), offset + w / 2 );
            }
            else
            {
                int w = rectangle.Width;
                g.FillRectangle( _fill, offset, rectangle.Y, w, rectangle.Height );
                g.DrawLine( _outline, offset, rectangle.Y, offset, rectangle.Height );
                g.DrawLine( _outline2, offset + w - 1, rectangle.Y, offset + w - 1, rectangle.Height );
                g.DrawLine( _dots, offset + w / 2, ((rectangle.Height / 2) - (gripLineWidth / 2)), offset + w / 2, ((rectangle.Height / 2) + (gripLineWidth / 2)) );
            }
        }
    }
}
