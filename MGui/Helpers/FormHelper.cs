using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui.Helpers
{
    public static class FormHelper
    {
        // Messages
        private const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// Enumerates all controls within [ctrl] of type [T].
        /// </summary>
        public static IEnumerable<Control> EnumerateControls( Control ctrl, Control ignore )
        {
            if (ctrl == ignore)
            {
                yield break;
            }

            foreach (Control ctrl2 in ctrl.Controls)
            {
                foreach (Control ctrl3 in EnumerateControls( ctrl2, ignore ))
                {
                    yield return ctrl3;
                }
            }

            yield return ctrl;
        }

        /// <summary>
        /// Enumerates all controls within [ctrl] of type [T].
        /// </summary>
        public static IEnumerable<T> EnumerateControls<T>( Control ctrl )
             where T : class
        {
            foreach (Control ctrl2 in ctrl.Controls)
            {
                foreach (T ctrl3 in EnumerateControls<T>( ctrl2 ))
                {
                    yield return ctrl3;
                }
            }

            T t = ctrl as T;

            if (t != null)
            {
                yield return t;
            }
        }

        /// <summary>
        /// Enumerates all controls within [ctrl] of type [T] and calls [action] on them.
        /// </summary>
        public static void EnumerateControls<T>( Control ctrl, Action<T> action )
            where T : class
        {
            foreach (Control ctrl2 in ctrl.Controls)
            {
                EnumerateControls<T>( ctrl2, action );
            }

            T t = ctrl as T;

            if (t != null)
            {
                action( t );
            }
        }

        public static void SuspendDrawingAndLayout( this Control control )
        {
            Message message = Message.Create( control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero );

            NativeWindow window = NativeWindow.FromHandle( control.Handle );
            window.DefWndProc( ref message );
        }

        public static void ResumeDrawingAndLayout( this Control control )
        {
            Message message = Message.Create( control.Handle, WM_SETREDRAW, new IntPtr( 1 ), IntPtr.Zero );

            NativeWindow window = NativeWindow.FromHandle( control.Handle );
            window.DefWndProc( ref message );

            control.Refresh();
        }
    }
}
