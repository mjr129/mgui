using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
{
    /// <summary>
    /// A suite of functions for assisting with arrays.
    /// Most of these are extension methods.
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// Ensures array size is >= needed.
        /// O(n)
        /// </summary>      
        public static void EnsureCapacity<T>( ref T[] array, int sizeNeeded, Func<int, T> creator = null )
        {                   
            if (array.Length > sizeNeeded)
            {
                return;
            }

            int newSize;

            if (array.Length > (int.MaxValue / 2))
            {
                newSize = int.MaxValue;
            }
            else
            {
                newSize = array.Length * 2;
            }

            if (newSize < sizeNeeded)
            {
                newSize = sizeNeeded + 1;
            }

            T[] newArray = new T[newSize];
            Array.Copy( array, newArray, array.Length );

            if (creator != null)
            {
                for (int n = array.Length; n < newArray.Length; n++)
                {
                    newArray[n] = creator( n );
                }
            }

            array = newArray;
        }  

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Is the <paramref name="enumerable"/> empty?
        /// </summary>
        public static bool IsEmpty( this IEnumerable enumerable )
        {
            return !enumerable.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Returns the unique elements (i.e. ToHashSet).
        /// </summary>
        public static HashSet<T> Unique<T>( this IEnumerable<T> self )
        {
            return new HashSet<T>( self );
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Returns the unique elements (i.e. ToHashSet).
        /// </summary>
        public static HashSet<TResult> Unique<T, TResult>( this IEnumerable<T> self, Func<T, TResult> selector )
        {
            return new HashSet<TResult>( self.Select( selector ) );
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Like <see cref="Linq.Concat"/> but for single elements
        /// </summary>
        public static IEnumerable<T> ConcatSingle<T>( this IEnumerable<T> self, T toAdd )
        {
            return self.Concat( new T[] { toAdd } );
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Appends item(s) to an array and returns the result.
        /// The original array is unmodified.
        /// </summary>
        public static T[] Append<T>( this T[] self, params T[] toAdd )
        {
            int p = self.Length;
            T[] result = new T[p + toAdd.Length];

            for (int n = 0; n < self.Length; n++)
            {
                result[n] = self[n];
            }                       

            for (int i = 0; i < toAdd.Length; i++)
            {
                self[p + i] = toAdd[i];
            }

            return self;
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Converts or returns a reference to the list.
        /// </summary>
        public static List<T> AsList<T>( this IEnumerable<T> enumerable )
        {
            if (enumerable == null)
            {
                return null;
            }

            var list = enumerable as List<T>;

            if (list != null)
            {
                return list;
            }

            return enumerable.ToList();
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Replaces the contents of this list with another.
        /// </summary>
        public static void ReplaceAll<T>( this List<T> self, IEnumerable<T> newContent )
        {
            if (newContent == self)
            {
                return;
            }

            self.Clear();
            self.AddRange( newContent );
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Replaces a single element of the enumerable.
        /// An exception is thrown if more than one instance of that element exists, or if the
        /// element does not exist, in the original array.
        /// </summary>                                    
        public static IEnumerable<T> ReplaceSingle<T>( this IEnumerable<T> self, T find, T replace )
        {
            bool hasFound = false;

            foreach (T t in self)
            {
                if (t.Equals( find ))
                {
                    if (hasFound)
                    {
                        throw new InvalidOperationException( "ReplaceSingle contained more than one element to find." );
                    }

                    hasFound = true;
                    yield return replace;
                }
                else
                {
                    yield return t;
                }
            }

            if (!hasFound)
            {
                throw new InvalidOperationException( "ReplaceSingle did not contain the element to find." );
            }
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Provides AddRange functionality for a <see cref="HashSet{T}"/>.
        /// </summary>                                                 
        public static void AddRange<T>( this HashSet<T> self, IEnumerable<T> toAdd )
        {
            foreach (T o in toAdd)
            {
                self.Add( o );
            }
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Provides AddRange functionality for a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>             
        public static void AddRange<TKey, TValue>( this Dictionary<TKey, TValue> self, Dictionary<TKey, TValue> toAdd )
        {
            foreach (KeyValuePair<TKey, TValue> kvp in toAdd)
            {
                self.Add( kvp.Key, kvp.Value );
            }
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Provides AddRange functionality for a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>             
        public static void AddRange<TKey, TValue>( this Dictionary<TKey, TValue> self, IEnumerable<TValue> toAdd, Converter<TValue, TKey> keySelector )
        {
            foreach (TValue kvp in toAdd)
            {
                self.Add( keySelector( kvp ), kvp );
            }
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Provides AddRange functionality for a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>             
        public static void AddRange<TKey, TValue, TEnum>( this Dictionary<TKey, TValue> self, IEnumerable<TEnum> toAdd, Converter<TEnum, TKey> keySelector, Converter<TEnum, TValue> valueSelector )
            where TEnum : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {
            foreach (TEnum kvp in toAdd)
            {
                self.Add( keySelector( kvp ), valueSelector( kvp ) );
            }
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Returns the indices of the enumerable.
        /// R: seq_along(x)
        /// </summary>
        public static IEnumerable<int> Indices( this IList self )
        {
            return Enumerable.Range( 0, self.Count );
        }    

        /// <summary>
        /// (MJR) Compares all adjacent elements
        /// </summary>
        public static bool CompareAdjacent<T>( this IEnumerable<T> self, Func<T, T, bool> comparer )
        {
            T prevT = default( T );
            bool hasT = false;

            foreach (T t in self)
            {
                if (hasT)
                {
                    if (!comparer( prevT, t ))
                    {
                        return false;
                    }
                }
                else
                {
                    hasT = true;
                }

                prevT = t;
            }

            return true;
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Returns the indices of the enumerable
        /// R equivalent of: seq_along(self)
        /// </summary>
        public static IEnumerable<int> Indices( this IEnumerable self )
        {
            var e = self.GetEnumerator();
            int i = -1;

            while (e.MoveNext())
            {
                yield return ++i;
            }
        }

        /// <summary>
        /// (MJR) Yields a set of KVPs containing the objects and their indices
        /// </summary>
        public static IEnumerable<KeyValuePair<int, T>> IndicesAndObject<T>( this IEnumerable<T> self )
        {
            int i = 0;

            foreach (T o in self)
            {
                yield return new KeyValuePair<int, T>( i++, o );
            }
        }

        /// <summary>
        /// (MJR) Returns the value of the [key], if not present a new one is automatically added using new [TValue]().
        /// </summary>
        public static TValue GetOrNew<TKey, TValue>( this Dictionary<TKey, TValue> dict, TKey key )
            where TValue : new()
        {
            TValue value;

            if (!dict.TryGetValue( key, out value ))
            {
                value = new TValue();
                dict.Add( key, value );
            }

            return value;
        }

        /// <summary>
        /// (MJR) Returns the value of the [key], if not present a new one is automatically added using [creator].
        /// </summary>
        public static TValue GetOrCreate<TKey, TValue>( this Dictionary<TKey, TValue> dict, TKey key, Converter<TKey, TValue> creator )
        {
            TValue value;

            if (!dict.TryGetValue( key, out value ))
            {
                value = creator( key );
                dict.Add( key, value );
            }

            return value;
        }

        /// <summary>
        /// (MJR) Returns the value of the [key], if not present a new one is automatically added using new [TValue]()
        /// and initialised using [action].
        /// </summary>
        public static TValue GetOrNew<TKey, TValue>( this Dictionary<TKey, TValue> dict, TKey key, Action<TKey, TValue> action )
            where TValue : new()
        {
            TValue value;

            if (!dict.TryGetValue( key, out value ))
            {
                value = new TValue();
                dict.Add( key, value );
                action( key, value );
            }

            return value;
        }

        /// <summary>
        /// (MJR) Returns the value of the [key], if not present returns double.NaN.
        /// </summary>
        public static double GetOrNan<TKey>( this Dictionary<TKey, double> dict, TKey key )
        {
            double value;

            if (!dict.TryGetValue( key, out value ))
            {
                return double.NaN;
            }

            return value;
        }

        public static TValue GetOrDefault<TKey, TValue>( this Dictionary<TKey, TValue> dict, TKey key )
        {
            TValue value;

            if (!dict.TryGetValue( key, out value ))
            {
                return default( TValue );
            }

            return value;
        }

        /// <summary>
        /// (MJR) Returns the value of the [key], if not present returns [defaultValue].
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>( this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue )
        {
            TValue value;

            if (!dict.TryGetValue( key, out value ))
            {
                return defaultValue;
            }

            return value;
        }

        /// <summary>
        /// (MJR) Counts items in enumerable. Iteratively.
        /// </summary>
        public static int CountAll( this IEnumerable self )
        {
            int i = 0;
            var e = self.GetEnumerator();

            while (e.MoveNext())
            {
                i++;
            }

            return i;
        }

        /// <summary>
        /// Truncates the list to the specified size.
        /// </summary>
        public static void TrimList<T>( List<T> list, int max )
        {
            if (list.Count > max)
            {
                list.RemoveRange( max, list.Count - max );
            }
        }

        /// <summary>
        /// (MJR) Converts an enumerable to a dictionary.
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionary<TEnumerable, TKey, TValue>( this IEnumerable<TEnumerable> enumerable, Converter<TEnumerable, TKey> keySelector, Converter<TEnumerable, TValue> valueSelector )
        {
            Dictionary<TKey, TValue> r = new Dictionary<TKey, TValue>( enumerable.Count() );

            foreach (TEnumerable e in enumerable)
            {
                r.Add( keySelector( e ), valueSelector( e ) );
            }

            return r;
        }

        /// <summary>
        /// Returns the index of the first difference between two lists.
        /// </summary>
        public static int GetIndexOfFirstDifference<T>( IEnumerable<T> dest, IEnumerable<T> source )
        {
            // if its new
            // if anything before has been added
            // if anything before has been removed
            // if anything before has been changed
            var d = dest.GetEnumerator();
            var s = source.GetEnumerator();
            int i = 0;

            while (s.MoveNext())
            {
                if (!d.MoveNext() || !d.Current.Equals( s.Current ))
                {
                    return i;
                }

                i++;
            }

            return i;
        }

        /// <summary>
        /// Sorts the [values] based on [order].
        /// </summary>
        public static void Sort<TKey, TValue>( TKey[] keys, TValue[] values, Comparison<TKey> order )
        {
            Array.Sort( keys, values, new ComparisonComparer<TKey>( order ) );
        }

        /// <summary>
        /// (MJR) Merges two sequences into a tuple
        /// </summary>
        public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>( this IEnumerable<T1> a, IEnumerable<T2> b )
        {
            var x = a.GetEnumerator();
            var y = b.GetEnumerator();

            while (x.MoveNext() && y.MoveNext())
            {
                yield return new Tuple<T1, T2>( x.Current, y.Current );
            }
        }

        /// <summary>
        /// (MJR) Merges three sequences into a tuple
        /// </summary>
        public static IEnumerable<Tuple<T1, T2, T3>> Zip<T1, T2, T3>( this IEnumerable<T1> a, IEnumerable<T2> b, IEnumerable<T3> c )
        {
            var x = a.GetEnumerator();
            var y = b.GetEnumerator();
            var z = c.GetEnumerator();

            while (x.MoveNext() && y.MoveNext() && z.MoveNext())
            {
                yield return new Tuple<T1, T2, T3>( x.Current, y.Current, z.Current );
            }
        }

        /// <summary>
        /// (MJR) Returns the first item in the IEnumerable or null if the IEnumerable is empty.
        /// </summary>
        public static object FirstOrDefault2( this IEnumerable self )
        {
            var e = self.GetEnumerator();

            if (e.MoveNext())
            {
                return e.Current;
            }

            return null;
        }

        /// <summary>
        /// (MJR) Returns the first item in the IEnumerable or [default] if the IEnumerable is empty.
        /// </summary>
        public static T FirstOrDefault<T>( this IEnumerable<T> self, T @default )
        {
            if (self == null)
            {
                return @default;
            }

            IEnumerator<T> e = self.GetEnumerator();

            if (e.MoveNext())
            {
                return e.Current;
            }

            return @default;
        }

        /// <summary>
        /// Negates a boolean array.   
        /// </summary>              
        public static IEnumerable<bool> Negate( this IEnumerable<bool> array )
        {
            return array.Select( z => !z );
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// R equivalent of: array[...]
        /// </summary>             
        public static IEnumerable<T> At<T>( this IEnumerable<T> array1, IEnumerable<bool> array2 )
        {
            return At<T, bool>( array1, array2, λ => λ );
        }                    

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Yields elements of array where corrsponding elements in array2 are matched by the predicate
        /// R equivalent of: array[...]
        /// </summary>
        public static IEnumerable<T> At<T, U>( this IEnumerable<T> array1, IEnumerable<U> array2, Predicate<U> predicate )
        {
            var e1 = array1.GetEnumerator();
            var e2 = array2.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                if (predicate( e2.Current ))
                {
                    yield return e1.Current;
                }
            }
        }

        /// <summary>
        /// (MJR) Yields indices of array where the predicate is true (or -1 if not).
        /// </summary>
        public static int FirstIndexWhere<T>( this IEnumerable<T> array, Predicate<T> predicate )
        {
            int n = 0;

            foreach (T t in array)
            {
                if (predicate( t ))
                {
                    return n;
                }

                ++n;
            }

            return -1;
        }

        /// <summary>
        /// (MJR) Yields first and only element of array where the predicate is true (or throws an exception).
        /// </summary>
        public static T Only<T>( this IEnumerable<T> array, Predicate<T> predicate )
        {
            T result = default( T );
            bool foundResult = false;

            foreach (T t in array)
            {
                if (predicate( t ))
                {
                    if (foundResult)
                    {
                        throw new InvalidOperationException( "Multiple elements satisfy condition." );
                    }
                    else
                    {
                        result = t;
                        foundResult = true;
                    }
                }
            }

            if (!foundResult)
            {
                throw new InvalidOperationException( "No elements satisfy condition." );
            }

            return result;
        }

        /// <summary>
        /// (MJR) Yields the first index of the array where the item equals the passed value (returns -1 if not found).
        /// </summary>
        public static int IndexOf<T>( this IEnumerable<T> array, T item )
        {
            int n = 0;

            foreach (T t in array)
            {
                if (object.Equals( t, item ))
                {
                    return n;
                }

                ++n;
            }

            return -1;
        }

        /// <summary>
        /// (MJR) Yields indices of <paramref name="array"/> where the <paramref name="predicate"/> 
        /// is true and returns the results in the specified <paramref name="order"/>.
        /// </summary>
        public static IEnumerable<int> WhichInOrder<T>( this IReadOnlyList<T> array, Predicate<T> predicate, Comparison<T> order )
        {
            int[] which = Which( array, predicate ).ToArray();
            T[] vals = Extract<T>( array, which ).ToArray();

            ArrayHelper.Sort( vals, which, order );

            return which;
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Yields the indices of the <paramref name="array"/> in the order specified by
        /// <paramref name="order"/>.
        /// </summary>                                                      
        public static IEnumerable<int> WhichOrder<T>( this T[] array, Comparison<T> order )
        {
            int[] indices = array.Indices().ToArray();

            ArrayHelper.Sort( array, indices, order );

            return indices;
        }

        /// <summary>
        /// (MJR) Yields the indices of <paramref name="array"/> where the <paramref name="predicate"/>
        /// is true.
        /// </summary>
        public static IEnumerable<int> Which<T>( this IEnumerable<T> array, Predicate<T> predicate )
        {
            int n = 0;

            foreach (T t in array)
            {
                if (predicate( t ))
                {
                    yield return n;
                }

                ++n;
            }
        }      

        /// <summary>
        /// (MJR) (EXTENSION)
        /// This is a version of <see cref="At"/> which returns an array.
        /// Since the size of the array is known in advance this is slightly more optimal.
        /// </summary>                      
        /// <param name="array">Source array</param>
        /// <param name="which">Indices of elements to extract. Elements are returned in this order. Duplicates are permitted.</param>
        /// <returns>Subset or array</returns>
        public static T[] Extract<T>( this IReadOnlyList<T> array, int[] which )
        {
            T[] result = new T[which.Length];

            for (int i = 0; i < which.Length; i++)
            {
                result[i] = array[which[i]];
            }

            return result;
        }                 

        /// <summary>
        /// (MJR) Yields the elements of array which correspond to the indices of which.
        /// Indices may be specified in any order and duplicates are permitted.
        /// </summary>
        public static IEnumerable<T> At<T>( this T[] array, IEnumerable<int> which )
        {
            foreach (int i in which)
            {
                yield return array[i];
            }
        }

        /// <summary>
        /// (MJR) Yields the elements of array which correspond to the indices of which.
        /// Indices may be specified in any order and duplicates are permitted.
        /// </summary>
        public static IEnumerable<T> At<T>( this IReadOnlyList<T> array, IEnumerable<int> which )
        {
            foreach (int i in which)
            {
                yield return array[i];
            }
        }

        /// <summary>
        /// (MJR) Replaces the items in array with indices "indices" with the sequential values in "values".
        /// </summary>
        public static void ReplaceIn<T>( this T[] array, IEnumerable<int> indices, IEnumerable<T> values )
        {
            var e1 = indices.GetEnumerator();
            var e2 = values.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                array[e1.Current] = e2.Current;
            }
        }

        /// <summary>
        /// (MJR) Add or replace with a lock
        /// </summary>           
        public static void ThreadSafeIndex<T, U>( this Dictionary<T, U> self, T key, U value )
        {
            lock (self)
            {
                self[key] = value;
            }
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Flattens a jagged array.
        /// </summary>              
        public static T[,] Flatten<T>( this IReadOnlyList<IReadOnlyList< T>> jagged )
        {
            int numi = jagged.Count;                           
            int numj = jagged[0].Count;
            T[,] result = new T[numi, numj];

            for (int i = 0; i < numi; i++)
            {
                if (i != 0 && (jagged[i].Count != jagged[i - 1].Count))
                {
                    throw new InvalidOperationException( "Attempt to flatten a jagged array where all elements are not of equal length." );
                }

                for (int j = 0; j < numj; j++)
                {
                    result[i, j] = jagged[i][j];
                }
            }

            return result;
        }

        /// <summary>
        /// Wraps a <see cref="Comparison{T}"/> into an <see cref="IComparer{T}"/>.
        /// </summary>                      
        private class ComparisonComparer<T> : IComparer<T>
        {
            private Comparison<T> order;

            public ComparisonComparer( Comparison<T> order )
            {
                this.order = order;
            }

            public int Compare( T x, T y )
            {
                return order( x, y );
            }
        }

        /// <summary>
        /// See <see cref="Hash(IEnumerable)"/>
        /// </summary>                      
        public static int HashTogether<T>( params T[] en )
        {
            return Hash((IEnumerable<T>) en );
        }

        /// <summary>
        /// Hashes all the elements of an <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">The type of values to hash (required to avoid boxing of value types, which changes the hash)</typeparam>
        /// <param name="en">Sequence containing elements</param>
        /// <returns>The combined hash</returns>
        public static int Hash<T>( IEnumerable<T> en )
        {
            int r = 0;

            foreach (T x in en)
            {
                if (x != null)
                {
                    r = r.RotateLeft(1);
                    r = r ^ x.GetHashCode();
                }
            }

            return r;
        }

        /// <summary>
        /// SequenceEqual for untyped arrays.
        /// </summary>
        /// <param name="a">First array</param>
        /// <param name="b">Second array</param>
        /// <returns>If arrays are equal</returns>
        public static bool SequenceEqual( IEnumerable a, IEnumerable b )
        {
            if (a == null)
            {
                if (b == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (b == null)
            {
                return false;
            }   

            var enA = a.GetEnumerator();
            var enB = b.GetEnumerator();

            while (enA.MoveNext())
            {
                enB.MoveNext();

                if (!enA.Current.Equals( enB.Current ))
                {
                    return false;
                }
            }

            return true;
        }

        #region Obsolete wrappers 

        [Obsolete( "Use At" )]
        public static IEnumerable<T> Corresponding<T>( this IEnumerable<T> array1, IEnumerable<bool> array2 )
        {
            return At( array1, array2 );
        }

        [Obsolete( "Use At" )]
        public static IEnumerable<T> Corresponding<T, U>( this IEnumerable<T> array1, IEnumerable<U> array2, Predicate<U> predicate )
        {
            return At( array1, array2, predicate );
        }

        [Obsolete( "Use Extract" )]
        public static T[] In2<T>( this T[] array, int[] which )
        {
            return Extract( array, which );
        }      

        [Obsolete( "Use At" )]
        public static IEnumerable<T> In<T>( this T[] array, IEnumerable<int> which )
        {
            return At( array, which );
        }

        [Obsolete( "Use At" )]
        public static IEnumerable<T> In<T>( this IReadOnlyList<T> array, IEnumerable<int> which )
        {
            return At( array, which );
        }

        #endregion
    }
}
