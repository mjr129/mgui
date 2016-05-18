using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MGui.Helpers;

namespace MGui.Controls
{
    public partial class CtlFileSelector : UserControl
    {
        public CtlFileSelector()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return textBox1.Text;
            }   
            set
            {
                textBox1.Text = value;
            }
        }

        [DefaultValue(false)]
        public bool BrowseFolders { get; set; }

        [DefaultValue( "All files (*.*)|*.*")]
        public string Filter { get; set; } = "All files (*.*)|*.*";

        public bool Prompt()
        {
            if (BrowseFolders)
            {
                return textBox1.SetTextIfNotNull( FileHelper.BrowseForFolder( this, textBox1.Text ) );
            }
            else
            {
                return FileHelper.Browse( textBox1, Filter );
            }
        }

        private void button1_Click( object sender, EventArgs e )
        {
            Prompt();
        }

        protected override void OnSizeChanged( EventArgs e )
        {
            base.OnSizeChanged( e );

            Height = textBox1.Height;
            button1.Size = new Size( textBox1.Height, textBox1.Height );
        }
    }
}
