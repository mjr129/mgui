﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MGui.Datatypes;

namespace MGui.Helpers
{
    /// <summary>
    /// Helper functions for strings.
    /// </summary>
    public static class StringHelper
    {
        // UTF alphabets 0 1 2 3 4 5 6 7 8 
        // 0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567
        public static string UtNormal = @"0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz";
        public static string UtBold = @"𝟎𝟏𝟐𝟑𝟒𝟓𝟔𝟕𝟖𝟗 𝐀𝐁𝐂𝐃𝐄𝐅𝐆𝐇𝐈𝐉𝐊𝐋𝐌𝐍𝐎𝐏𝐐𝐑𝐒𝐓𝐔𝐕𝐖𝐗𝐘𝐙 𝐚𝐛𝐜𝐝𝐞𝐟𝐠𝐡𝐢𝐣𝐤𝐥𝐦𝐧𝐨𝐩𝐪𝐫𝐬𝐭𝐮𝐯𝐰𝐱𝐲𝐳";
        public static string UtFraktur = @"0123456789 𝕬𝕭𝕮𝕯𝕰𝕱𝕲𝕳𝕴𝕵𝕶𝕷𝕸𝕹𝕺𝕻𝕼𝕽𝕾𝕿𝖀𝖁𝖂𝖃𝖄𝖅 𝖆𝖇𝖈𝖉𝖊𝖋𝖌𝖍𝖎𝖏𝖐𝖑𝖒𝖓𝖔𝖕𝖖𝖗𝖘𝖙𝖚𝖛𝖜𝖝𝖞𝖟";
        public static string UtBoldItalic = @"0123456789 𝑨𝑩𝑪𝑫𝑬𝑭𝑮𝑯𝑰𝑱𝑲𝑳𝑴𝑵𝑶𝑷𝑸𝑹𝑺𝑻𝑼𝑽𝑾𝑿𝒀𝒁 𝒂𝒃𝒄𝒅𝒆𝒇𝒈𝒉𝒊𝒋𝒌𝒍𝒎𝒏𝒐𝒑𝒒𝒓𝒔𝒕𝒖𝒗𝒘𝒙𝒚𝒛";
        public static string UtFrakturItalic = @"0123456789 𝓐𝓑𝓒𝓓𝓔𝓕𝓖𝓗𝓘𝓙𝓚𝓛𝓜𝓝𝓞𝓟𝓠𝓡𝓢𝓣𝓤𝓥𝓦𝓧𝓨𝓩 𝓪𝓫𝓬𝓭𝓮𝓯𝓰𝓱𝓲𝓳𝓴𝓵𝓶𝓷𝓸𝓹𝓺𝓻𝓼𝓽𝓾𝓿𝔀𝔁𝔂𝔃";
        public static string UtDouble = @"𝟘𝟙𝟚𝟛𝟜𝟝𝟞𝟟𝟠𝟡 𝔸𝔹ℂ𝔻𝔼𝔽𝔾ℍ𝕀𝕁𝕂𝕃𝕄ℕ𝕆ℙℚℝ𝕊𝕋𝕌𝕍𝕎𝕏𝕐ℤ 𝕒𝕓𝕔𝕕𝕖𝕗𝕘𝕙𝕚𝕛𝕜𝕝𝕞𝕟𝕠𝕡𝕢𝕣𝕤𝕥𝕦𝕧𝕨𝕩𝕪𝕫";
        public static string UtMono = @"𝟶𝟷𝟸𝟹𝟺𝟻𝟼𝟽𝟾𝟿 𝙰𝙱𝙲𝙳𝙴𝙵𝙶𝙷𝙸𝙹𝙺𝙻𝙼𝙽𝙾𝙿𝚀𝚁𝚂𝚃𝚄𝚅𝚆𝚇𝚈𝚉 𝚊𝚋𝚌𝚍𝚎𝚏𝚐𝚑𝚒𝚓𝚔𝚕𝚖𝚗𝚘𝚙𝚚𝚛𝚜𝚝𝚞𝚟𝚠𝚡𝚢𝚣";
        public static string UtSans = @"𝟢𝟣𝟤𝟥𝟦𝟧𝟨𝟩𝟪𝟫 𝖠𝖡𝖢𝖣𝖤𝖥𝖦𝖧𝖨𝖩𝖪𝖫𝖬𝖭𝖮𝖯𝖰𝖱𝖲𝖳𝖴𝖵𝖶𝖷𝖸𝖹 𝖺𝖻𝖼𝖽𝖾𝖿𝗀𝗁𝗂𝗃𝗄𝗅𝗆𝗇𝗈𝗉𝗊𝗋𝗌𝗍𝗎𝗏𝗐𝗑𝗒𝗓";
        public static string UtSansBold = @"𝟬𝟭𝟮𝟯𝟰𝟱𝟲𝟳𝟴𝟵 𝗔𝗕𝗖𝗗𝗘𝗙𝗚𝗛𝗜𝗝𝗞𝗟𝗠𝗡𝗢𝗣𝗤𝗥𝗦𝗧𝗨𝗩𝗪𝗫𝗬𝗭 𝗮𝗯𝗰𝗱𝗲𝗳𝗴𝗵𝗶𝗷𝗸𝗹𝗺𝗻𝗼𝗽𝗾𝗿𝘀𝘁𝘂𝘃𝘄𝘅𝘆𝘇";
        public static string UtSansBoldItalic = @"0123456789 𝘼𝘽𝘾𝘿𝙀𝙁𝙂𝙃𝙄𝙅𝙆𝙇𝙈𝙉𝙊𝙋𝙌𝙍𝙎𝙏𝙐𝙑𝙒𝙓𝙔𝙕 𝙖𝙗𝙘𝙙𝙚𝙛𝙜𝙝𝙞𝙟𝙠𝙡𝙢𝙣𝙤𝙥𝙦𝙧𝙨𝙩𝙪𝙫𝙬𝙭𝙮𝙯";
        public static string UtCircled = @"0⑴⑵⑶⑷⑸⑹⑺⑻⑼ ⒜⒝⒞⒟⒠⒡⒢⒣⒤⒥⒦⒧⒨⒩⒪⒫⒬⒭⒮⒯⒰⒱⒲⒳⒴⒵ ⒜⒝⒞⒟⒠⒡⒢⒣⒤⒥⒦⒧⒨⒩⒪⒫⒬⒭⒮⒯⒰⒱⒲⒳⒴⒵";
        public static string UtSmallCaps = @"0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZ ᴀʙᴄᴅᴇғɢʜɪᴊᴋʟᴍɴᴏᴩǫʀsᴛᴜᴠᴡxʏᴢ";
        public static string UtSuper = @"⁰¹²³⁴⁵⁶⁷⁸⁹ ᴬᴮᶜᴰᴱᶠᴳᴴᴵᴶᴷᴸᴹᴺᴼᴾQᴿˢᵀᵁⱽᵂˣʸᶻ ᵃᵇᶜᵈᵉᶠᵍʰⁱʲᵏˡᵐⁿᵒᵖqʳˢᵗᵘᵛʷˣʸᶻ";
        public static string UtSubscript = @"₀₁₂₃₄₅₆₇₈₉ ₐBCDₑFGₕᵢⱼₖₗₘₙₒₚQᵣₛₜᵤᵥWₓYZ ₐbcdₑfgₕᵢⱼₖₗₘₙₒₚqᵣₛₜᵤᵥwₓyz";


        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        private static string ToUtf( this string x, string style )
        {
            if (x == null)
            {
                return null;
            }

            StringInfo si = new StringInfo( style );
            StringBuilder sb = new StringBuilder();

            for (int n = 0; n < x.Length; n++)
            {
                char c = x[n];

                if (c >= '0' && c < '9')
                {
                    string nc = si.SubstringByTextElements( c - '0', 1 );
                    sb.Append( nc );
                }
                else if (c >= 'A' && c < 'Z')
                {
                    string nc = si.SubstringByTextElements( c - 'A' + 11, 1 );
                    sb.Append( nc );
                }
                else if (c >= 'a' && c < 'z')
                {
                    string nc = si.SubstringByTextElements( c - 'a' + 38, 1 );
                    sb.Append( nc );
                }
                else
                {
                    sb.Append( c );
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        public static string ToBold( this string x )
        {
            return ToUtf( x, UtBold );
        }

        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        public static string ToSans( this string x )
        {
            return ToUtf( x, UtSans );
        }

        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        public static string ToSansBold( this string x )
        {
            return ToUtf( x, UtSansBold );
        }

        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        public static string ToSansBoldItalic( this string x )
        {
            return ToUtf( x, UtSansBoldItalic );
        }

        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        public static string ToSmallCaps( this string x )
        {
            return ToUtf( x, UtSmallCaps );
        }

        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        public static string ToSubScript( int value )
        {
            return value.ToString().ToUtf( UtSubscript );
        }

        /// <summary>
        /// (EXTENSION) (MJR) UTF text conversion
        /// </summary>
        public static string Circle( int value )
        {
            return value.ToString().ToUtf( UtCircled );
        }

        /// <summary>
        /// (EXTENSION) (MJR) Returns the object as a string, or an empty string if the object is null
        /// </summary>                                                                                
        public static string ToStringSafe<T>( this T self )
            where T : class
        {
            if (self == null)
            {
                return string.Empty;
            }

            return self.ToString();
        }

        /// <summary>
        /// (MJR) Like PadRight / PadLeft but also truncates the string if it is too long.
        /// </summary>
        public static string FixWidth( this string str, int wid = 10, bool r = true )
        {
            if (str.Length > wid)
            {
                return str.Substring( 0, wid );
            }
            else if (r)
            {
                return str.PadRight( wid );
            }
            else
            {
                return str.PadLeft( wid );
            }
        }

        /// <summary>
        /// (MJR) Capitalises the first letter of the string.
        /// Other characters remain unchanged.
        /// </summary>
        public static string ToSentence( this string self )
        {
            if (self == null)
            {
                return null;
            }

            if (self.Length <= 1)
            {
                return self.ToUpper();
            }

            return self.Substring( 0, 1 ).ToUpper() + self.Substring( 1 );
        }

        /// <summary>
        /// (MJR) Capitalises the first letter of the string and characters after a space or punctuation.
        /// </summary>
        public static string ToTitle( this string self )
        {
            char[] r = new char[self.Length];
            bool u = true;

            for (int n = 0; n < self.Length; n++)
            {
                r[n] = u ? char.ToUpper( self[n] ) : self[n];
                u = !char.IsLetterOrDigit( self[n] );
            }

            return new string( r );
        }

        /// <summary>
        /// (MJR) Formats x unless it is null or empty, in which case it returns empty.
        /// </summary>
        public static string FormatIf( this string x, string prefix = "", string suffix = "" )
        {
            if (string.IsNullOrEmpty( x ))
            {
                return "";
            }

            return prefix + x + suffix;
        }

        /// <summary>
        /// (MJR) Splits a string about "," accounting for nested sequences using "{" and "}".
        /// </summary>
        public static List<string> SplitGroups( this string text )
        {
            bool isInBrackets = false;
            StringBuilder current = new StringBuilder();
            char[] delimiters = { ',' };
            char[] startBracket = { '{' };
            char[] endBracket = { '}' };
            char[] ignorable = { ' ' };

            bool isNewElement = true;

            List<string> result = new List<string>();

            foreach (char c in text)
            {
                if (isInBrackets && endBracket.Contains( c ))
                {
                    isInBrackets = false;
                }
                else if (!isInBrackets && delimiters.Contains( c ))
                {
                    result.Add( current.ToString() );
                    current.Clear();
                }
                else if (!isInBrackets && isNewElement && startBracket.Contains( c ))
                {
                    isInBrackets = true;
                }
                else
                {
                    if (isNewElement && !ignorable.Contains( c ))
                    {
                        isNewElement = false;
                    }

                    current.Append( c );
                }
            }

            result.Add( current.ToString() );
            current.Clear();

            return result;
        }

        /// <summary>
        /// Converts a string to an array of [T], using a specified [conversion] and splitting about [sep]
        /// </summary>           
        public static T[] StringToArray<T>( string str, Converter<string, T> conversion, string sep = "," )
        {
            string[] elements = str.Split( new[] { sep }, StringSplitOptions.RemoveEmptyEntries );

            T[] result = new T[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i].Trim();
                result[i] = conversion( element );
            }

            return result;
        }

        /// <summary>
        /// Returns a [timeSpan] as a number of minutes or seconds.
        /// </summary>                                             
        public static string TimeAsString( TimeSpan timeSpan )
        {
            if (timeSpan.TotalMinutes > 2)
            {
                return (int)timeSpan.TotalMinutes + " minutes";
            }
            else if (timeSpan.TotalMinutes > 1)
            {
                return (int)timeSpan.TotalMinutes + " minute";
            }
            else
            {
                return (int)timeSpan.TotalSeconds + " seconds";
            }
        }

        /// <summary>
        /// Like string::join this converts an array to a string, but doesn't consider the type.
        /// </summary>                                           
        public static string ArrayToString( IEnumerable array, string delimiter = ", " )
        {
            if (array == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            foreach (object t in array)
            {
                if (sb.Length != 0)
                {
                    sb.Append( delimiter );
                }

                if (t != null)
                {
                    sb.Append( t.ToString() );
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts an array of integers to a string, including ranges (e.g. 2-5) where possible.
        /// </summary>
        /// <param name="list">Values</param>
        /// <param name="prefix">Prefix for each value</param>
        /// <returns>String</returns>
        public static string ArrayToStringInt( IEnumerable<int> list, string prefix = "" )
        {
            if (list == null)
            {
                return null;
            }

            List<int> hs = new List<int>( list );

            hs.Sort();

            if (hs.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            int? onHold = null;

            sb.Append( hs[0] );

            for (int i = 1; i < hs.Count; i++)
            {
                int c = hs[i];

                if (hs[i] == (hs[i - 1] + 1))
                {
                    if (!onHold.HasValue)
                    {
                        // One more!
                        onHold = hs[i - 1];
                    }
                }
                else
                {
                    if (onHold.HasValue)
                    {
                        // Sequence broken
                        int s = onHold.Value;
                        int e = hs[i - 1];
                        onHold = null;

                        WriteFromTo( sb, s, e, prefix );
                    }

                    sb.Append( ", " );
                    sb.Append( prefix );
                    sb.Append( c );
                }
            }

            if (onHold.HasValue)
            {
                // Sequence broken
                int s = onHold.Value;
                int e = hs[hs.Count - 1];
                WriteFromTo( sb, s, e, prefix );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Used by [ArrayToStringInt], this writes a range (x-y), unless the range is 1 in which
        /// case it writes two individual items (x, y).
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="rangeStart"></param>
        /// <param name="rangeEnd"></param>
        /// <param name="prefix"></param>
        private static void WriteFromTo( StringBuilder sb, int rangeStart, int rangeEnd, string prefix )
        {
            if (rangeEnd == rangeStart + 1)
            {
                sb.Append( ", " );
                sb.Append( prefix );
                sb.Append( rangeEnd );
            }
            else
            {
                sb.Append( "-" );
                sb.Append( prefix );
                sb.Append( rangeEnd );
            }
        }

        /// <summary>
        /// Undoes a [ArrayToStringInt], converting an string, possibly including ranges, to an
        /// array of integers.
        /// </summary>        
        public static List<int> StringToArrayInt( string str, EErrorHandler error )
        {
            List<int> result = new List<int>();

            foreach (string elem in str.Split( new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries ))
            {
                string elem2 = elem.Trim();

                int i = elem2.IndexOf( '-', 1 );

                if (i != -1)
                {
                    string[] elem3 = elem2.Split( "-".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries );

                    if (elem3.Length == 2)
                    {
                        int iS, iE;

                        if (int.TryParse( elem3[0].Trim(), out iS )
                            && int.TryParse( elem3[1].Trim(), out iE ))
                        {
                            for (int iC = iS; iC <= iE; iC++)
                            {
                                result.Add( iC );
                            }
                        }
                        else
                        {
                            switch (error)
                            {
                                case EErrorHandler.Ignore:
                                    break;

                                case EErrorHandler.ReturnNull:
                                    return null;

                                case EErrorHandler.ThrowError:
                                default:
                                    throw new FormatException( "Cannot parse: " + elem2 );
                            }
                        }
                    }
                }
                else
                {
                    int iC;

                    if (int.TryParse( elem2, out iC ))
                    {
                        result.Add( iC );
                    }
                    else
                    {
                        switch (error)
                        {
                            case EErrorHandler.Ignore:
                                break;

                            case EErrorHandler.ReturnNull:
                                return null;

                            case EErrorHandler.ThrowError:
                            default:
                                throw new FormatException( "Cannot parse: " + elem2 );
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Converts an array to a string
        /// </summary>                   
        public static string ArrayToString( IEnumerable array, Converter<object, string> conversion, string delimiter = ", " )
        {
            if (array == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            foreach (object t in array)
            {
                if (sb.Length != 0)
                {
                    sb.Append( delimiter );
                }

                sb.Append( conversion( t ) );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts an array to a string
        /// </summary>                   
        public static string ArrayToString<T>( IEnumerable<WeakReference<T>> array, Converter<T, string> conversion, string delimiter = ", " )
            where T : class
        {
            return ArrayToString( array, z => WeakReferenceHelper.SafeToString( z, conversion ), delimiter );
        }

        /// <summary>
        /// Converts an array to a string
        /// </summary>                   
        public static string ArrayToString<T>( IEnumerable<T> array, Converter<T, string> conversion, string delimiter = ", " )
        {
            if (array == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            foreach (T t in array)
            {
                if (sb.Length != 0)
                {
                    sb.Append( delimiter );
                }

                sb.Append( conversion( t ) );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Splits a string into a field:value pair about a specified symbol
        /// </summary>
        /// <param name="elem">The original string --> the first part</param>
        /// <param name="splitSign">What to split about</param>
        /// <returns>The last part</returns>
        public static string SplitEquals( ref string elem, string splitSign = "=" )
        {
            int i = elem.IndexOf( splitSign );

            if (i != -1)
            {
                string result = elem.Substring( i + 1 );
                elem = elem.Substring( 0, i );
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Splits a string into a field:value pair about a specified symbol
        /// </summary>
        public static bool SplitEquals( string elem, out string left, out string right, string splitSign = "=" )
        {
            int i = elem.IndexOf( splitSign );

            if (i != -1)
            {
                right = elem.Substring( i + 1 );
                left = elem.Substring( 0, i );
                return true;
            }
            else
            {
                left = elem;
                right = null;
                return false;
            }
        }

        /// <summary>
        /// Describes a as a fraction of b (as a string).
        /// </summary>
        public static string AsFraction( int a, int b )
        {
            return a.ToString() + " / " + b + " (" + (((double)a / b) * 100).ToString( "F00" ) + "%)";
        }

        public static string JoinAsString( this IEnumerable array, string separator = ", " )
        {
            return string.Join( separator, array.Cast<object>() );
        }

        /// <summary>
        /// Displays a number of bytes as b, kb or Mb
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string DisplayBytes( long bytes )
        {
            if (bytes < 1024)
            {
                return bytes + "b";
            }
            else if (bytes < (1024 * 1024))
            {
                return (bytes / 1024) + "kb";
            }
            else
            {
                return (bytes / (1024 * 1024)) + "Mb";
            }
        }

        public static bool IsInteger( string id )
        {
            return id.All( char.IsDigit );
        }

        public static string After( this string self, string text )
        {
            int index = self.IndexOf( text );

            if (index != -1)
            {
                return self.Substring( index + text.Length );
            }

            return string.Empty;
        }

        public static string Before( this string self, string text )
        {
            int index = self.IndexOf( text );

            if (index != -1)
            {
                return self.Substring( 0, index );
            }

            return text;
        }

        public static int GetDecimalPlaces( string number )
        {
            return number.Length - number.IndexOf( '.' ) - 1;
        }

        /// <summary>
        /// (MJR) (EXTENSION)
        /// Gets the display name of a MemberInfo.
        /// </summary>
        /// <remarks>
        /// Normally <see cref="DisplayNameAttribute"/> is used to define the names of such things,
        /// but since <see cref="EnumHelper.ToUiString(Enum)"/> is forced to use <see cref="NameAttribute"/>
        /// due to constraints on <see cref="DisplayNameAttribute"/> this function checks for both.
        /// </remarks>
        public static string ToUiString( this MemberInfo self )
        {
            Type type = self as Type;

            if (type !=null && type.IsGenericType)
            {             
                return FormatGenericName( self, type.GetGenericArguments() );
            }

            MethodInfo method = self as MethodInfo;

            if (method != null && method.IsGenericMethod)
            {                     
                return FormatGenericName( self, method.GetGenericArguments() );
            }

            return GetNameFromAttribute( self );
        }

        private static string FormatGenericName( MemberInfo member, Type[] arguments )
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( GetNameFromAttribute( member ) );
            sb.Append( "<" );
            sb.Append( string.Join( ", ", arguments.Select( z => z.ToUiString() ) ) );
            sb.Append( ">" );

            return sb.ToString();
        }

        private static string GetNameFromAttribute( MemberInfo member )
        {
            NameAttribute attr = member.GetCustomAttribute<NameAttribute>();

            if (attr != null)
            {
                return attr.Name;
            }

            DisplayNameAttribute dattr = member.GetCustomAttribute<DisplayNameAttribute>();

            if (dattr != null)
            {
                return dattr.DisplayName;
            }

            int x = member.Name.IndexOf( "`" );

            if (x != -1)
            {
                return member.Name.Substring( 0, x );
            }

            return member.Name;
        }

        public static string UndoCamelCase( string name )
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in name)
            {
                bool upper = char.IsUpper( c );

                if (upper && sb.Length!=0)
                {
                    sb.Append( " " );
                }

                sb.Append( c );          
            }

            return sb.ToString();
        }

        private static Comparer _comparer = Comparer.Default;

        /// <summary>
        /// Implements the Windows API function "StrCmpLogicalW" in MSIL.
        /// </summary>         
        public static int StrCmpLogicalW( string x, string y )
        {
            if (x == null)
            {

            }

            string[] xElements = Regex.Split( x, "([0-9]+)" );
            string[] yElements = Regex.Split( y, "([0-9]+)" );
            IEnumerable<object> xObjects = xElements.Select( ToIntIfPossible );
            IEnumerable<object> yObjects = yElements.Select( ToIntIfPossible );

            return CompareEnumerables( xObjects, yObjects );
        }

        /// <summary>
        /// Compares two enumerables.
        /// Used by <see cref="StrCmpLogicalW"/>.
        /// </summary>                           
        private static int CompareEnumerables( IEnumerable<object> x, IEnumerable<object> y )
        {
            IEnumerator xi = x.GetEnumerator();
            IEnumerator yi = y.GetEnumerator();

            while (true)
            {
                bool xm = xi.MoveNext();
                bool rm = yi.MoveNext();

                if (!(xm || rm))
                {
                    return 0;
                }

                if (!xm)
                {
                    return -1;
                }

                if (!rm)
                {
                    return 1;
                }

                int compared = _comparer.Compare( xi.Current, yi.Current );

                if (compared != 0)
                {
                    return compared;
                }
            }

        }

        /// <summary>
        /// Returns the argument as an integer, or itself if that is not possible.
        /// </summary> 
        private static object ToIntIfPossible( string arg )
        {
            int v;

            if (int.TryParse( arg, out v ))
            {
                return v;
            }

            return arg;
        }
    }
}
