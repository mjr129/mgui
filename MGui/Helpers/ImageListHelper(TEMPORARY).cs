using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    //
    // TODO: TEMPORARY FILE UNTIL GIT UPDATE
    //

    public class ImageListHelper
    {
        public ImageList ImageList;
        Dictionary<object, int> keys = new Dictionary<object, int>();

        public ImageListHelper( ImageList smallImageList )
        {
            this.ImageList = smallImageList;
        }

        //
        // TODO: TEMPORARY FILE UNTIL GIT UPDATE
        //

        public void Dispose()
        {
            Clear();
            ImageList.Dispose();
        }

        public void Clear()
        {
            ImageList.Images.Clear();
            keys.Clear();
        }

        //
        // TODO: TEMPORARY FILE UNTIL GIT UPDATE
        //

        public int GetAssociatedImageIndex( object v, Func<Image> p )
        {
            int i;

            if (keys.TryGetValue( v, out i ))
            {
                return i;
            }

            Image image = p();
            i = ImageList.Images.Count;
            keys.Add( v, i );
            ImageList.Images.Add( image );
            return i;
        }
    }
}
