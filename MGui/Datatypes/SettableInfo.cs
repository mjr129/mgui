using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    /// <summary>
    /// Wraps PropertyInfo and FieldInfo into a common derived class with
    /// SetValue, GetValue and FieldType/PropertyType.
    /// </summary>
    public abstract class SettableInfo : MemberInfo
    {
        public abstract MemberInfo Member { get; }

        public override Type DeclaringType => Member.DeclaringType;

        /// <summary>
        /// Overrides MemberInfo to resturn MemberTypes.Custom.
        /// Returning the true type leads to <see cref="InvalidCastException"/>.
        /// </summary>
        public override MemberTypes MemberType => MemberTypes.Custom;

        public override string Name => Member.Name;

        public override Type ReflectedType => Member.ReflectedType;

        public abstract Type PropertyType { get; }

        public override object[] GetCustomAttributes( bool inherit ) => Member.GetCustomAttributes( inherit );

        public override object[] GetCustomAttributes( Type attributeType, bool inherit ) => Member.GetCustomAttributes( attributeType, inherit );

        public override bool IsDefined( Type attributeType, bool inherit ) => Member.IsDefined( attributeType, inherit ); 

        public abstract void SetValue( object target, object value );

        public abstract object GetValue( object target );

        public override string ToString()
        {
            return Member.ToString();
        }

        public static SettableInfo New( object x )
        {
            if (x is PropertyInfo)
            {
                return new Property( (PropertyInfo)x );
            }
            else if (x is FieldInfo)
            {
                return new Field( (FieldInfo)x );
            }
            else
            {
                throw new InvalidCastException( $"Cannot infer type of object \"{x}\" for SettableInfo." );
            }
        }

        private class Property : SettableInfo
        {
            private readonly PropertyInfo _member;

            public Property( PropertyInfo member )
            {
                _member = member;
            }

            public override MemberInfo Member => _member;

            public override Type PropertyType => _member.PropertyType;

            public override object GetValue( object target )
            {
                return _member.GetValue( target );
            }

            public override void SetValue( object target, object value )
            {
                _member.SetValue( target, value );
            }
        }

        private class Field : SettableInfo
        {
            private readonly FieldInfo _member;

            public Field( FieldInfo member )
            {
                _member = member;
            }

            public override MemberInfo Member => _member;

            public override Type PropertyType => _member.FieldType;

            public override object GetValue( object target )
            {
                return _member.GetValue( target );
            }

            public override void SetValue( object target, object value )
            {
                _member.SetValue( target, value );
            }
        }
    }

    public static class SettableInfoExtensions
    {
        public static SettableInfo[] GetSettables( this Type type )
        {
            var fields = type.GetFields().Select( z => SettableInfo.New( z ) );
            var properties = type.GetProperties().Select( z => SettableInfo.New( z ) );

            return fields.Concat( properties ).ToArray();
        }
    }
}
