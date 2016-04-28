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
                Add( new BinderCheckBox() );
                Add( new BinderRadioButton() );
                Add( new BinderNumericUpDown() );
            }
        }

        public void Add( Binder binder )
        {
            _all.Add( binder );
        }

        public Binder FindSuitableBinder( object control, Type dataType )
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
        public abstract bool CanHandle( object control, Type dataType );
        public abstract object Get( object control, Type dataType );
        public abstract void Set( object control, object value, Type dataType );
    }

    public abstract class Binder<TControl, TData> : Binder
    {
        public override bool CanHandle( object control, Type dataType )
        {
            return typeof( TControl ).IsAssignableFrom( control.GetType() ) && typeof( TData ).IsAssignableFrom( dataType );
        }

        public override object Get( object control, Type dataType )
        {
            return Get( (TControl)control, dataType );
        }

        protected abstract TData GetValue( TControl control, Type dataType );

        public override void Set( object control, object value, Type dataType )
        {
            SetValue( (TControl)control, (TData)value, dataType );
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
        protected override Array GetValue( TextBox control, Type dataType )
        {
            Type dt = dataType.GetElementType();
            return Spreadsheet.ReadFields( control.Text ).Select( z => (IConvertible)Convert.ChangeType( z, dt ) ).ToArray();
        }

        protected override void SetValue( TextBox control, Array value, Type dataType )
        {
            control.Text = Spreadsheet.WriteFields( value );
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
