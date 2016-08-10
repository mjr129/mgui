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

            NamedValue<T> nv = obj as NamedValue<T>;

            if (nv != null)
            {
                if (nv.Value == null)
                {
                    return _value == null;
                }

                return nv.Value.Equals( _value );
            }

            return obj.Equals( _value );
        }
    }
}
