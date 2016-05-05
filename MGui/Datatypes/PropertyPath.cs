using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    public class PropertyPath<TSource, TDestination>
    {
        public delegate TDestination Property( TSource target );
        private readonly SettableInfo[] _properties;

        public PropertyPath( Expression<Property> property )
        {
            List<SettableInfo> properties = new List<SettableInfo>();

            var body = property.Body;

            while (!(body is ParameterExpression))
            {
                var bodyUe = body as UnaryExpression;

                if (bodyUe != null)
                {
                    MemberExpression memEx = bodyUe.Operand as MemberExpression;
                    properties.Add( SettableInfo.New(memEx.Member) );

                    body = memEx.Expression;
                    continue;
                }

                var bodyMe = body as MemberExpression;

                if (bodyMe != null)
                {
                    properties.Add( SettableInfo.New( bodyMe.Member) );

                    body = bodyMe.Expression;
                    continue;
                }
            }

            _properties = properties.Reverse<SettableInfo>().ToArray();
        }

        public PropertyPath( params SettableInfo[] pathInSequence )
        {
            _properties = pathInSequence;
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

        public SettableInfo Last
        {
            get
            {
                return _properties[_properties.Length - 1];
            }
        }

        public TDestination Get( TSource source )
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

            return (TDestination)target;
        }

        private object TryToCreateTarget( Type t )
        {
            return Activator.CreateInstance( t );
        }

        public void Set( TSource source, TDestination value )
        {
            object target = source;

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

        internal static PropertyPath<TSource,TDestination>[] ReflectAll( Type type )
        {
            List<PropertyPath<TSource, TDestination>> result = new List<PropertyPath<TSource, TDestination>>();

            foreach (SettableInfo settable in type.GetSettables())
            {
                result.Add( new PropertyPath<TSource, TDestination>( settable ) );
            }

            return result.ToArray();
        }
    }
}
