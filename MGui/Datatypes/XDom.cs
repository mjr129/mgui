using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MGui.Datatypes
{
    /// <summary>
    /// Creates a DOM hierarchy from an XML file.
    /// </summary>
    public class XDom
    {
        public enum EDomAttributes
        {
            None = 0,
            Attribute
        }

        public enum EDomType
        {
            Xml = 1,
            Hierarchy
        }

        public XDom Parent = null;
        public List<XDom> Contents = new List<XDom>();
        public string Name = "";
        public string Value = "";
        public EDomAttributes Attributes = EDomAttributes.None;

        /// <summary>
        /// Removes white space from a string
        /// </summary>
        private static string RemoveWhiteSpace( string aStringIn )
        {
            char[] whiteSpaceChars = { ' ', '\n', '\r' };
            string r = aStringIn.Trim( whiteSpaceChars );
            while (r.Contains( "  " ))
                r = r.Replace( "  ", " " );

            return r;
        }

        /// <summary>
        /// Returns the first item with aTitle from this object's collection
        /// </summary>
        public XDom GetItem( string aTitle )
        {
            aTitle = aTitle.ToLower();
            return this.Contents.FirstOrDefault( c => c.Name.ToLower() == aTitle );
        }

        /// <summary>
        /// Returns all items with aTitle from this object's collection
        /// </summary>
        public IEnumerable<XDom> GetItems( string aTitle )
        {
            aTitle = aTitle.ToLower();
            return Contents.Where( z => z.Name.ToLower() == aTitle );
        }

        public bool Has( string title )
        {
            return GetItem( title ) != null;
        }

        public XDom this[string title]
        {
            get
            {
                XDom result = GetItem( title );

                if (result == null)
                {
                    throw new InvalidOperationException( $"Expected to find {title} in {Name}." );
                }

                return result;
            }
        }

        /// <summary>
        /// As GetItem, but creates the item if it does not exist
        /// </summary>
        public XDom GetOrCreateItem( string aTitle )
        {
            XDom r = GetItem( aTitle );
            if (r == null)
            {
                r = new XDom();
                r.Name = aTitle;
                r.Parent = this;
                this.Contents.Add( r );
            }
            return r;
        }

        /// <summary>
        /// Finds how many characters aStrIn is indented by, returns the unindented form of the string in aStrOut
        /// </summary>
        private static int FindIndent( string aStrIn, ref string aStrOut )
        {
            // Hello, world
            aStrOut = aStrIn.TrimStart();
            return aStrIn.Length - aStrOut.Length;
        }

        /// <summary>
        /// Creates a new CDom object from an INI-style line ("x=y")
        /// </summary>
        private static XDom NewFromIniLine( string aLine, XDom aParent )
        {
            XDom r = new XDom();
            r.Parent = aParent;
            if (r.Parent != null)
            {
                r.Parent.Contents.Add( r );
            }
            if (aLine.Contains( "=" ))
            {
                r.Name = aLine.Substring( 0, aLine.IndexOf( '=' ) ).Trim();
                r.Value = aLine.Substring( aLine.IndexOf( '=' ) + 1 ).Trim();
            }
            else
            {
                r.Name = aLine.Trim();
                r.Value = "";
            }
            return r;
        }

        public static XDom LoadFile( string aFileName, EDomType aFileType )
        {
            switch (aFileType)
            {
                case EDomType.Xml:
                    return NewFromXml( aFileName );
                case EDomType.Hierarchy:
                    return NewFromIndentedHierarchy( aFileName );
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates a new CDom object from a heirarchial-INI file
        /// </summary>
        private static XDom NewFromIndentedHierarchy( string aFileName )
        {
            XDom rootNode = new XDom();
            string[] fc = System.IO.File.ReadAllLines( aFileName );
            Stack<int> indentStack = new Stack<int>();
            indentStack.Push( 0 );
            int currentIndent = 0;
            XDom previousNode = null;
            XDom levelNode = rootNode;
            foreach (string ll in fc)
            {
                string l = "";
                int i = FindIndent( ll, ref l );
                if (l.Length != 0)
                {
                    if (l.StartsWith( "=" ))
                    {
                        // Append
                        previousNode.Value += "\n" + l.Substring( 1 );
                    }
                    else if (i > currentIndent)
                    {
                        // Indent
                        indentStack.Push( i );
                        levelNode = previousNode;
                    }
                    else if (i < currentIndent)
                    {
                        // Unindent
                        while (i < indentStack.Peek())
                        {
                            indentStack.Pop();
                            levelNode = levelNode.Parent;
                        }
                    }

                    previousNode = NewFromIniLine( l, levelNode );
                    currentIndent = i;
                }
            }

            return rootNode;
        }

        /// <summary>
        /// Returns a simple string representation of this object
        /// </summary>
        public override string ToString()
        {
            return this.Name + "=" + this.Value;
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        public string ToString( EDomType aType )
        {
            return ToString( aType, 0 );
        }

        /// <summary>
        /// Returns a string representation of this object with a set indent
        /// </summary>
        private string ToString( EDomType aType, int aIndent )
        {
            StringBuilder sb = new StringBuilder();
            if (this.Value.Length != 0 || this.Contents.Count == 0)
            {
                sb.Append( ' ', aIndent * 4 );
                switch (aType)
                {
                    case EDomType.Xml:
                        sb.Append( "<" + Name + ">" + Value );
                        break;
                    case EDomType.Hierarchy:
                        sb.Append( Name + "=" + Value + "\r\n" );
                        break;
                }

                foreach (XDom c in this.Contents)
                    sb.Append( c.ToString( aType, aIndent + 1 ) );

                switch (aType)
                {
                    case EDomType.Xml:
                        sb.Append( "</" + Name + ">\r\n" );
                        break;
                    case EDomType.Hierarchy:
                        sb.Append( "\r\n" );
                        break;
                }
            }
            else
            {
                sb.Append( ' ', aIndent * 2 );

                switch (aType)
                {
                    case EDomType.Xml:
                        sb.Append( "<" + Name + ">\r\n" );
                        break;
                    case EDomType.Hierarchy:
                        sb.Append( Name + "\r\n" );
                        break;
                }

                foreach (XDom c in this.Contents)
                    sb.Append( c.ToString( aType, aIndent + 1 ) );

                switch (aType)
                {
                    case EDomType.Xml:
                        sb.Append( ' ', aIndent * 2 );
                        sb.Append( "</" + Name + ">\r\n" );
                        break;
                    case EDomType.Hierarchy:
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a new CDom object from an XML file
        /// </summary>
        private static XDom NewFromXml( string aFileName )
        {
            // Settings
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.IgnoreComments = true;
            xrs.IgnoreWhitespace = true;
            xrs.CloseInput = true;
            xrs.CheckCharacters = true;
            xrs.ProhibitDtd = false;

            // Open file
            XmlReader reader = XmlReader.Create( aFileName, xrs );

            // Create root node
            XDom rootNode = null;
            XDom currentNode = null;

            // Read nodes
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            XDom newNode = new XDom();
                            newNode.Name = reader.Name;
                            if (rootNode == null)
                                rootNode = newNode;
                            else
                                currentNode.Contents.Add( newNode );
                            newNode.Parent = currentNode;

                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    XDom newAttrNode = new XDom();
                                    newAttrNode.Name = reader.Name;
                                    newAttrNode.Value = reader.Value;
                                    newAttrNode.Attributes = EDomAttributes.Attribute;
                                    newNode.Contents.Add( newAttrNode );
                                }

                                reader.MoveToElement();
                            }

                            if (!reader.IsEmptyElement)
                                currentNode = newNode;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        currentNode = currentNode.Parent;
                        break;
                    case XmlNodeType.Text:
                        currentNode.Value += RemoveWhiteSpace( reader.Value );
                        break;
                    case XmlNodeType.Attribute:
                        {
                            XDom newNode = new XDom();
                            newNode.Name = reader.Name;
                            newNode.Value = reader.Value;
                            newNode.Attributes = EDomAttributes.Attribute;
                            currentNode.Contents.Add( newNode );
                        }
                        break;

                    case XmlNodeType.XmlDeclaration:
                        break;

                    default:
                        throw new Exception( "Unknown XML node: " + reader.NodeType.ToString() );
                } // switch
            } // while

            reader.Close();

            return rootNode;
        }
    }
}
