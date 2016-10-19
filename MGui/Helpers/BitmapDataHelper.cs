using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    /// <summary>
    /// Creates a modifiable bitmap from the source and locks the image for editing.
    /// This essentially wraps <see cref="BitmapData"/> into something easier to deal with.
    /// </summary>
    /// <remarks>
    /// The pixel array is copied into managed memory. While slightly inefficient this allows operating outside an unsafe context.
    /// </remarks>
    public class BitmapDataHelper
    {
        public readonly int Height;
        public readonly int Width;
        private Bitmap _result;
        private readonly uint[] _data;
        private readonly int _strideInInts;
        private readonly BitmapData _bitmapData;

        /// <summary>
        /// Creates a modifiable bitmap from the source and locks the image for editing.
        /// Use <see cref="Unlock"/> to retrieve the result.
        /// </summary>                                      
        public BitmapDataHelper( Image source )
        {
            this._result = new Bitmap( source );
            Rectangle entirity = new Rectangle( 0, 0, _result.Width, _result.Height );

            this._bitmapData = _result.LockBits( entirity, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );

            this._strideInInts = _bitmapData.Stride / sizeof( UInt32 );
            int sizeInInts = _bitmapData.Height * _strideInInts;
            this._data = new UInt32[sizeInInts];
            this.Height = _bitmapData.Height;
            this.Width = _bitmapData.Width;

            // Marshal to avoid unsafe code...
            Marshal.Copy( _bitmapData.Scan0, (int[])(object)_data, 0, sizeInInts );
        }

        /// <summary>
        /// Gets or sets the pixel.
        /// </summary>             
        public uint this[int x, int y]
        {
            get { return _data[x + y * _strideInInts]; }
            set { _data[x + y * _strideInInts] = value; }
        }

        /// <summary>
        /// Retrieves the final image (can only be called once as the image is not relocked).
        /// </summary>                                          
        public Bitmap Unlock()
        {
            Marshal.Copy( (int[])(object)_data, 0, _bitmapData.Scan0, _data.Length );
            _result.UnlockBits( _bitmapData );
            var fr = _result;
            _result = null;
            return fr;
        }

        /// <summary>
        /// Gets the pixel at x, y, or returns 0 if the coordinates are out of range.
        /// </summary>                                                               
        public uint GetOrDefault( int x, int y )
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return 0;
            }

            return this[x, y];
        }
    }
}
