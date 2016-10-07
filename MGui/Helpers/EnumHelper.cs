using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// (MJR) (EXTENSION)
        /// Returns if the enum contains all of the specified flags.
        /// </summary>         
        public static bool Has<T>( this T self, T flag )
            where T : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {
            uint selfI = Convert.ToUInt32( self );
            uint flagI = Convert.ToUInt32( flag );
            return (selfI & flagI) == flagI;
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Converts an enum to a string accounting for NameAttribute attributes.
        /// </summary>
        public static string ToUiString( this Enum @enum )
        {
            string name = @enum.ToString();
            FieldInfo fieldInfo = @enum.GetType().GetField( name );

            if (fieldInfo != null) // i.e. if not a valid enum such as "-1"
            {
                NameAttribute nameAttr = (NameAttribute)fieldInfo.GetCustomAttribute<NameAttribute>();

                if (nameAttr != null)
                {
                    return nameAttr.Name;
                }
            }

            return name;
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Converts an enum to a string accounting for DescriptionAttribute attributes.
        /// </summary>
        public static string ToDescription( Enum @enum )
        {
            string name = @enum.ToString();
            FieldInfo fieldInfo = @enum.GetType().GetField( name );

            if (fieldInfo != null) // i.e. if not a valid enum such as "-1"
            {
                DescriptionAttribute nameAttr = (DescriptionAttribute)fieldInfo.GetCustomAttribute<DescriptionAttribute>();

                if (nameAttr != null)
                {
                    return nameAttr.Description;
                }
            }

            return null;
        }

        /// <summary>
        /// Splits an enum into its constituent flags.
        /// </summary>
        public static IEnumerable<T> SplitEnum<T>( T stats )
            where T : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {
            int e = (int)(object)stats;

            for (int x = 1; x != 0; x = x << 1)
            {
                if ((e & x) == x)
                {
                    yield return (T)(object)x;
                }
            }
        }

        /// <summary>
        /// Combines an enums constituent flags into a single enum.
        /// </summary>
        public static T SumEnum<T>( IEnumerable<T> enumerable )
            where T : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {
            if (enumerable == null)
            {
                return (T)Enum.ToObject( typeof(T), 0 );
            }

            int flags = 0;

            foreach (T t in enumerable)
            {
                flags |= (int)(object)t;
            }

            return (T)(object)flags;
        }



        /// <summary>
        /// Returns the constituent flags of an enum.
        /// </summary>
        public static IEnumerable<T> GetEnumFlags<T>()
            where T : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {
            HashSet<int> vals = Enum.GetValues( typeof(T) ).Cast<int>().Unique();

            foreach (int i in vals)
            {
                if (i != 0 && (i & (i - 1)) == 0)
                {
                    yield return (T)(object)i;
                }
            }
        }

        /// <summary>
        /// Returns the constituent values of an enum.
        /// </summary>
        public static T[] GetEnumValues<T>()
            where T : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {
            return (T[])Enum.GetValues( typeof(T) );
        }

        /// <summary>
        /// Returns the names of an enum as a dictionary.
        /// Includes C# names and the UI strings (if present).
        /// </summary>
        public static Dictionary<string, T> GetEnumKeys<T>()
            where T : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {
            Dictionary<string, T> res = new Dictionary<string, T>();

            foreach (Enum val in Enum.GetValues( typeof(T) ))
            {
                T t = (T)(object)val;
                res[val.ToString().ToUpper()] = t;
                res[val.ToUiString().ToUpper()] = t;
            }

            return res;
        }

        /// <summary>
        /// Uses an enum members' Name attribute to parse an enum.
        /// </summary>                                            
        public static T Parse<T>( string value, bool ignoreCase = true )
            where T : struct, IComparable, IFormattable, IConvertible // aka. Enum
        {                 
            foreach (Enum t in Enum.GetValues(typeof( T ) ))
            {
                string name = ToUiString( t );

                if (string.Compare( value, name, ignoreCase ) == 0)
                {
                    return (T)(object)t;
                }
            }

            throw new InvalidOperationException( $"The string \"{value}\" cannot be converted into the enum type \"{typeof(T).Name}\". The available options are: \"{string.Join( "\", \"", GetEnumValues<T>() )}\"." );
        }
    }

    /// <summary>
    /// Name attributes, for giving names to enum members.
    /// ([ComponentModel.DisplayNameAttribute] unfortunately doesn't work on enums)
    /// </summary>
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }

        public NameAttribute( string name )
        {
            Name = name;
        }
    }
}
