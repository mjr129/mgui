using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MGui.Datatypes;

namespace MGui.Helpers
{
    public static class ListViewHelper
    {
        public static void Populate( ListView listView, IEnumerable contents )
        {
            listView.Clear();

            var en = contents.GetEnumerator();

            if (!en.MoveNext())
            {
                return;
            }
                          
            SettableInfo[] si = en.Current.GetType().GetSettables();

            foreach (var x in si)
            {
                listView.Columns.Add( x.Name );
            }

            do
            {
                ListViewItem lvi = _CreateListViewItem( en.Current, si );

                listView.Items.Add( lvi );

            } while (en.MoveNext());  
        }

        public static T GetSelectedTag<T>( this ListView self )
            where T : class
        {
            return (T)GetSelectedItem( self )?.Tag;
        }

        public static ListViewItem GetSelectedItem( this ListView self )
        {
            return self.SelectedItems.Count != 1 ? null : self.SelectedItems[0];
        }

        private static ListViewItem _CreateListViewItem( object item, SettableInfo[] sis )
        {
            if (sis.Length == 0)
            {
                return new ListViewItem();
            }          

            ListViewItem lvi = new ListViewItem( sis[0].GetValue( item ).ToStringSafe() );

            for (int n = 1; n < sis.Length; n++)
            {
                lvi.SubItems.Add( sis[n].GetValue( item ).ToStringSafe() );
            }

            return lvi;
        }
    }
}
