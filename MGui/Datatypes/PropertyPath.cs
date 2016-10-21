using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MGui.Helpers;

namespace MGui.Datatypes
{
    public class PropertyPath
    {                                                     
        protected readonly SettableInfo[] _properties;

        public PropertyPath( IEnumerable<SettableInfo> properties )
        {
            _properties = properties.ToArray();
        }

        public PropertyPath( Expression<PropertyPath<object, object>.Property> property )
            : this( Decompose( property ) )
        {
            // NFA
        }

        public PropertyPath( params SettableInfo[] properties )
            : this( (IEnumerable<SettableInfo>)properties )
        {
            // NFA
        }

        public static IEnumerable<SettableInfo> Decompose<T, U>( Expression<PropertyPath<T, U>.Property> property )
        {
            List<SettableInfo> properties = new List<SettableInfo>();

            Expression body = property.Body;

            while (!(body is ParameterExpression))
            {
                var bodyUe = body as UnaryExpression;

                if (bodyUe != null)
                {
                    MemberExpression memEx = bodyUe.Operand as MemberExpression;
                    properties.Add( SettableInfo.New( memEx.Member ) );

                    body = memEx.Expression;
                    continue;
                }

                var bodyMe = body as MemberExpression;

                if (bodyMe != null)
                {
                    properties.Add( SettableInfo.New( bodyMe.Member ) );

                    body = bodyMe.Expression;
                    continue;
                }
            }

            return properties.Reverse<SettableInfo>();
        }

        public bool HasDefaultValue
        {
            get
            {
                return Last.GetCustomAttribute<DefaultValueAttribute>() != null;
            }
        }

        public object DefaultValue
        {
            get
            {
                var attr = Last.GetCustomAttribute<DefaultValueAttribute>();

                if (attr != null)
                {
                    return attr.Value;
                }

                return Last.GetType().IsValueType ? Activator.CreateInstance( Last.GetType() ) : null;
            }
        }

        public Type PropertyType => Last.PropertyType;

        public Type DeclaringType => First.DeclaringType;

        public SettableInfo First
        {
            get
            {
                return _properties[0];
            }
        }

        public SettableInfo Last
        {
            get
            {
                return _properties[_properties.Length - 1];
            }
        }

        public object Get( object source )
        {
            object target = source;

            for (int n = 0; n < _properties.Length; n++)
            {
                SettableInfo property = _properties[n];

                target = property.GetValue( target );

                if ((n != _properties.Length - 1) && target == null)
                {
                    target = TryToCreateTarget( property.PropertyType );
                }
            }

            return target;
        }

        private object TryToCreateTarget( Type t )
        {
            return Activator.CreateInstance( t );
        }

        public void Set( object target, object value )
        {                             
            for (int n = 0; n < _properties.Length; n++)
            {
                SettableInfo property = _properties[n];

                if (n == _properties.Length - 1)
                {
                    property.SetValue( target, value );
                }
                else
                {
                    object lastTarget = target;
                    target = property.GetValue( target );

                    if (target == null)
                    {
                        target = TryToCreateTarget( property.PropertyType );
                        property.SetValue( lastTarget, target );
                    }
                }
            }
        }

        /// <summary>
        /// Property path as a string.
        /// </summary>             
        public override string ToString()
        {
            return string.Join( ".", this._properties.Select( z => z.Name ) );
        }

        /// <summary>
        /// Same as <see cref="ToString"/> but uses <see cref="StringHelper.ToUiString"/>.
        /// </summary>                                                         
        public string ToUiString()
        {
            return string.Join( " → ", this._properties.Select( z => z.ToUiString() ) );
        }

        internal static PropertyPath[] ReflectAll(Type type)
        {
            List<PropertyPath> result = new List<PropertyPath>();

            foreach (SettableInfo settable in type.GetSettables())
            {
                result.Add( new PropertyPath( settable ) );
            }

            return result.ToArray();
        }

        internal static PropertyPath<T, object>[] ReflectAll<T>()
        {
            List<PropertyPath<T, object>> result = new List<PropertyPath<T, object>>();

            foreach (SettableInfo settable in typeof( T ).GetSettables())
            {
                result.Add( new PropertyPath<T, object>( settable ) );
            }

            return result.ToArray();
        }

        public PropertyPath Append( PropertyPath x )
        {
            return new PropertyPath( AppendInternal( x ) );
        }

        protected IEnumerable<SettableInfo> AppendInternal( PropertyPath x )
        {
            if (x.DeclaringType != this.PropertyType)
            {
                throw new InvalidOperationException( $"Cannot join a property path returning {{{this.PropertyType.Name}}} to one accepting {{{x.DeclaringType}}}." );
            }

            return this._properties.Concat( x._properties );
        }
    }

    public class PropertyPath<TSource, TDestination>     : PropertyPath
    {
        public delegate TDestination Property( TSource target );

        public PropertyPath<TSource, T> Append<T>( PropertyPath<TDestination, T> x )
        {                                                          
            return new PropertyPath<TSource, T>( AppendInternal( x ) );
        }

        public PropertyPath<TSource, T> Append<T>( Expression< PropertyPath<TDestination, T>.Property> x )
        {
            return new PropertyPath<TSource, T>( AppendInternal( new PropertyPath<TDestination, T>( x ) ) );
        }

        public PropertyPath( IEnumerable<SettableInfo> properties ) : base( properties )
        {
          // NFA
        }

        public PropertyPath( Expression<Property> property ) : base( PropertyPath.Decompose<TSource, TDestination>( property) )
        {
           // NFA
        }

        public PropertyPath( params SettableInfo[] properties ) : base( properties )
        {
            // NFA
        }                      

        public new TDestination Get( TSource target )
        {
            return (TDestination) base.Get( target );
        }

        public new void Set( TSource target, TDestination value )
        {
            base.Set( target, value );
        }
    }
}
