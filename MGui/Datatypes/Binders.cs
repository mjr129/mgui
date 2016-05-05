using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MGui.Datatypes;

namespace MGui
{
    /// <summary>
    /// Manages a list of binders.
    /// </summary>
    public class BinderCollection
    {
        private List<Binder> _all = new List<Binder>();
        public static BinderCollection Default = new BinderCollection( true );

        /// <summary>
        /// Constructs a new BinderCollection.
        /// </summary>
        public BinderCollection()
        {
            // NA
        }

        /// <summary>
        /// PRIVATE
        /// Constructs a new BinderCollection, populated with a set of defaults.
        /// </summary>
        public BinderCollection( bool populateWithDefaults )
        {
            if (populateWithDefaults)
            {
                Add( new BinderTextBox() );
                Add( new BinderTextBoxArray() );
                Add( new BinderCheckBox() );
                Add( new BinderRadioButton() );
                Add( new BinderNumericUpDown() );
            }
        }

        public void Add( Binder binder )
        {
            _all.Add( binder );
        }

        public Binder FindSuitableBinder( Type dataType )
        {
            foreach (Binder b in _all)
            {
                if (b.CanHandle( dataType ))
                {
                    return b;
                }
            }

            throw new InvalidOperationException( "Cannot find a Binder for " + dataType.Name );
        }

        public Binder FindSuitableBinder( Control control, Type dataType )
        {
            foreach (Binder b in _all)
            {
                if (b.CanHandle( control, dataType ))
                {
                    return b;
                }
            }

            throw new InvalidOperationException( "Cannot find a Binder between " + control.GetType().Name + " and " + dataType.Name );
        }
    }

    public abstract class Binder
    {
        public abstract Control CreateControl( Type dataType );
        public abstract bool CanHandle( Control control );
        public abstract bool CanHandle( Type dataType );
        public abstract object Get( Control control, Type dataType );
        public abstract void Set( Control control, object value, Type dataType );
        public bool CanHandle( Control control, Type dataType )
        {
            return CanHandle( control ) && CanHandle( dataType );
        }
    }

    public abstract class Binder<TControl, TData> : Binder
        where TControl : Control, new()
    {
        public override bool CanHandle( Control control )
        {
            return typeof( TControl ).IsAssignableFrom( control.GetType() );
        }

        public override bool CanHandle( Type dataType )
        {
             return typeof( TData ).IsAssignableFrom( dataType );
        }

        public override object Get( Control control, Type dataType )
        {
            return GetValue( (TControl)control, dataType );
        }

        protected abstract TData GetValue( TControl control, Type dataType );

        public override void Set( Control control, object value, Type dataType )
        {
            SetValue( (TControl)control, (TData)value, dataType );
        }

        public sealed override Control CreateControl( Type dataType )
        {
            TControl control = new TControl();   

            ConfigureControl( control );

            return control;
        }

        protected virtual void ConfigureControl( TControl control )
        {
            // No action
        }

        protected abstract void SetValue( TControl control, TData value, Type dataType );
    }

    internal class BinderTextBox : Binder<TextBox, IConvertible>
    {         
        protected override IConvertible GetValue( TextBox control, Type dataType )
        {
            return (IConvertible)Convert.ChangeType( control.Text, dataType );
        }

        protected override void SetValue( TextBox control, IConvertible value, Type dataType )
        {
            control.Text = Convert.ToString( value );
        }
    }

    internal class BinderTextBoxArray : Binder<TextBox, Array>
    {
        public override bool CanHandle( Type dataType )
        {
            return dataType.IsArray 
                && dataType.GetArrayRank() == 1
                && typeof( IConvertible ).IsAssignableFrom( dataType.GetElementType() );
        } 

        protected override Array GetValue( TextBox control, Type dataType )
        {
            Type dt = dataType.GetElementType();

            string[] fields = SpreadsheetReader.Default.ReadFields( control.Text );         

            Array result= Array.CreateInstance( dt, fields.Length );

            for (int n = 0; n < fields.Length; n++)
            {
                result.SetValue( Convert.ChangeType( fields[n], dt ), n );
            }

            return result;
        }

        protected override void SetValue( TextBox control, Array value, Type dataType )
        {
            control.Text = SpreadsheetReader.Default.WriteFields( value );
        }
    }

    internal class BinderCheckBox : Binder<CheckBox, IConvertible>
    {
        protected override IConvertible GetValue( CheckBox control, Type dataType )
        {
            return (IConvertible)Convert.ChangeType( control.Checked, dataType );
        }

        protected override void SetValue( CheckBox control, IConvertible value, Type dataType )
        {
            control.Checked = Convert.ToBoolean( value );
        }
    }

    internal class BinderRadioButton : Binder<RadioButton, IConvertible>
    {
        protected override IConvertible GetValue( RadioButton control, Type dataType )
        {
            return (IConvertible)Convert.ChangeType( control.Checked, dataType );
        }

        protected override void SetValue( RadioButton control, IConvertible value, Type dataType )
        {
            control.Checked = Convert.ToBoolean( value );
        }
    }

    internal class BinderNumericUpDown : Binder<NumericUpDown, IConvertible>
    {
        protected override IConvertible GetValue( NumericUpDown control, Type dataType )
        {
            return (IConvertible)Convert.ChangeType( control.Value, dataType );
        }

        protected override void SetValue( NumericUpDown control, IConvertible value, Type dataType )
        {
            control.Value = Convert.ToDecimal( value );
        }
    }
}
