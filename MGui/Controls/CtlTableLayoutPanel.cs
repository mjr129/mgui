using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Controls
{
    public class CtlTableLayoutPanel : TableLayoutPanel
    {
        public CtlTableLayoutPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        public CtlTableLayoutPanel(IContainer container)
            : this()
        {
            container.Add(this);
        }
    }
}
