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

    public class BitmapDataHelper
    {
        private Bitmap source;
        private readonly BitmapData data;
        uint[] dataa;
        private readonly int strideinints;

        public BitmapDataHelper( Image source )
        {
            this.source = new Bitmap( source );
            Rectangle all = new Rectangle( 0, 0, Width, Height );
            data= this.source.LockBits( all, ImageLockMode.ReadWrite,  PixelFormat.Format32bppArgb );
            strideinints= data.Stride / sizeof( int );
            dataa = new uint[data.Height * strideinints]; 
            Marshal.Copy( data.Scan0, (int[])(object)dataa, 0, dataa .Length);
        }

        public int Height => source.Height;
        public int Width => source.Width;

        public Bitmap Unlock()
        {
            Marshal.Copy( (int[])(object)dataa, 0, data.Scan0,    dataa.Length);
            source.UnlockBits( data );
            return source;
        }

        public uint GetOrDefault(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return 0;
            }

            return this[x, y];
        }

        public uint this[int x, int y]
        {
            get
            {
                return dataa[x + strideinints * y];
            }
            set
            {
                dataa[x + strideinints * y] = value; ;
            }
        }
    }
}
