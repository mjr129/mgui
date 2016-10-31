using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MGui.Helpers;

namespace MGui.Datatypes
{
    /// <summary>
    /// Serialises objects to and from INI-type files.
    /// </summary>
    public static class IniSerialiser
    {
        /// <summary>
        /// Serialises the fields of <paramref name="data"/> to the <paramref name="stream"/>.
        /// Only the fields in <typeparamref name="T"/> will be considered, even if the object is of a more derived type.
        /// </summary>                 
        public static void Serialise<T>( Stream stream, T data )
        {
            using (StreamWriter sw = new StreamWriter( stream ))
            {                     
                foreach (FieldInfo field in typeof( T ).GetFields())
                {
                    sw.Write( field.Name );
                    sw.Write( "=" );
                    sw.Write( ObjectToString( field.GetValue( data )) );
                    sw.WriteLine();
                }
            }
        }

        [Flags]
        public enum EFlags
        {
            None = 0,

            /// <summary>
            /// Normally the deserialiser will skip over any unknown fields (i.e. "Foo = Bar" where there is no such field as "Foo" in <typeparamref name="T"/>).
            /// This flag causes an error in these circumstances instead.
            /// </summary>
            ErrorOnUnknownField = 1,
        }
            
        /// <summary>
        /// Deserialises an object from <paramref name="stream"/>.
        /// The type will be constructed using <see cref="ReflectionHelper.Construct{T}"/>
        /// </summary>
        public static T Deserialise<T>( Stream stream, EFlags flags = EFlags.None )
        {
            T r = ReflectionHelper.Construct<T>();

            using (StreamReader sr = new StreamReader( stream ))
            {
                while (!sr.EndOfStream)
                {
                    string l = sr.ReadLine();

                    string[] e = l.Split( new[] { '=' }, 2 );

                    FieldInfo field = typeof( T ).GetField( e[0] );

                    if (field == null)
                    {
                        if (flags.Has( EFlags.ErrorOnUnknownField ))
                        {
                            throw new FormatException( $"The INI deserialiser encountered a field {{{e[0]}}} but that field does not exist in the type {{{typeof(T).ToUiString()}}}. The file may have been saved using an older version of the software." );
                        }
                        else
                        {
                            continue;
                        }
                    }

                    object x = StringToObject( e[1], field.FieldType );

                    field.SetValue( r, x );
                }
            }

            return r;
        }

        private static string ObjectToString(object x)
        {
            if (x is string[])
            {           
                return SpreadsheetReader.Default.WriteFields((string[])x );
            }

            return Convert.ToString( x );
        }                        

        private static object StringToObject( string x, Type t )
        {
            if (t == typeof( string[] ))
            {
                return SpreadsheetReader.Default.ReadFields( x );
            }

            return Convert.ChangeType( x, t );
        }
    }
}
