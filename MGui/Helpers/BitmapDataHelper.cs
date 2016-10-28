using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
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
            Rectangle entirity = new Rectangle( 0, 0, this._result.Width, this._result.Height );

            this._bitmapData = this._result.LockBits( entirity, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );

            this._strideInInts = this._bitmapData.Stride / sizeof( UInt32 );
            int sizeInInts = this._bitmapData.Height * this._strideInInts;
            this._data = new UInt32[sizeInInts];
            this.Height = this._bitmapData.Height;
            this.Width = this._bitmapData.Width;

            // Marshal to avoid unsafe code...
            Marshal.Copy( this._bitmapData.Scan0, (int[])(object)this._data, 0, sizeInInts );
        }

        /// <summary>
        /// Gets or sets the pixel.
        /// </summary>             
        public uint this[int x, int y]
        {
            get { return this._data[x + y * this._strideInInts]; }
            set { this._data[x + y * this._strideInInts] = value; }
        }

        /// <summary>
        /// Retrieves the final image (can only be called once as the image is not relocked).
        /// </summary>                                          
        public Bitmap Unlock()
        {
            Marshal.Copy( (int[])(object)this._data, 0, this._bitmapData.Scan0, this._data.Length );
            this._result.UnlockBits( this._bitmapData );
            var fr = this._result;
            this._result = null;
            return fr;
        }

        /// <summary>
        /// Gets the pixel at x, y, or returns 0 if the coordinates are out of range.
        /// </summary>                                                               
        public uint GetOrDefault( int x, int y )
        {
            if (x < 0 || x >= this.Width || y < 0 || y >= this.Height)
            {
                return 0;
            }

            return this[x, y];
        }
    }
}
