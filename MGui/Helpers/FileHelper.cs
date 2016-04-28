using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    public static class FileHelper
    {
        public static string Browse( IWin32Window form, string filter, string @default )
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.FileName = @default;

                if (ofd.ShowDialog( form ) == DialogResult.OK)
                {
                    return ofd.FileName;
                }

                return null;
            }               
        }

        public static void Browse( TextBox textBox, string filter )
        {
            string nfn = Browse( textBox.FindForm(), filter, textBox.Text );

            if (nfn != null)
            {
                textBox.Text = nfn;
            }
        }

        public static string DateAndTimeFile()
        {
            return DateAndTimeFile( DateTime.Now );
        }

        private static string DateAndTimeFile( DateTime now )
        {
            return DateTime.Now.ToString( "yyyy MM dd - HH mm ss" );
        }
    }
}
