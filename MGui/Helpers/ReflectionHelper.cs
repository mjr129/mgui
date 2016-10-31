using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
{
    /// <summary>
    /// Utility functions involving reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Creates an instance of a generic type using the default constructor.
        /// </summary>                            
        public static object CreateGeneric( Type baseType, Type parameter1 )
        {
            Type type = baseType.MakeGenericType( parameter1 );
            return Activator.CreateInstance( type );
        }

        /// <summary>
        /// Returns if <paramref name="type"/> is a generic type.
        /// </summary>                                      
        public static bool IsOfGenericType( object result, Type type )
        {
			if (result == null)
            {
                return false;
            }

            Type t = result.GetType();

            return t.IsGenericType && type.IsAssignableFrom( t.GetGenericTypeDefinition() );
        }


        /// <summary>
        /// This is a run-time version of <c>default(T)</c>.
        /// </summary>                                      
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

        /// <summary>
        /// Constructs an object using the default constructor.
        /// If that is not available <see cref="FormatterServices.GetUninitializedObject"/> is used.
        /// </summary>                                                                               
        /// <remarks>
        /// This is really only intended for deserialisation, where all fields will be filled out, while at the same time
        /// allowing optional default values via the default constructor.
        /// </remarks>
        public static T Construct<T>()
        {
            ConstructorInfo ctor = typeof( T ).GetConstructor( new Type[0] );

            if (ctor != null)
            {
                return (T)ctor.Invoke( new object[0] );
            }
            else
            {
                return (T)FormatterServices.GetUninitializedObject( typeof( T ) );
            }
        }
    }
}
