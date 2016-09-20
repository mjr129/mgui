using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
{
    public static class ReflectionHelper
    {
        public static object CreateGeneric( Type baseType, Type parameter1 )
        {
            Type type = baseType.MakeGenericType( parameter1 );
            return Activator.CreateInstance( type );
        }
    }
}
