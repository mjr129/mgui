using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    public static class ListBoxHelper
    {
        public static bool RemoveSelectedItem( this ListBox listBox )
        {
            if (listBox == null)
            {
                return false;
            }

            if (listBox.SelectedItem != null)
            {
                listBox.Items.Remove( listBox.SelectedItem );
                return true;
            }

            return false;
        }
    }
}
