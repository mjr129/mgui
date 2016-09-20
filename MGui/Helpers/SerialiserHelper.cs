using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using MGui.Datatypes;

namespace MGui.Helpers
{
    public static class SerialiserHelper
    {
        public static void Serialise<T>( string fileName, T value )
        {
            Serialise<T>( fileName, ESerialisationFlags._Default, value );
        }

        public static void Serialise<T>(string fileName, ESerialisationFlags format, T value )
        {
            fileName = GetFileName( fileName );

            using (FileStream fs = new FileStream( fileName, FileMode.Create ))
            {
                Serialise<T>( fs, format, value );
            }
        }

        public static void Serialise<T>( Stream fs, T value )
        {
            Serialise<T>( fs, ESerialisationFlags._Default, value );
        }

        public static void Serialise<T>( Stream fs, ESerialisationFlags format, T value )
        {
            Stream fsu;

            switch (format & ESerialisationFlags._CompressMask)
            {
                case ESerialisationFlags.GZip: 
                    fsu = new GZipStream( fs, CompressionMode.Compress );
                    break;

                case ESerialisationFlags.NoCompression:
                    fsu = fs;
                    break;                                                                                       

                default:
                    throw new SwitchException( "format", format );
            }

            try
            {
                switch (format & ESerialisationFlags._SerialiserMask)
                {
                    case ESerialisationFlags.Binary:      
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize( fsu, value );
                        break;

                    case ESerialisationFlags.Xml:
                        XmlSerializer xs = new XmlSerializer( typeof( T ) );
                        xs.Serialize( fsu, value );
                        break;

                    case ESerialisationFlags.Ini:
                        IniSerialiser.Serialise<T>( fsu, value );
                        break;                                                                                         

                    default:
                        throw new SwitchException( "format", format );
                }
            }
            finally
            {
                switch (format & ESerialisationFlags._CompressMask)
                {   
                    case ESerialisationFlags.GZip:   
                        fsu.Dispose();
                        break;

                    case ESerialisationFlags.NoCompression:
                        break;

                    default:
                        throw new SwitchException( format );
                }
            }
        }    

        public static T DeserialiseOrDefault<T>( string fileName )
        {
            return DeserialiseOrDefault<T>( fileName, ESerialisationFlags._Default, (Func<T>)null );
        }

        public static T DeserialiseOrDefault<T>( string fileName, ESerialisationFlags format )
        {
            return DeserialiseOrDefault<T>( fileName, format, (Func<T>)null );
        }

        public static T DeserialiseOrDefault<T>( string fileName, ESerialisationFlags format, T defaultValue )
        {
            return DeserialiseOrDefault<T>( fileName, format, () => defaultValue );
        }

        public static T DeserialiseOrDefault<T>( string fileName, ESerialisationFlags format, Func<T> defaultProvider )
        {
            if (File.Exists( fileName ))
            {
                try
                {
                    return Deserialise<T>( fileName, format );
                }
                catch
                {
                    // Ignore
                }
            }

            if (defaultProvider == null)
            {
                return default( T );
            }
            else
            {
                return defaultProvider();
            }
        }

        public static T Deserialise<T>( string fileName, ESerialisationFlags format )
        {
            fileName = GetFileName( fileName );

            if (!File.Exists( fileName ))
            {
                throw new FileNotFoundException( "Deserialise file not found.", fileName );
            }
                                                            
            using (FileStream fs = new FileStream( fileName, FileMode.Open ))
            {
                return Deserialise<T>( fs, format );
            }
        }

        private static string GetFileName( string fileName )
        {
            if (!fileName.Contains( "\\" ))
            {
                return Path.Combine( Application.StartupPath, fileName );
            }

            return fileName;
        }

        public static T Deserialise<T>( FileStream fs, ESerialisationFlags format )
        {
            Stream fsu;

            switch (format & ESerialisationFlags._CompressMask)
            {
                case ESerialisationFlags.GZip:
                    fsu = new GZipStream( fs, CompressionMode.Decompress );
                    break;

                case ESerialisationFlags.NoCompression:
                    fsu = fs;
                    break;                                                                                          

                default:
                    throw new SwitchException( format );
            }

            try
            {
                switch (format & ESerialisationFlags._SerialiserMask)
                {
                    case ESerialisationFlags.Binary:
                        BinaryFormatter bf = new BinaryFormatter();
                        return (T)bf.Deserialize( fsu );

                    case ESerialisationFlags.Xml:
                        XmlSerializer xs = new XmlSerializer(typeof(T));
                        return (T)xs.Deserialize( fsu );

                    case ESerialisationFlags.Ini:
                        return IniSerialiser.Deserialise<T>( fsu );                                                  

                    default:
                        throw new SwitchException( "format", format );
                }
            }
            finally
            {
                switch (format & ESerialisationFlags._CompressMask)
                {
                    case ESerialisationFlags.GZip:
                        fsu.Dispose();
                        break;

                    case ESerialisationFlags.NoCompression:
                        break;

                    default:
                        throw new SwitchException( format );
                }
            }
        }
    }

    [Flags]
    public enum ESerialisationFlags
    {                 
        Binary = 0,
        Xml = 1,
        Ini = 2,

        NoCompression = 0 << 16,
        GZip = 1 << 16,


        _Default = Binary | GZip,
        _SerialiserMask = unchecked((int)0x0000FFFF),
        _CompressMask = unchecked((int)0xFFFF0000),
    };
}
