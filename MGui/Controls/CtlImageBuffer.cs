using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Controls
{
    public class CtlImageBuffer : Control
    {
        private Bitmap _buffer;
        private bool _dirty;
        public event RenderEventHandler Render;

        private void EnsureBuffer()
        {       
            if (_buffer == null || _buffer.Size != ClientSize)
            {
                this._buffer?.Dispose();

                _buffer = new Bitmap( ClientSize.Width, ClientSize.Height );
            }
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            EnsureBuffer();

            if (_dirty)
            {
                using (Graphics g = Graphics.FromImage( _buffer ))
                {       
                    RenderEventArgs ev = new RenderEventArgs( _buffer, g );

                    OnRender( ev );
                }
            }

            e.Graphics.DrawImage( _buffer, 0, 0 );
        }   

        /// <summary>
        /// Flags the buffer as dirty and causes an invalidation.
        /// </summary>
        public void Rerender()
        {
            _dirty = true;
            Invalidate();
        }

        protected virtual void OnRender( RenderEventArgs e )
        {
            if (Render != null)
            {
                Render( this, e );
            }
        }

        protected override void OnPaintBackground( PaintEventArgs pevent )
        {
            // NA
        }
    }

    public delegate void RenderEventHandler( object sender, RenderEventArgs e );

    public class RenderEventArgs : EventArgs
    {
        public RenderEventArgs( Bitmap buffer, Graphics graphics )
        {
            Bitmap = buffer;
            Graphics = graphics;
        }

        public Graphics Graphics { get; private set; }

        public Bitmap Bitmap { get; private set; }
    }
}
