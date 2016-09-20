using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MGui.Controls;
using MGui.Helpers;

namespace MGui.Datatypes
{
    /// <summary>
    /// Manages a list of binders.
    /// </summary>
    public class BinderCollection : IEnumerable<Binder>
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
                Add( new BinderCtlColour() );
                Add( new BinderTextBoxArray() );
                Add( new BinderComboBoxBoolean() );
                Add( new BinderComboBoxEnum() );
                Add( new BinderConversion(this) );
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
                if (b.CanHandle( dataType ) )
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
                if (b.CanHandle( control, dataType ) )
                {
                    return b;
                }
            }

            throw new InvalidOperationException( "Cannot find a Binder between " + control.GetType().Name + " and " + dataType.Name );
        }

        public IEnumerator<Binder> GetEnumerator()
        {
            return ((IEnumerable<Binder>)this._all).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Binder>)this._all).GetEnumerator();
        }
    }

    /// <summary>
    /// Binders handle the marshalling of data to and from a control.
    /// Handles both existing controls as well as the capability of creating new controls.
    /// </summary>
    public abstract class Binder
    {
        /// <summary>
        /// Creates and returns a control suitable for use with this binder.
        /// </summary>
        /// <param name="dataType">
        /// Type of data the control is to expect
        /// Guaranteed to have passed a CanHandle(Type) check for this dataType.
        /// </param>
        /// <returns>The created control</returns>
        public abstract Control CreateControl( Type dataType );

        /// <summary>
        /// Gets a datatype valid for use with this binder.
        /// </summary>
        public abstract Type PreferredDataType { get; }

        /// <summary>
        /// Gets the value of the control
        /// </summary>
        /// <param name="control">The control to get the value for</param>
        /// <param name="dataType">
        /// The type of value expected.
        /// Guaranteed to be the same as was called for InitialiseControl on this control.
        /// </param>
        /// <returns>The value</returns>
        public abstract object Get( Control control, Type dataType );

        /// <summary>
        /// Sets the value of the control
        /// </summary>
        /// <param name="control">The control to set the value for</param>
        /// <param name="value">The value to set</param>
        /// <param name="dataType">
        /// The type of the value.
        /// Guaranteed to be the same as was called for InitialiseControl on this control.
        /// </param>
        public abstract void Set( Control control, object value, Type dataType );

        /// <summary>
        /// Returns if this binder can handle an existing control with data of the specified type.
        /// </summary>
        /// <param name="control">Existing control</param>
        /// <param name="dataType">Datatype expected</param>
        /// <returns>true if can handle, otherwise false</returns>
        public abstract bool CanHandle( Control control, Type dataType );

        /// <summary>
        /// Returns if this binder can handle the specified type if the binder can create its own control.
        /// </summary>
        /// <param name="dataType">Datatype expected</param>
        /// <returns>true if can handle, otherwise false</returns>
        public abstract bool CanHandle( Type dataType );

        /// <summary>
        /// Initialises the specified control (always called regardless of whether CreateControl is called)
        /// </summary>
        /// <param name="control">The control to initialise</param>
        /// <param name="dataType">
        /// The datatype to initialise for.
        /// If a new control was created this is guaranteed to be the same type passed to CanHandle(Type) and CreateControl.
        /// If an existing control is used this is guaranteed to be the same type passed to CanHandle(Control, Type).
        /// </param>
        public abstract void InitialiseControl( Control control, Type dataType );         
    }

    public class BinderConversion : Binder
    {
        private readonly BinderCollection _collection;

        private class Tup
        {
            public readonly Binder Binder;
            public readonly TypeConverter Converter;

            public Tup( TypeConverter converter, Binder binder )
            {
                this.Converter = converter;
                this.Binder = binder;
            }
        }

        public BinderConversion( BinderCollection collection)
        {
            _collection = collection;
        }

        public override Type PreferredDataType => null;

        public override bool CanHandle( Control control, Type dataType )
        {
            return CanHandle2( control, dataType ) != null;
        }

        public override bool CanHandle( Type dataType )
        {
            return CanHandle2( null, dataType ) != null;
        }

        private Tup CanHandle2( Control control, Type dataType)
        {
            TypeConverterAttribute attr = dataType.GetCustomAttribute<TypeConverterAttribute>();

            if (attr == null)
            {                 
                return null;
            }

            Type converterType = Type.GetType( attr.ConverterTypeName );

            if (converterType == null)
            {                 
                return null;
            }

            TypeConverter converter = (TypeConverter)Activator.CreateInstance( converterType );

            foreach (Binder binder in _collection)
            {
                if (binder.PreferredDataType != null
                    && ( control == null || binder.CanHandle( control, binder.PreferredDataType ) )
                    && converter.CanConvertFrom( binder.PreferredDataType )
                    && converter.CanConvertTo( binder.PreferredDataType ) )
                {
                    return new Tup( converter, binder );
                }
            }

            return null;
        }

        public override Control CreateControl( Type dataType )
        {
            // Create
            Tup b = CanHandle2( null, dataType );
            return b.Binder.CreateControl( b.Binder.PreferredDataType );
        }

        public override void InitialiseControl( Control control, Type dataType )
        {
            Tup b = CanHandle2( control, dataType );
            b.Binder.InitialiseControl( control, dataType );
        }

        public override object Get( Control control, Type dataType )
        {
            // Convert from binder's preferred type
            Tup b = CanHandle2( control, dataType );
            object value = b.Binder.Get( control, b.Binder.PreferredDataType );
            return b.Converter.ConvertFrom( value );
        }

        public override void Set( Control control, object value, Type dataType )
        {
            // Convert to binder's preferred type
            Tup b = CanHandle2( control, dataType );
            value = b.Converter.ConvertTo( value, b.Binder.PreferredDataType );
            b.Binder.Set( control, value, b.Binder.PreferredDataType );
        }
    }

    public abstract class Binder<TControl, TData> : Binder
        where TControl : Control, new()
    {
        private bool CanHandle( Control control )
        {
            return typeof( TControl ).IsAssignableFrom( control.GetType() );
        }

        public override bool CanHandle( Control control, Type dataType )
        {
            return CanHandle( control ) && CanHandle( dataType );
        }

        public override bool CanHandle( Type dataType )
        {
            return typeof( TData ).IsAssignableFrom( dataType );
        }

        public override Type PreferredDataType => typeof(TData);

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
            return new TControl();   
        }

        public override void InitialiseControl( Control control, Type dataType )
        {
            ConfigureControl( (TControl)control, dataType );
        }

        protected virtual void ConfigureControl( TControl control, Type dataType )
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

        public override Type PreferredDataType => typeof( string );
    }

    internal class BinderTextBoxArray : Binder<TextBox, Array>
    {
        public override bool CanHandle( Type dataType )
        {
            return dataType.IsArray 
                && dataType.GetArrayRank() == 1
                && typeof( IConvertible ).IsAssignableFrom( dataType.GetElementType() );
        }

        public override Type PreferredDataType => typeof( string[] );

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

    /// <summary>
    /// ENUM in COMBO BOX
    /// </summary>
    internal class BinderComboBoxEnum : Binder <ComboBox, object>
    {                   
        public override bool CanHandle( Type dataType )
        {
            return dataType.IsEnum;
        }

        protected override object GetValue( ComboBox control, Type dataType )
        {
            if (control.SelectedItem == null)
            {
                return Activator.CreateInstance( dataType );
            }

            return ((NamedValue<object>)control.SelectedItem).Value;
        }

        protected override void SetValue( ComboBox control, object value, Type dataType )
        {
            control.SelectedItem = value;
        }

        protected override void ConfigureControl( ComboBox control, Type dataType )
        {
            base.ConfigureControl( control, dataType );           

            Array values = dataType.GetEnumValues();

            foreach(object value in values)
            {                                        
                control.Items.Add( new NamedValue<object>( ((Enum)value).ToUiString(), value ) );
            }
        }
    }

    internal class BinderComboBoxBoolean : Binder<ComboBox, bool>
    {
        protected override bool GetValue( ComboBox control, Type dataType )
        {
            return control.SelectedIndex == 1;
        }

        protected override void SetValue( ComboBox control, bool value, Type dataType )
        {
            control.SelectedIndex = value ? 1 : 0;
        }

        protected override void ConfigureControl( ComboBox control, Type dataType )
        {
            base.ConfigureControl( control, dataType );

            control.Items.Add( "No" );
            control.Items.Add( "Yes" );
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

        protected override void ConfigureControl( NumericUpDown control, Type dataType )
        {   
            base.ConfigureControl( control, dataType );

            control.Minimum = decimal.MinValue; // todo: take account of specific dataType?
            control.Maximum = decimal.MaxValue;
        }
    }

    internal class BinderCtlColour : Binder<CtlColourEditor, Color>
    {
        protected override Color GetValue( CtlColourEditor control, Type dataType )
        {
            return control.SelectedColor;
        }

        protected override void SetValue( CtlColourEditor control, Color value, Type dataType )
        {
            control.SelectedColor = value;
        }
    }
}
