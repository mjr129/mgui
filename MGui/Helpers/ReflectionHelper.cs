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

        public static bool IsOfGenericType( object o, Type t )
        {
            if (o == null)
            {
                return false;
            }

            Type ot = o.GetType();

            return ot.IsGenericType && ot.GetGenericTypeDefinition().IsAssignableFrom( t );
        }
    }
}
