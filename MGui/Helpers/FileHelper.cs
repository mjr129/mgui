using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    public static class FileHelper
    {
        public static string BrowseForFolder( IWin32Window form, string @default = null )
        {
            using (OpenFileDialog sfd = new OpenFileDialog())
            {
                const string SELECT_FILENAME = "Select this directory";

                if (!string.IsNullOrEmpty( @default ))
                {
                    sfd.FileName = Path.Combine( @default, SELECT_FILENAME );
                    sfd.InitialDirectory = @default;
                }
                else
                {
                    sfd.FileName = SELECT_FILENAME;
                }
                
                sfd.Filter = "All directories|*.*";
                sfd.CheckFileExists = false;
                sfd.Multiselect = false;
                sfd.Title = "Select Directory";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return Path.GetDirectoryName( sfd.FileName );
                }

                return null;
            }
        }

        public static void BrowseForFolder( TextBox textBox )
        {
            string x = BrowseForFolder( textBox.FindForm(), textBox.Text );

            if (x != null)
            {
                textBox.Text = x;
            }
        }

        public static string Browse( IWin32Window form, string filter, string @default = null )
        {
            using (FileDialog ofd = new OpenFileDialog())
            {
                return BrowseInternal( form, filter, @default, ofd );
            }
        }

        public static string Save( IWin32Window form, string filter, string @default )
        {
            using (FileDialog ofd = new SaveFileDialog())
            {
                return BrowseInternal( form, filter, @default, ofd );
            }
        }

        private static string BrowseInternal( IWin32Window form, string filter, string @default, FileDialog ofd )
        {
            ofd.FileName = @default;
            ofd.Filter = filter;

            if (ofd.ShowDialog( form ) == DialogResult.OK)
            {
                return ofd.FileName;
            }

            return null;
        }

        public static bool Browse( TextBox textBox, string filter )
        {
            string nfn = Browse( textBox.FindForm(), filter, textBox.Text );

            if (nfn != null)
            {
                textBox.Text = nfn;
                return true;
            }

            return false;
        }

        public static string DateAndTimeFile()
        {
            return DateAndTimeFile( DateTime.Now );
        }

        private static string DateAndTimeFile( DateTime now )
        {
            return DateTime.Now.ToString( "yyyy MM dd - HH mm ss" );
        }

        public static bool SetTextIfNotNull( this TextBox self, string newValue )
        {
            if (newValue != null)
            {
                self.Text = newValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Changes the extension of a fullname, including path.
        /// </summary>
        /// <param name="fileName">Original filename</param>
        /// <param name="extension">New extension, including the dot and anything new suffix to put before the dot.</param>
        /// <returns>Filename with new extension</returns>
        public static string ChangeExtension( string fileName, string extension )
        {
            return Path.Combine( Path.GetDirectoryName( fileName ), Path.GetFileNameWithoutExtension( fileName ) ) + extension;
        }

        public static string SanitiseFilename( string file )
        {
            foreach (char c in "\\/?*|><:\"")
            {
                file = file.Replace( c, '-' );
            }

            return file;
        }
    }
}
