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
        public static bool HasSelection( this ListBox listBox )
        {
            return listBox?.SelectedItem != null;
        }

        public static bool MoveSelectedItemDown( this ListBox listBox )
        {
            if (!HasSelection( listBox ))
            {
                return false;
            }

            int index = listBox.SelectedIndex;

            if (index == listBox.Items.Count - 1)
            {
                return false;
            }

            object item = listBox.SelectedItem;

            listBox.Items.RemoveAt( index );
            listBox.Items.Insert( index + 1, item );

            return true;
        }

        public static bool MoveSelectedItemUp( this ListBox listBox )
        {
            if (!HasSelection( listBox ))
            {
                return false;
            }

            int index = listBox.SelectedIndex;

            if (index == 0)
            {
                return false;
            }

            object item = listBox.SelectedItem;

            listBox.Items.RemoveAt( index );
            listBox.Items.Insert( index - 1, item );

            return true;
        }

        public static bool RemoveSelectedItem( this ListBox listBox )
        {
            if (!HasSelection(listBox))
            {
                return false;
            }    

            listBox.Items.Remove( listBox.SelectedItem );
            return true;
        }
    }
}
