﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    public class ImageListIndexHelper : IDisposable
    {
        private readonly Dictionary<object, int> _objectAssociations = new Dictionary<object, int>();
        private readonly Dictionary<Image, int> _imageIndices = new Dictionary<Image, int>();
        private readonly ImageList _imageList;                      

        public ImageListIndexHelper()
        {
            this._imageList = new ImageList();
        }

        public ImageListIndexHelper( ImageList imageList )
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
            _imageIndices.Clear();   
        }

        /// <summary>
        /// Gets the index associated with the object, creating the image if it doesn't exist
        /// </summary>                                                               
        /// <remarks>
        /// For use when the image is generated and needs to be associated with an object.
        /// </remarks>
        public int GetAssociatedImageIndex( object x, Func<Image> provider )
        {
            int result;

            if (_objectAssociations.TryGetValue( x, out result ))
            {
                return result;
            }

            Image image = provider();  
            result = GetIndex( image );
            _objectAssociations.Add( x, result );
            return result;
        }

        /// <summary>
        /// Gets the index associated with the image, creating it if it doesn't exist
        /// </summary>                                                               
        /// <remarks>
        /// This only works where references to images stay intact.
        /// Notably Resource.Xyz returns a different image each time, which is why <see cref="GetAssociatedImageIndex(object, Func{Image})"/> exists.
        /// </remarks>
        public int GetIndex( Image image )
        {          
            int result;

            if (_imageIndices.TryGetValue( image, out result ))
            {
                return result;
            }                              

            result = _imageList.Images.Count;
            _imageList.Images.Add( image );
            _imageIndices.Add( image, result );
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
