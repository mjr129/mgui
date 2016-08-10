using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MGui.Datatypes;

namespace MSerialisers
{
    public static class IniSerialiser
    {
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

        public static T Deserialise<T>( Stream stream )
        {
            T r = Construct<T>();

            using (StreamReader sr = new StreamReader( stream ))
            {
                while (!sr.EndOfStream)
                {
                    string l = sr.ReadLine();

                    string[] e = l.Split( new[] { '=' }, 2 );

                    FieldInfo field = typeof( T ).GetField( e[0] );

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

        private static T Construct<T>()
        {       
            var ctor = typeof( T ).GetConstructor( new Type[0] );

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
