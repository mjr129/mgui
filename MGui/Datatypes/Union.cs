using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    /// <summary>
    /// An implementation of C++ union, without the space saving.
    /// </summary>                     
    public sealed class Either<T1, T2>
    {
        byte _item;
        T1 _item1;
        T2 _item2;

        public bool HasItem1 => _item == 1;
        public bool HasItem2 => _item == 2;
        public T1 Item1 => _item1;
        public T2 Item2 => _item2;

        public Either( T1 t )
        {
            _item1 = t;
            _item = 1;
        }

        public Either( T2 u )
        {
            _item2 = u;
            _item = 2;
        }

        // Using IMPLICIT operators because typing generics is overly verbose
        // Note that if T1 or T2 are INTERFACES or OBJECTS these operators will be unavailable

        public static implicit operator Either<T1, T2>( T1 item1 )
        {
            return new Either<T1, T2>( item1 );
        }

        public static implicit operator Either<T1, T2>( T2 item2 )
        {
            return new Either<T1, T2>( item2 );
        }  
    }
}
