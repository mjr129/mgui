using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    /// <summary>
    /// Counts the number of each item.
    /// </summary>                      
    public class Counter<T> : IEnumerable<KeyValuePair<T, int>>
    {
        Dictionary<T, int> _counts;   

        public Counter()
        {
            _counts = new Dictionary<T, int>();
        }

        public void Increment( T key )
        {
            this[key] = this[key] + 1;
        }

        public int this[T key]
        {
            get
            {
                int current;

                if (!_counts.TryGetValue( key, out current ))
                {
                   return 0;
                }

                return current;
            }
            set
            {
                _counts[key] = value;
            }
        }

        public int Count => _counts.Count;

        public void Clear()
        {
            _counts.Clear();
        }

        public KeyValuePair<T, int> FindMax()
        {
            T result = default( T );
            int resultCount = int.MinValue;

            foreach (var kvp in _counts)
            {
                if (kvp.Value > resultCount)
                {
                    result = kvp.Key;
                    resultCount = kvp.Value;
                }
            }

            return new KeyValuePair<T, int>( result, resultCount );
        }

        public int FindMinNonZero()
        {
            return _counts.Values.Where( z => z > 0 ).Min();
        }

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T, int>>)this._counts).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T, int>>)this._counts).GetEnumerator();
        }

        public IEnumerable<T> Find( int n )
        {
            foreach (var kvp in _counts)
            {
                if (kvp.Value == n)
                {
                    yield return kvp.Key;
                }
            }
        }
    }
}
