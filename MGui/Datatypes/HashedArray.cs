using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGui.Helpers;

namespace MGui.Datatypes
{
    /// <summary>
    /// Maintains an array with an associated hash of ALL its members.
    /// The array should be not be changed once wrapped.
    /// </summary>
    /// <typeparam name="T">Type of enumerable</typeparam>
    public class HashedEnumerable<T, U>
        where T: ICollection, IEnumerable<U>
    {           
        public readonly T Array;
        public readonly int HashCode;

        public HashedEnumerable( T array )
        {              
            Array = array;
            HashCode = ArrayHelper.Hash<U>( Array );
        }

        public static implicit operator HashedEnumerable<T, U>( T array )
        {
            return new HashedEnumerable<T, U>( array );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as HashedEnumerable<T, U> );
        }

        public bool Equals( HashedEnumerable<T, U> obj )
        {
            if (obj == null)
            {
                return false;
            }

            return HashCode == obj.HashCode
                && Array.Count == obj.Array.Count
                && ArrayHelper.SequenceEqual( Array, obj.Array );
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public override string ToString()
        {
            return HashCode.ToString( "X" ) + ": " + Array.Count + " items";
        }
    }
}
