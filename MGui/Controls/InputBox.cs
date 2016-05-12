using System;
using System.Collections;
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
        private Control _control;

        /// <summary>
        /// Shows the <see cref="InputBox"/> using a default selection (<see cref="string.Empty"/>).
        /// </summary>           
        public static string Show( IWin32Window owner, string message, EMode multiLine = EMode.SingleLine, IEnumerable options = null )
        {
            return Show( owner, null, message, null, multiLine, options );
        }       
  
        /// <summary>
        /// Shows the <see cref="InputBox"/> using a default windowTitle.
        /// </summary>           
        public static string Show( IWin32Window owner, string message, object defaultInput, EMode multiLine = EMode.SingleLine, IEnumerable options = null )
        {
            return Show( owner, null, message, defaultInput, multiLine, options );
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
        public static string Show( IWin32Window owner, string windowTitle, string message, object defaultInput, EMode multiLine = EMode.SingleLine, IEnumerable options = null )
        {
            using (InputBox inputBox = new InputBox( owner, windowTitle, message, defaultInput, multiLine, options ))
            { 
                if (inputBox.ShowDialog( owner ) == DialogResult.OK)
                {
                    return inputBox._control.Text;
                }

                return null;
            }
        }

        public enum EMode
        {
            SingleLine,
            MultiLine,
            ComboBox,
        }

        /// <summary>
        /// CONSTRUCTOR (Private)
        /// </summary>
        private InputBox( IWin32Window owner, string windowTitle, string message, object @default, EMode mode, IEnumerable options )
        {
            InitializeComponent();

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

            this.label1.Text    = message;
            this.label1.Visible = !string.IsNullOrEmpty( message );
            this.Text           = windowTitle;

            switch (mode)
            {
                case EMode.ComboBox:
                    ComboBox comboBox = new ComboBox();

                    if (options != null)
                    {
                        comboBox.Items.AddRange( options.Cast<object>().ToArray() );
                    }

                    _control = comboBox;
                    break;

                case EMode.SingleLine:
                    TextBox slTextBox = new TextBox();
                    _control = slTextBox;
                    break;

                case EMode.MultiLine:
                    TextBox mlTextBox = new TextBox();
                    _control = mlTextBox;
                    mlTextBox.Multiline = true;
                    this.Height *= 2;
                    break;
            }

            _control.Text    = @default != null ? @default.ToString() : null;
            _control.Visible = true;
            _control.Dock = DockStyle.Top;
            _control.Margin = new Padding( 8, 8, 8, 8 );
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.Controls.Add( _control, 0, 1 );
            tableLayoutPanel1.SetColumnSpan( _control, 3 );
            tableLayoutPanel1.ResumeLayout();

            _control.Focus();
        }
    }
}
