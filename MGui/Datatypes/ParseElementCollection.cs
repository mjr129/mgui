using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MGui.Helpers;

namespace MGui.Datatypes
{
    /// <summary>
    /// A TypeConverter for [ParseElementCollection].
    /// Used by the "options" PropertyGrid to provide editing for [ParseElementCollection].
    /// (All this verbosity really does is converts to and from a string).
    /// </summary>
    public class ParseElementCollectionConverter : TypeConverter
    {
        public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
        {
            if (typeof( IConvertible ).IsAssignableFrom( destinationType ))
            {
                return true;
            }

            return base.CanConvertTo( context, destinationType );
        }

        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            if (typeof( IConvertible ).IsAssignableFrom( sourceType))
            {
                return true;
            }

            return base.CanConvertFrom( context, sourceType );
        }

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            return new ParseElementCollection(  value.ToString() );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            if (typeof( IConvertible ).IsAssignableFrom( destinationType ))
            {
                return Convert.ChangeType( ((ParseElementCollection)value).ToStringSafe(), destinationType );
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }
    }

    /// <summary>
    /// Collection of 'ParseElement's. 
    /// </summary>
    [Serializable]
    [TypeConverter( typeof( ParseElementCollectionConverter ) )]
    public class ParseElementCollection
    {  
        private readonly List<ParseElement> _contents;

        public IReadOnlyList<ParseElement> Contents => _contents;

        /// <summary>
        /// Parses a string like
        /// abc{xyz}abc{xyz}
        /// </summary>
        public ParseElementCollection( string x )
        {
            _contents = new List<ParseElement>();
            StringBuilder sb = new StringBuilder();
            bool isOpen = false;

            // Could be more efficient but it suitable for purpose
            foreach (char c in x)
            {
                if (!isOpen && c == '{')
                {
                    if (sb.Length != 0)
                    {
                        _contents.Add( new ParseElement( sb.ToString(), false ) );
                    }
                    sb.Clear();
                    isOpen = true;
                }
                else if (isOpen && c == '}')
                {
                    _contents.Add( new ParseElement( sb.ToString(), true ) );
                    sb.Clear();
                    isOpen = false;
                }
                else
                {
                    sb.Append( c );
                }
            }

            if (sb.Length != 0)
            {
                if (isOpen)
                {
                    throw new InvalidOperationException( "Open bracket with content \"" + sb.ToString() + "\"." );
                }

                _contents.Add( new ParseElement( sb.ToString(), isOpen ) );
            }                                                                    
        }

        public override string ToString()
        {
            return string.Join( string.Empty, Contents );
        }

        public static bool IsNullOrEmpty( ParseElementCollection collection )
        {
            return (collection == null || collection.Contents.Count == 0
                    || (collection.Contents.Count == 1 && string.IsNullOrWhiteSpace( collection.Contents[0].Value )));
        }      
    }

    /// <summary>
    /// A string and whether it is in brackets.
    /// </summary>
    [Serializable]
    public class ParseElement
    {
        public readonly string Value;
        public readonly bool IsInBrackets;

        public ParseElement( string value, bool isInBrackets )
        {
            this.Value = value;
            this.IsInBrackets = isInBrackets && this.Value != "{";
        }

        public override string ToString()
        {
            if (IsInBrackets)
            {
                return "{" + Value + "}";
            }
            else
            {
                return Value;
            }
        }
    }
}
