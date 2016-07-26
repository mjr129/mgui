using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    public class Map<T1, T2>
    {
        private readonly Dictionary<T1, T2> _t1 = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _t2 = new Dictionary<T2, T1>();

        public void Add( T1 t1, T2 t2 )
        {
            Add1( t1, t2 );
        }

        public void Add( T2 t2, T1 t1 )
        {
            Add2( t2, t1 );
        }

        public void Add1( T1 t1, T2 t2 )
        {
            _t1.Add( t1, t2 );
            _t2.Add( t2, t1 );
        }

        public void Add2( T2 t2, T1 t1 )
        {
            _t1.Add( t1, t2 );
            _t2.Add( t2, t1 );
        }

        public T1 this[ T2 key ]
        {
            get { return Get2( key ); }
            set { Set2( key, value ); }
        }

        public T2 this[ T1 key ]
        {
            get { return Get1( key ); }
            set { Set1( key, value ); }
        }

        public T1 Get2( T2 key )
        {
            return _t2[key];
        }

        public void Set2( T2 key, T1 value )
        {
            _t2[key] = value;
            _t1[value] = key;
        }

        public T2 Get1( T1 key )
        {
            return _t1[key];
        }

        public void Set1( T1 key, T2 value )
        {
            _t1[key] = value;
            _t2[value] = key;
        }

        public void Clear()
        {
            _t1.Clear();
            _t2.Clear();
        }

        public bool Contains( T1 t1 )
        {
            return Contains1( t1 );
        }

        public bool Contains( T2 t2 )
        {
            return Contains2( t2 );
        }

        public bool Contains1( T1 t1 )
        {
            return _t1.ContainsKey( t1 );
        }

        public bool Contains2( T2 t2 )
        {
            return _t2.ContainsKey( t2 );
        }

        public void AddRange( IEnumerable<KeyValuePair <T1, T2>> values )
        {
            AddRange1( values );
        }

        public void AddRange( IEnumerable<KeyValuePair<T2, T1>> values )
        {
            AddRange2( values );
        }

        public void AddRange1( IEnumerable<KeyValuePair<T1, T2>> values )
        {
            foreach (var val in values)
            {
                Add1( val.Key, val.Value );
            }
        }

        public void AddRange2( IEnumerable<KeyValuePair<T2, T1>> values )
        {
            foreach (var val in values)
            {
                Add2( val.Key, val.Value );
            }
        }

        public IEnumerator<T1> GetEnumerator1()
        {
            return _t1.Keys.GetEnumerator();
        }

        public IEnumerator<T2> GetEnumerator2()
        {
            return _t2.Keys.GetEnumerator();
        }

        public bool TryGetValue( T1 t1, out T2 t2 )
        {
            return TryGetValue1( t1, out t2 );
        }

        public bool TryGetValue( T2 t2, out T1 t1 )
        {
            return TryGetValue2( t2, out t1 );
        }

        public bool TryGetValue1( T1 t1, out T2 t2 )
        {
            return _t1.TryGetValue( t1, out t2 );
        }

        public bool TryGetValue2( T2 t2, out T1 t1 )
        {
            return _t2.TryGetValue( t2, out t1 );
        }
    }
}
