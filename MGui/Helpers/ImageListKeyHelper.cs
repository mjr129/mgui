using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    public class ImageListKeyHelper : IDisposable
    {
        private readonly Dictionary<object, string> _objectAssociations = new Dictionary<object, string>();
        private readonly Dictionary<Image, string> _imageKeys = new Dictionary<Image, string>();
        private readonly ImageList _imageList;                      
        private int _nextKey = 0;

        public ImageListKeyHelper()
        {
            this._imageList = new ImageList();
        }

        public ImageListKeyHelper( ImageList imageList )
        {
            this._imageList = imageList;
        }

        public ImageList ImageList => _imageList;

        /// <summary>
        /// Clears all images and associations.
        /// </summary>
        public void Clear()
        {
            _objectAssociations.Clear();
            _imageList.Images.Clear();
            _imageKeys.Clear();
        }

        /// <summary>
        /// Gets the index associated with the object, creating the image if it doesn't exist
        /// </summary>                                                               
        /// <remarks>
        /// For use when the image is generated and needs to be associated with an object.
        /// </remarks>
        public string GetAssociatedImageKey( object x, Func<Image> provider )
        {
            string result;

            if (_objectAssociations.TryGetValue( x, out result ))
            {
                return result;
            }

            Image image = provider();
            result = GetKey( image );
            _objectAssociations.Add( x, result );
            return result;
        }

        /// <summary>
        /// Removes the association between an object and its image.
        /// </summary>                                              
        public void RemoveAssociation( object x )
        {
            string removedIndex;

            if (!_objectAssociations.TryGetValue( x, out removedIndex ))
            {
                // Nothing to do
                return;
            }

            _objectAssociations.Remove( x );

            // We can only remove the index if it's not in use
            TryRemoveKey( removedIndex );
        }

        /// <summary>
        /// Tries to removes an image by index from the ImageList.
        /// 
        /// The image/index is NOT removed if any objects are associated with it.
        /// </summary>                
        private bool TryRemoveKey( string key )
        {
            if (_objectAssociations.Any( z => z.Value == key ))
            {
                return false;
            }

            RemoveKey( key );

            return true;
        }

        /// <summary>
        /// Removes an image by index from the ImageList.
        /// </summary>
        /// <remarks>
        /// Time: O(n)
        /// This is UNSAFE if objects are still associated with the image - use TryRemoveIndex instead.
        /// The index/image is not actually removed since this would upset the other indices. Instead it is marked as available.
        /// The image is not Dispose'd, since we cannot be sure whether it is in use elsewhere.
        /// </remarks>
        /// <param name="key"></param>
        private void RemoveKey( string key )
        {
            Image image = _imageList.Images[key];

            _imageKeys.Remove( image );
            _imageList.Images.RemoveByKey( key );
        }

        /// <summary>
        /// Gets the index associated with the image, creating it if it doesn't exist
        /// </summary>                                                               
        /// <remarks>
        /// This only works where references to images stay intact.
        /// Notably Resource.Xyz returns a different image each time, which is why <see cref="GetAssociatedImageIndex(object, Func{Image})"/> exists.
        /// </remarks>
        public string GetKey( Image image )
        {
            string result;

            if (_imageKeys.TryGetValue( image, out result ))
            {
                return result;
            }

            result = (++_nextKey).ToString();

            _imageList.Images.Add( result, image );   
            _imageKeys.Add( image, result );
            Debug.WriteLine( "_imageList.Count is " + _imageList.Images.Count );
            return result;
        }

        public void Dispose()
        {
            _imageList.Dispose();
            Clear();
        }
    }
}
