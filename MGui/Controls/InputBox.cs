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
        public static string Show( IWin32Window owner, string prompt, EMode mode )
        {
            return Show( owner, null, prompt, null, mode, null );
        }

        public static string Show( IWin32Window owner, string prompt )
        {
            return Show( owner, null, prompt, null, EMode.SingleLine, null );
        }

        public static string Show( IWin32Window owner, string prompt, EMode mode, IEnumerable options )
        {
            return Show( owner, null, prompt, null, mode, options );
        }

        public static string Show( IWin32Window owner, string prompt, object defaultInput )
        {
            return Show( owner, null, prompt, defaultInput, EMode.SingleLine, null );

        }

        public static string Show( IWin32Window owner, string prompt, object @default, EMode mode )
        {
            return Show( owner, null, prompt, @default, mode, null );
        }

        /// <summary>
        /// Shows the <see cref="InputBox"/> using a default windowTitle.
        /// </summary>           
        public static string Show( IWin32Window owner, string prompt, object @default, EMode mode, IEnumerable options )
        {
            return Show( owner, null, prompt, @default, mode, options );
        }              

        /// <summary>
        /// Shows the <see cref="InputBox"/>.
        /// </summary>
        /// <param name="owner">Owning form (or null)</param>
        /// <param name="windowTitle">Window title (null ~ owner.Text)</param>
        /// <param name="prompt">Message text (null ~ "")</param>
        /// <param name="default">Default value (null ~ "")</param>
        /// <param name="mode">Multi-line edit mode</param>
        /// <returns>Text entered, or null if cancelled.</returns>
        public static string Show( IWin32Window owner, string windowTitle, string prompt, object @default, EMode mode, IEnumerable options)
        {
            using (InputBox inputBox = new InputBox( owner, windowTitle, prompt, @default, mode, options ))
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
        private InputBox( IWin32Window owner, string windowTitle, string prompt, object @default, EMode mode, IEnumerable options )
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

            this.label1.Text    = prompt;
            this.label1.Visible = !string.IsNullOrEmpty( prompt );
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
                    _control.Dock = DockStyle.Top;
                    break;

                case EMode.SingleLine:
                    TextBox slTextBox = new TextBox();
                    _control = slTextBox;
                    _control.Dock = DockStyle.Top;
                    break;

                case EMode.MultiLine:
                    TextBox mlTextBox = new TextBox();
                    _control = mlTextBox;
                    mlTextBox.Multiline = true;
                    mlTextBox.ScrollBars = ScrollBars.Vertical;
                    mlTextBox.Dock = DockStyle.Fill;
                    this.Height *= 2;
                    break;
            }

            _control.Text    = @default != null ? @default.ToString() : null;
            _control.TabIndex = 1;
            _control.Visible = true;
            _control.Margin = new Padding( 8, 8, 8, 8 );
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.Controls.Add( _control, 0, 1 );
            tableLayoutPanel1.SetColumnSpan( _control, 3 );
            tableLayoutPanel1.ResumeLayout();

            _control.Focus();
        }
    }
}
