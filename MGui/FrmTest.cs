using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MGui.Controls;

namespace MGui
{
    public partial class FrmTest : Form
    {
        public FrmTest()
        {
            InitializeComponent();
        }

        public static void Main()
        {
            Application.Run( new FrmTest() );
        }

        private void button1_Click( object sender, EventArgs e )
        {
            
        }

        private void ctlColourEditor1_Click( object sender, EventArgs e )
        {

        }
    }
}
