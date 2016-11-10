using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using MGui.Helpers;

namespace MGui.Datatypes
{
    public sealed class XmlDocReader
    {
        public Dictionary<string, string> _documentation = new Dictionary<string, string>();

        public XmlDocReader()
        {
            // NA
        }

        public void ReadMine()
        {
            Read( FileHelper.ChangeExtension( Application.ExecutablePath, ".xml" ) );
        }

        public void Read( string fileName )
        {
            XDom root = XDom.LoadFile( fileName, XDom.EDomType.Xml );

            if (root.Name != "doc")
            {
                throw new FormatException( $"Expected the XML documentation file {{{fileName}}} to have a root named doc, but it is named {{{root.Name}}}." );
            }

            XDom members = root["members"];

            foreach (XDom member in members.GetItems( "member" ))
            {
                XDom name = member["name"];
                XDom summary = member["summary"];

                _documentation.Add( name.Value, summary.Value );
            }
        }                

        public string Get( MemberInfo member )
        {
            string prefix = GetPrefix( member );
            string suffix = GetQualifiedName( member );
            string result;

            if (_documentation.TryGetValue(prefix+suffix, out result))
            {
                return result;
            }

            return null;
        }

        private string GetQualifiedName( MemberInfo member )
        {
            switch (member.MemberType)
            {
                case MemberTypes.NestedType:
                case MemberTypes.TypeInfo:
                    TypeInfo t = (TypeInfo)member;
                    return t.FullName;

                default:
                    return GetQualifiedName( member.DeclaringType ) + "." + member.Name;
            }
        }

        private static string GetPrefix( MemberInfo member )
        {                   
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return "E:";

                case MemberTypes.Field:
                    return "F:";

                case MemberTypes.Constructor:
                case MemberTypes.Method:
                    return "M:";

                case MemberTypes.Property:
                    return "P:";

                case MemberTypes.NestedType:
                case MemberTypes.TypeInfo:
                    return "T:";

                case MemberTypes.Custom:
                case MemberTypes.All:
                default:
                    throw new NotSupportedException( $"Cannot get documentation for type {{{member.MemberType}}}." );
            }
        }
    }
}
