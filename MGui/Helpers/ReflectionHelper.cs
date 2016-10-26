﻿using System;
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

        public static bool IsOfGenericType( object result, Type type )
        {
			if (result == null)
            {
                return false;
            }

            Type t = result.GetType();

            return t.IsGenericType && type.IsAssignableFrom( t.GetGenericTypeDefinition() );
        }


        public static object GetDefault( Type dataType )
        {
            if (dataType.IsValueType)
            {
                return Activator.CreateInstance( dataType );
            }
            else
            {
                return null;
            }
        }
    }
}
