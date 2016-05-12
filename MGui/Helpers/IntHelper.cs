using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
{
    public static class IntHelper
    {
        /// <summary>
        /// (MJR) (EXTENSION)
        /// Rotates the bits to the left.
        /// </summary>                   
        public static uint RotateLeft( this uint number, int numberOfBits )
        {
            return (number << numberOfBits) | (number >> (32 - numberOfBits));
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Rotates the bits to the left.
        /// </summary>                   
        public static int RotateLeft( this int number, int numberOfBits )
        {
            return unchecked((int)RotateLeft( (uint)number, numberOfBits ));
        }
    }
}
