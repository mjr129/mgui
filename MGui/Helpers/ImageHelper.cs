using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap Resize( this Image image, int width, int height )
        {
            var src = new Rectangle( 0, 0, image.Width, image.Height );
            var dest = new Rectangle( 0, 0, width, height );
            var result = new Bitmap( width, height );                                    

            using (Graphics graphics = Graphics.FromImage( result ))
            {
                graphics.DrawImage( image, dest, src, GraphicsUnit.Pixel );
            }

            return result;
        }    
    }
}
