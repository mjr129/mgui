using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Controls
{
    public class DrawBufferEventArgs : EventArgs
    {
        public readonly Graphics Graphics;
        public readonly Bitmap Bitmap;

        public DrawBufferEventArgs( Graphics graphics, Bitmap buffer )
        {
            this.Graphics = graphics;
            this.Bitmap = buffer;
        }
    }

    public class CtlBuffer : Control
    {
        Bitmap _buffer;
        bool _isDirty;

        public event EventHandler<DrawBufferEventArgs> DrawBuffer;

        public CtlBuffer()
        {
        }

        protected override void Dispose( bool disposing )
        {
            if (disposing)
            {
                _buffer.Dispose();
            }

            base.Dispose( disposing );
        }

        public void Rerender()
        {
            _isDirty = true;
            Invalidate();
        } 

        protected override void OnPaint( PaintEventArgs e )
        {
            if (ClientSize.Width == 0 || ClientSize.Height==0)
            {
                return;
            }

            if (_buffer == null || _buffer.Width != ClientSize.Width || _buffer.Height != ClientSize.Height)
            {
                if (_buffer != null)
                {
                    _buffer.Dispose();
                }

                _buffer = new Bitmap( ClientSize.Width, ClientSize.Height );
                _isDirty = true;
            }

            if (_isDirty)
            {   
                DrawBufferInternal();
                _isDirty = false;
            }

            e.Graphics.DrawImage( _buffer, 0, 0 );
        }

        private void DrawBufferInternal()
        {
            using (Graphics g = Graphics.FromImage( _buffer ))
            {
                DrawBufferEventArgs args = new DrawBufferEventArgs( g, _buffer );
                OnDrawBuffer(args);
            }
        }

        protected virtual void OnDrawBuffer( DrawBufferEventArgs args )
        {
            if (DrawBuffer != null)
            {
                DrawBuffer( this, args );
            }
        }
    }
}
