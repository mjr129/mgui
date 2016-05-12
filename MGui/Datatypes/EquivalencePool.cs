using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    /// <summary>
    /// Reduces memory usage by finding equivalent objects.
    /// Objects must implement Equals/GetHashCode.
    /// </summary>                                         
    public class EquivalencePool<T> : IEnumerable<T>
    {
        Dictionary<T, T> _contents = new Dictionary<T, T>();

        public bool Has( T x )
        {
            return _contents.ContainsKey( x );
        }                           

        public T Find( T x )
        {                   
            T y;

            if (_contents.TryGetValue( x, out y ))
            {
                return y;
            }

            _contents.Add( x, x );
            return x;
        }

        public void Clear()
        {
            _contents.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _contents.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
