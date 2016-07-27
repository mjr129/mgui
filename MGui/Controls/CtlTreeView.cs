using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Controls
{
    public class CtlTreeView : TreeView
    {
        protected override void WndProc( ref Message m )
        {
            // Ignore double click because it messes with state
            if (m.Msg == 0x0203)
            {
                m.Result = IntPtr.Zero;
            }
            else
            {
                base.WndProc( ref m );
            }
        }
    }
}
