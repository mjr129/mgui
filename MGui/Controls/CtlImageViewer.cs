using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace MGui.Controls
{
    public class CtlImageViewer : Control
    {
        // Property fields
        private Image _image;
        private Image _transparentImage;
        private PointF _offset;
        private float _xZoom = 1.0f;
        private float _yZoom = 1.0f;

        // Behaviour property fields
        private const float _mouseZoom = 1.5f;
        bool _animateMouseZoom = true;
        private const InterpolationMode _moveInterpolationMode = InterpolationMode.NearestNeighbor;
        private const InterpolationMode _normalInterpolationMode = InterpolationMode.NearestNeighbor;
        private const MouseButtons _panButtons = MouseButtons.Left | MouseButtons.Middle;   

        // Drawing fields
        private Bitmap _buffer;
        private bool _needsRedraw = true;
        private bool _isPanning = false;
        private Point? _pickPoint = null;

        // Mousemove fields
        private Point _mouseDownPoint;
        private PointF _mouseDownOffset;
        private Point _mouseMovePoint;

        // Constants
        private const float _minZoom = 0.1f;
        private const float _maxZoom = 100.0f;
        private const float _pixelGridZoom = 16.0f;

        /// <summary>
        /// Which image is being displayed
        /// </summary>
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Point highlighted on image.
        /// </summary>
        public Point? PickPoint
        {
            get
            {
                return _pickPoint;
            }
            set
            {
                _pickPoint = value;
            }
        }

        /// <summary>
        /// Whether mouse zooming is smooth.
        /// </summary>
        public bool AnimateMouseZoom
        {
            get
            {
                return _animateMouseZoom;
            }
            set
            {
                _animateMouseZoom = value;
            }
        }

        /// <summary>
        /// Image displayed in the background
        /// </summary>
        public Image TransparentImage
        {
            get
            {
                return _transparentImage;
            }
            set
            {
                _transparentImage = value;
                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Zoom level
        /// </summary>
        public float ZoomX
        {
            get
            {
                return _xZoom;
            }
            set
            {
                _xZoom = value;

                if (_xZoom < _minZoom)
                {
                    _xZoom = _minZoom;
                }
                else if (_xZoom > _maxZoom)
                {
                    _xZoom = _maxZoom;
                }

                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Zoom level
        /// </summary>
        public float ZoomY
        {
            get
            {
                return _yZoom;
            }
            set
            {
                _yZoom = value;

                if (_xZoom < _minZoom)
                {
                    _yZoom = _minZoom;
                }
                else if (_yZoom > _maxZoom)
                {
                    _yZoom = _maxZoom;
                }

                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Zoom level
        /// </summary>
        public float Zoom
        {
            get
            {
                return ZoomX;
            }
            set
            {
                ZoomX = value;
                ZoomY = value;
            }
        }

        /// <summary>
        /// Which point of the target image appears in the centre of the control.
        /// This is conserved when zooming.
        /// </summary>
        public PointF Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Which point of the target image appears in the top-left of the control.
        /// This is not conserved when zooming.
        /// </summary>
        PointF TopLeftOffset
        {
            get
            {
                return new PointF(
                    _offset.X - (int)(ClientSize.Width / 2.0f / _xZoom),
                    _offset.Y - (int)(ClientSize.Height / 2.0f / _yZoom));
            }
            set
            {
                _offset.X = value.X + (int)(ClientSize.Width / 2.0f / _xZoom);
                _offset.Y = value.Y + (int)(ClientSize.Height / 2.0f / _yZoom);

                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Where drawing is centred about.
        /// </summary>
        private PointF DrawingOffset
        {
            get
            {
                return TopLeftOffset;
            }
        }

        public enum ZoomMode
        {
            ActualSize,
            ZoomToFit,
            ZoomAndStretch,
        }

        /// <summary>
        /// Loads image at default zoom and position.
        /// (If you DON'T wish to reposition use the <see cref="Image"/> property instead.)
        /// </summary>
        public void LoadImage(Image image, ZoomMode zoomMode)
        {
            _image = image;
            _xZoom = _maxZoom;
            _yZoom = _maxZoom;
            _offset = new Point( image.Width / 2, image.Height / 2 );
            Reposition( zoomMode );
        }

        /// <summary>
        /// Makes the image large enough so its maximum dimension fits snugly in the control.
        /// </summary>
        public void Reposition(ZoomMode zoomMode) 
        {
            switch (zoomMode)
            {
                case ZoomMode.ActualSize:
                    _xZoom = 1.0f;
                    _yZoom = 1.0f;
                    TopLeftOffset = new Point(0, 0);
                    break;

                case ZoomMode.ZoomToFit:
                    _xZoom = (float)ClientSize.Width / _image.Width;
                    _yZoom = _xZoom;
                    Offset = new Point(_image.Width / 2, _image.Height / 2);
                    break;

                case ZoomMode.ZoomAndStretch:
                    _xZoom = (float)ClientSize.Width/_image.Width ;
                    _yZoom = (float)ClientSize.Height/_image.Height ;
                    Offset = new Point( _image.Width / 2, _image.Height / 2 );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Repositions the image with the option for a smooth animation.
        /// </summary>
        public void Reposition(Point newOffset, float newZoomX, float newZoomY, bool animate)
        {
            newZoomX = Math.Min(Math.Max( newZoomX, _minZoom), _maxZoom);
            newZoomY = Math.Min( Math.Max( newZoomY, _minZoom ), _maxZoom );

            if (animate)
            {
                PointF origOffset = _offset;
                float origZoomX = ZoomX;
                float origZoomY = ZoomY;

                for (float f = 0.1f; f <= 0.9f; f += 0.1f)
                {
                    _offset.X = Interpolate(origOffset.X, newOffset.X, f);
                    _offset.Y = Interpolate(origOffset.Y, newOffset.Y, f);
                    _xZoom = Interpolate(origZoomX, newZoomX, f);
                    _yZoom = Interpolate( origZoomY, newZoomY, f );
                    InvalidateBuffer();
                    this.Refresh();
                }
            }

            _offset = newOffset;
            _xZoom = newZoomX;
            _yZoom = newZoomY;
            InvalidateBuffer();
        }

        /// <summary>
        /// Redraws the image.
        /// </summary>
        private void InvalidateBuffer()
        {
            _needsRedraw = true;
            Invalidate();
        }

        /// <summary>
        /// Don't paint a background as we paint over everything.
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // base.OnPaintBackground(pevent);
        }

        /// <summary>
        /// Handle the painting.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // base.OnPaint(e);

            if (_needsRedraw)
            {
                Render();
            }

            e.Graphics.DrawImage(_buffer, 0, 0);
        }

        /// <summary>
        /// Core paint routine.
        /// </summary>
        private void Render()
        {
            // Ensure buffer of correct size exists
            Size clientSize = ClientSize;

            // Recreate the buffer if necessary
            if (_buffer == null || _buffer.Size != clientSize)
            {
                if (_buffer != null)
                {
                    _buffer.Dispose();
                }

                _buffer = new Bitmap(clientSize.Width, clientSize.Height);
            }

            using (Graphics g = Graphics.FromImage(_buffer))
            {
                // Draw the tiled background
                DrawTiledBackground(g, clientSize);

                // Draw the foreground
                DrawForegroundImage(g, clientSize);

                // Draw extra debugging info
                //DrawExtraInfo(g);
            }
        }

        /// <summary>
        /// Drawing phase 1.
        /// </summary>
        private void DrawTiledBackground(Graphics g, Size size)
        {
            if (_transparentImage == null)
            {
                g.Clear(BackColor);
                return;
            }

            for (int y = 0; y < size.Width; y += _transparentImage.Size.Height)
            {
                for (int x = 0; x < size.Width; x += _transparentImage.Size.Width)
                {
                    g.DrawImage(_transparentImage, x, y);
                }
            }
        }

        /// <summary>
        /// Drawing phase 2.
        /// </summary>
        private void DrawForegroundImage(Graphics g, Size size)
        {
            if (_image == null)
            {
                return;
            }

            g.InterpolationMode = _isPanning ? _moveInterpolationMode : _normalInterpolationMode;

            PointF trueOffset = DrawingOffset;
            float xLoc = trueOffset.X;
            float yLoc = trueOffset.Y;

            if (_xZoom < _minZoom) _xZoom = _minZoom;
            if (_xZoom > _maxZoom) _xZoom = _maxZoom;
            if (_yZoom < _minZoom) _yZoom = _minZoom;
            if (_yZoom > _maxZoom) _yZoom = _maxZoom;

            float xSize = (float)(size.Width / _xZoom);
            float ySize = (float)(size.Height / _yZoom);

            RectangleF srcRect = new RectangleF(xLoc - 0.5f, yLoc - 0.5f, xSize, ySize);
            RectangleF dstRect = new RectangleF(0, 0, size.Width, size.Height);

            g.DrawImage(_image, dstRect, srcRect, GraphicsUnit.Pixel);

            if (_xZoom > _pixelGridZoom && _yZoom > _pixelGridZoom)
            {
                using (Pen pen = new Pen(Color.Gray))
                {
                    DrawPixelGrid(g, xLoc, yLoc, xSize, ySize, pen);
                }
            }

            if (_pickPoint.HasValue)
            {
                using (Pen pen = new Pen(Color.Black))
                {
                    DrawPoint(g, _pickPoint.Value, pen);
                }
            }
        }

        /// <summary>
        /// Drawing phase 2.1.
        /// </summary>
        private void DrawPixelGrid(Graphics g, float xLoc, float yLoc, float xSize, float ySize, Pen pen)
        {
            for (float y = yLoc; y <= yLoc + ySize; y++)
            {
                for (float x = xLoc; x <= xLoc + xSize; x++)
                {
                    PointF p = new PointF(x, y);
                    p = DrawPoint(g, p, pen);
                }
            }
        }

        /// <summary>
        /// Drawing phase 2.2.
        /// </summary>
        private PointF DrawPoint(Graphics g, PointF p, Pen pen)
        {
            PointF screen = ImageToScreen(p);
            float pickPointSizeX = Math.Max(1, _xZoom);
            float pickPointSizeY = Math.Max( 1, _yZoom );
                                            
            g.DrawRectangle(pen, screen.X, screen.Y, pickPointSizeX, pickPointSizeY );
            return p;
        }

        /// <summary>
        /// Drawing phase 3.
        /// </summary>
        private void DrawExtraInfo(Graphics g)
        {
            string dbg = "Zx:" + _xZoom.ToString() + " / Zy:"+_yZoom+" / O:" + _offset + " / I:" + _pickPoint + " / S:" + _mouseMovePoint;
            SizeF sz = g.MeasureString(dbg, Font);
            g.FillRectangle(Brushes.White, new RectangleF(0.0f, 0.0f, sz.Width, sz.Height));
            g.DrawString(dbg, Font, Brushes.Blue, 0, 0);
        }

        /// <summary>
        /// Gets the point on the image represented by a point on the screen.
        /// </summary>
        public Point ScreenToImage(Point point)
        {
            PointF trueOffset = DrawingOffset;

            return new Point(
                (int)((point.X / _xZoom) + trueOffset.X),
                (int)((point.Y / _yZoom) + trueOffset.Y));
        }

        /// <summary>
        /// Gets the point on the image represented by a point on the screen.
        /// </summary>
        public PointF ScreenToImage( PointF point )
        {
            PointF trueOffset = DrawingOffset;

            return new PointF(
                (point.X / _xZoom) + trueOffset.X,
                (point.Y / _yZoom) + trueOffset.Y );
        }

        /// <summary>
        /// Gets a point on the screen represented by a point on the image.
        /// </summary>
        public Point ImageToScreen(Point point)
        {
            PointF trueOffset = DrawingOffset;

            return new Point(
                 (int)((point.X - trueOffset.X) * _xZoom),
                 (int)((point.Y - trueOffset.Y) * _yZoom));
        }

        /// <summary>
        /// Gets a point on the screen represented by a point on the image.
        /// </summary>
        public PointF ImageToScreen( PointF point )
        {
            PointF trueOffset = DrawingOffset;

            return new PointF(
                 (point.X - trueOffset.X) * _xZoom,
                 (point.Y - trueOffset.Y) * _yZoom );
        }

        /// <summary>
        /// Handle mousedown.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            if ((e.Button & _panButtons) != MouseButtons.None)
            {
                // Record current offset and mouse point
                _mouseDownPoint = e.Location;
                _mouseDownOffset = _offset;

                // Redraw
                _isPanning = true;
                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Handle mousemove.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ((e.Button & _panButtons) != MouseButtons.None && _isPanning)
            {
                // Calculate delta
                float deltaX = (e.Location.X - _mouseDownPoint.X) / _xZoom;
                float deltaY = (e.Location.Y - _mouseDownPoint.Y) / _yZoom;          

                // Offset new delta
                _offset = new PointF(_mouseDownOffset.X - deltaX, _mouseDownOffset.Y - deltaY);       
            }

            _mouseMovePoint = e.Location;
            InvalidateBuffer();
        }

        /// <summary>
        /// Handle mouse up.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if ((e.Button & _panButtons) != MouseButtons.None && _isPanning)
            {
                _isPanning = false;
                InvalidateBuffer();
            }
        }

        /// <summary>
        /// Handle mouse wheel.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            float zx;
            float zy;

            if (e.Delta < 0)
            {
                zx = Math.Max(ZoomX / _mouseZoom, _minZoom);
                zy = Math.Max( ZoomY / _mouseZoom, _minZoom );
            }
            else if (e.Delta > 0)
            {
                zx = Math.Min(ZoomX * _mouseZoom, _maxZoom);
                zy = Math.Min( ZoomY * _mouseZoom, _maxZoom );
            }
            else
            {
                return;
            }

            PointF origOffset = _offset;
            float origZoomX = ZoomX;
            float origZoomY = ZoomY;
            PointF o = ScreenToImage(new PointF(e.Location.X, e.Location .Y));

            // o is the point under the mouse cursor (e.Location)
            // o should still be under the mouse cursor after zooming
            _xZoom = zx;
            _yZoom = zy; 
            _offset = o;
            PointF a = o;
            PointF d = new PointF(a.X - o.X, a.Y - o.Y);
            o.X -= d.X;
            o.Y -= d.Y;

            if (AnimateMouseZoom)
            {
                for (float f = 0.1f; f <= 0.9f; f += 0.1f)
                {
                    _offset.X = Interpolate(origOffset.X, o.X, f);
                    _offset.Y = Interpolate(origOffset.Y, o.Y, f);
                    _xZoom = Interpolate(origZoomX, zx, f);
                    _yZoom = Interpolate( origZoomY, zy, f );
                    InvalidateBuffer();
                    this.Refresh();
                }
            }

            _offset = o;
            _xZoom = zx;
            _yZoom = zy;
            InvalidateBuffer();
        }

        private float Interpolate(float a, float b, float v)
        {
            return (b - a) * v + a;
        }

        private int Interpolate(int a, int b, float v)
        {
            return (int)Interpolate((float)a, (float)b, v);
        }
    }
}