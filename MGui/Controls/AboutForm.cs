using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MGui.Datatypes;

namespace MGui.Controls
{
    /// <summary>
    /// A generic about form.
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// Shows the about form as a dialog.
        /// </summary>
        /// <param name="owner">owning form</param>
        /// <param name="main">assembly to describe</param>
        public static void Show( Form owner, Assembly main, string additionalInformation = null, string webPage = null )
        {
            using (AboutForm frm = new AboutForm(main, owner.Icon, additionalInformation, webPage ))
            {
                frm.ShowDialog( owner );
            }
        }

        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        /// <param name="main">assembly to describe</param>
        private AboutForm(Assembly main, Icon icon, string additionalInformation, string webPage )
        {
            InitializeComponent();

            label5.Text = additionalInformation;
            label5.Visible = !string.IsNullOrEmpty( additionalInformation );

            linkLabel1.Text = webPage;
            linkLabel1.Visible = !string.IsNullOrEmpty( webPage );

            LoadAssemblies();
            Describe( main );
            ShowAssemblies();

            pictureBox1.Image = icon.ToBitmap();
        }

        List<NamedValue<Assembly>> allMsAssemblies = new List<NamedValue<Assembly>>();
        List<NamedValue<Assembly>> allNonMsAssemblies = new List<NamedValue<Assembly>>();

        private void LoadAssemblies()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                AddListItem( asm );
            }
        }

        private void ShowAssemblies()
        {
            listBox1.Items.Clear();

            if (showMicrosoftAssembliesToolStripMenuItem.Checked)
            {
                listBox1.Items.AddRange( allNonMsAssemblies.ToArray() );
                listBox1.Items.AddRange( allMsAssemblies.ToArray() );
            }
            else
            {
                listBox1.Items.AddRange( allNonMsAssemblies.ToArray() );
            }
        }

        private void AddListItem( Assembly assembly )
        {
            StringBuilder sb = new StringBuilder();

            var asName = assembly.GetName();
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

            bool debug = assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITTrackingEnabled ?? false;
            var version = asName.Version + (debug ? ".D" : "");

            sb.Append( asName.Name );
            sb.Append( " | " + version );
            sb.Append( " | " + company );

            var item = new NamedValue<Assembly>( sb.ToString(), assembly );

            if (company == "Microsoft Corporation")
            {
                allMsAssemblies.Add( item );
            }
            else
            {
                allNonMsAssemblies.Add( item );
            }
        }

        public void Describe( Assembly assembly )
        {
            StringBuilder sb = new StringBuilder();

            var asName = assembly.GetName();
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            bool debug = assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITTrackingEnabled ?? false;
            var version = asName.Version + (debug ? ".D" : "");

            label1.Text = asName.Name;
            label3.Text = version;
            label4.Text = company;

            if (debug)
            {
                label3.ForeColor = Color.Red;   
            }
        }      

        private void listBox1_DrawItem( object sender, DrawItemEventArgs e )
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();

            string item =listBox1.Items[ e.Index ].ToString();
            string[] elements = item.Split( '|' );

            int x = e.Bounds.Left;
            int y = e.Bounds.Top;

            SizeF size = e.Graphics.MeasureString( elements[0], e.Font );
            e.Graphics.DrawString( elements[0], e.Font, Brushes.Black, x, y );

            x += (int)size.Width;

            size = e.Graphics.MeasureString( elements[1], e.Font );
            e.Graphics.DrawString( elements[1], e.Font, (elements[1].EndsWith( ".D " ) ? Brushes.Red : Brushes.Blue), x, y );

            x += (int)size.Width;

            e.Graphics.DrawString( elements[2], e.Font, Brushes.Silver, x, y );

            e.DrawFocusRectangle();            
        }

        private void showMicrosoftAssembliesToolStripMenuItem_Click( object sender, EventArgs e )
        {
            ShowAssemblies();
        }

        private void tableLayoutPanel3_Paint( object sender, PaintEventArgs e )
        {

        }

        private void _btnCopy_Click( object sender, EventArgs e )
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( label1.Text );
            sb.AppendLine( label3.Text );
            sb.AppendLine( label4.Text );
            sb.AppendLine( label2.Text );

            foreach (var asm in allNonMsAssemblies)
            {
                sb.AppendLine( asm.ToString() );
            }

            foreach (var asm in allMsAssemblies)
            {
                sb.AppendLine( asm.ToString() );
            }

            Clipboard.SetText( sb.ToString() );
        }

        private void _btnView_Click( object sender, EventArgs e )
        {
            contextMenuStrip1.Show( _btnView, 0, _btnView.Height );
        }

        private void label3_Click( object sender, EventArgs e )
        {

        }

        private void label4_Click( object sender, EventArgs e )
        {

        }

        private void linkLabel1_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            Process.Start( linkLabel1.Text );
        }     

        private void listBox1_SelectedIndexChanged( object sender, EventArgs e )
        {
            exploreToassemblyToolStripMenuItem.Enabled = listBox1.SelectedItem != null;

            if (listBox1.SelectedItem != null)
            {
                exploreToassemblyToolStripMenuItem.Text = "&Explore folder of " + ((NamedValue<Assembly>)listBox1.SelectedItem).Value.GetName().Name;
            }
        }

        private void exploreToassemblyToolStripMenuItem_Click( object sender, EventArgs e )
        {
            try
            {
                Process.Start( "explorer.exe", "/select,\"" + ((NamedValue<Assembly>)listBox1.SelectedItem).Value.Location + "\"" );
            }
            catch (Exception ex)
            {
                MessageBox.Show( this, ex.Message );
            }
        }
    }
}
