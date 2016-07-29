using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Helpers
{
    public static class WeakReferenceHelper
    {
        public static WeakReference<T> ToWeakReference<T>(T self)
          where T : class
        {
            if (self == null)
            {
                return null;
            }
            else
            {
                return new WeakReference<T>(self);
            }
        }

        /// <summary>
        /// (MJR) Gets the target of a weak reference, or default(T) if it has expired.
        /// </summary>
        public static T GetTarget<T>(this WeakReference<T> self)
            where T : class
        {
            if (self == null)
            {
                return null;
            }

            T r;

            if (self.TryGetTarget(out r))
            {
                return r;
            }

            return null;
        }

        /// <summary>
        /// (MJR) Gets the target of a weak reference, or throws an exception if it has expired.
        /// </summary>
        public static T GetTargetOrThrow<T>(this WeakReference<T> self)
            where T : class
        {
            T r;

            if (self.TryGetTarget(out r))
            {
                return r;
            }

            throw new InvalidOperationException( "An attempt to retrieve an expired object was made. The object has been deleted or renamed since the reference was created. Please check the object still exists. Object type: \"" + typeof( T ).Name + "\"." );
        }

        public static string SafeToString<T>(this WeakReference<T> z, Converter<T, string> conversion)
         where T : class
        {
            T a = z.GetTarget();
            return (a == null) ? "〿" : conversion(a);
        }

        public static string SafeToString<T>(this WeakReference<T> z)
            where T : class
        {
            T a = z.GetTarget();
            return (a == null) ? "〿" : a.ToString();
        }

    }
}
