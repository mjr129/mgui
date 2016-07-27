using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MGui.Datatypes;

namespace MGui.Helpers
{
    public static class SerialiserHelper
    {
        public static void Serialise<T>( string fileName, T value )
        {
            Serialise<T>( fileName, ESerialisationFlags.Default, value );
        }

        public static void Serialise<T>(string fileName, ESerialisationFlags format, T value )
        {
            using (FileStream fs = new FileStream( fileName, FileMode.Create ))
            {
                Serialise<T>( fs, format, value );
            }
        }

        public static void Serialise<T>( Stream fs, T value )
        {
            Serialise<T>( fs, ESerialisationFlags.Default, value );
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
                    throw new SwitchException( format );
            }

            try
            {
                switch (format & ESerialisationFlags._SerialiserMask)
                {
                    case ESerialisationFlags.Binary:
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize( fsu, value );
                        break;

                    default:
                        throw new SwitchException( format );
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
            return DeserialiseOrDefault<T>( fileName, ESerialisationFlags.Binary, (Func<T>)null );
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
            if (!File.Exists( fileName ))
            {
                throw new FileNotFoundException( "Deserialise file not found.", fileName );
            }
                                                            
            using (FileStream fs = new FileStream( fileName, FileMode.Open ))
            {
                return Deserialise<T>( fs, format );
            }
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

                    default:
                        throw new SwitchException( format );
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
        GZip = 0,
        NoCompression = 1,

        Default = Binary | GZip,
        _SerialiserMask = Binary,
        _CompressMask = GZip,
    };
}
