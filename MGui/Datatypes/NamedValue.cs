using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    public class NamedValue<T>
    {
        private readonly string _name;
        private readonly T _value;

        public NamedValue( string name, T value )
        {
            if (name == null)
            {
                throw new ArgumentNullException( "name" );
            }

            _name = name;
            _value = value;
        }

        public T Value => _value;
        public string Name => _name;

        public override string ToString()
        {
            return _name;
        }

        public override int GetHashCode()
        {
            if (_value == null)
            {
                return 0;
            }

            return _value.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            if (obj == null)
            {
                return _value == null;
            }

            NamedValue<T> asNamedValue = obj as NamedValue<T>;

            if (asNamedValue == null)
            {
                return obj.Equals( _value );
            }

            if (asNamedValue.Value == null)
            {
                return _value == null;
            }

            return asNamedValue.Value.Equals( _value );
        }

        public static NamedValue<T> AsNamedValue( T x )
        {
            if (x == null)
            {
                return new NamedValue<T>( "(𝘯𝘶𝘭𝘭 𝘷𝘢𝘭𝘶𝘦)", x );
            }

            string s = x.ToString();

            if (s == null)
            {
                return new NamedValue<T>( "(𝘯𝘶𝘭𝘭 𝘵𝘦𝘹𝘵)", x );
            }
            else
            {
                return new NamedValue<T>( s, x );
            }
        }
    }
}
