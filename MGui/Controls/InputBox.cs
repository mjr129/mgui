using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Controls
{
    /// <summary>
    /// A simple InputBox for requesting text from the user.
    /// Analogous to the one in VisualBasic.
    /// </summary>
    public partial class InputBox : Form
    {           
        /// <summary>
        /// Shows the <see cref="InputBox"/>.
        /// </summary>           
        public static string Show( IWin32Window owner, string message, bool multiLine = false )
        {
            return Show( owner, null, message, null, multiLine );
        }       
  
        /// <summary>
        /// Shows the <see cref="InputBox"/>.
        /// </summary>           
        public static string Show( IWin32Window owner, string message, string defaultInput, bool multiLine = false )
        {
            return Show( owner, null, message, defaultInput, multiLine );
        }              

        /// <summary>
        /// Shows the <see cref="InputBox"/>.
        /// </summary>
        /// <param name="owner">Owning form (or null)</param>
        /// <param name="windowTitle">Window title (null ~ owner.Text)</param>
        /// <param name="message">Message text (null ~ "")</param>
        /// <param name="defaultInput">Default value (null ~ "")</param>
        /// <param name="multiLine">Multi-line edit mode</param>
        /// <returns>Text entered, or null if cancelled.</returns>
        public static string Show( IWin32Window owner, string windowTitle, string message, string defaultInput, bool multiLine = false )
        {
            using (InputBox inputBox = new InputBox())
            {
                if (windowTitle == null)
                {
                    if (owner is Form)
                    {
                        windowTitle = ((Form)owner).Text;
                    }
                    else
                    {
                        windowTitle = "Input";
                    }
                }

                inputBox.label1.Text = message;
                inputBox.label1.Visible = !string.IsNullOrEmpty( message );
                inputBox.Text = windowTitle;
                inputBox.textBox1.Multiline = multiLine;
                inputBox.textBox1.Text = defaultInput;

                if (multiLine)
                {
                    inputBox.Height *= 2;
                }

                if (inputBox.ShowDialog( owner ) == DialogResult.OK)
                {
                    return inputBox.textBox1.Text;
                }

                return null;
            }
        }

        /// <summary>
        /// CONSTRUCTOR (Private)
        /// </summary>
        private InputBox()
        {
            InitializeComponent();
        }
    }
}
