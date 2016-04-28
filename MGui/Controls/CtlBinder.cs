using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MGui.Datatypes;
using MGui.Helpers;

namespace MGui.Controls
{
    /// <summary>
    /// Binds controls to data.
    /// 
    /// See <see cref="BinderCollection"/> for how controls are bound.
    /// </summary>                                          
    public partial class CtlBinder<T> : Component
    {
        readonly Dictionary<Control, CtrlInfo> _properties = new Dictionary<Control, CtrlInfo>();
        private Control _revertButtonSelection;
        private T _target;
        private BinderCollection _binderCollection;

        [DefaultValue( true )]
        public bool GenerateRevertButtons { get; set; } = true;

        [DefaultValue( true )]
        public bool TestOnEdit { get; set; } = true;

        /// <summary>
        /// Gets or sets the BinderCollection.
        /// The default is <see cref="BinderCollection.Default"/>.
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public BinderCollection BinderCollection
        {
            get
            {
                if (_binderCollection == null)
                {
                    _binderCollection = BinderCollection.Default;
                }

                return _binderCollection;
            }             
            set
            {
                this._binderCollection = value;
            }
        }

        public CtlBinder()
            :this( null )
        {
            // NA
        }

        public class CtrlInfo
        {
            public CtlBinder<T> Owner;
            public PropertyPath<T, object> Path;
            public readonly Control Control;
                                  
            private object _originalValue;
            Binder _binder;

            public CtrlInfo( CtlBinder<T> owner, Control control, PropertyPath<T, object> path, Binder binder )
            {
                this.Owner = owner;
                this.Control = control;
                this.Path = path;
                this._binder = binder;
            }

            public T Target
            {
                get
                {
                    return Owner._target;
                }
            }

            public void UpdateOriginal( )
            {                           
                _originalValue = TargetValue;
            }                           

            public object OriginalValue
            {
                get
                {
                    return _originalValue;
                }
            }

            public object ControlValue
            {
                get
                {
                    return _binder.Get( Control, Path.Last.PropertyType );
                }
                set
                {
                    _binder.Set( Control, value, Path.Last.PropertyType );
                }
            }

            public object TargetValue
            {
                get
                {
                    return Path.Get( Target );
                }
                set
                {
                    Path.Set( Target, value );
                }
            }
        }

        public CtlBinder( IContainer container )
        {
            if (container != null)
            {
                container.Add( this );
            }

            InitializeComponent();

            _cmsRevertButton.Opening += _cmsRevertButton_Opening;
            _mnuSetToDefault.Click += _mnuSetToDefault_Click;
            _mnuUndoChanges.Click += _mnuUndoChanges_Click;
        }

        private void _cmsRevertButton_Opening( object sender, CancelEventArgs e )
        {
            _mnuSetToDefault.Visible = _properties[_revertButtonSelection].Path.HasDefaultValue;
        }

        private void _mnuUndoChanges_Click( object sender, EventArgs e )
        {
            var ctrlInfo = _properties[_revertButtonSelection];
            ctrlInfo.ControlValue = ctrlInfo.OriginalValue;
        }

        private void _mnuSetToDefault_Click( object sender, EventArgs e )
        {
            var ctrlInfo = _properties[_revertButtonSelection];
            ctrlInfo.ControlValue = ctrlInfo.Path.DefaultValue;
        }

        public void Bind( Control control, Expression<PropertyPath<T, object>.Property> property )
        {
            // Get property path
            var path = new PropertyPath<T, object>( property );
            var propType = path.Last.PropertyType;
            var binder = this.BinderCollection.FindSuitableBinder( control, path.Last.PropertyType );

            _properties.Add( control, new CtrlInfo( this, control, path, binder ) );

            // Track changes
            if (TestOnEdit)
            {
                TrackChanges( control );
            }

            // Create tooltops
            foreach (CtrlInfo ctrlInfo in _properties.Values)
            {
                StringBuilder sb = new StringBuilder();
                CategoryAttribute cat = ctrlInfo.Path.Last.GetCustomAttribute<CategoryAttribute>();
                DisplayNameAttribute namer = ctrlInfo.Path.Last.GetCustomAttribute<DisplayNameAttribute>();
                DescriptionAttribute desc = ctrlInfo.Path.Last.GetCustomAttribute<DescriptionAttribute>();

                if (cat != null)
                {
                    sb.Append( cat.Category.ToBold() + ": " );
                }

                if (namer != null)
                {
                    sb.Append( namer.DisplayName.ToBold() );
                }
                else
                {
                    sb.Append( property.Name.ToBold() );
                }

                if (desc != null)
                {
                    sb.AppendLine();
                    sb.Append( desc.Description );
                }

                toolTip1.SetToolTip( ctrlInfo.Control, sb.ToString() );
            }

            // Create reset button
            if (!(control is CheckBox) && GenerateRevertButtons)
            {
                Button resetButton = new Button
                {
                    Text = string.Empty,
                    Image = Properties.Resources.MnuUndo,
                    Size = new Size(control.Height, control.Height ),
                    Visible = true,
                    Tag = control,
                    Margin = control.Margin
                };

                TableLayoutPanel tlp = (TableLayoutPanel)control.Parent;
                tlp.Controls.Add( resetButton, tlp.ColumnCount - 1, tlp.GetRow( control ) );

                resetButton.Click += resetButton_Click;
            }

            control.MouseUp += Control_MouseUp;
        }

        private void Control_MouseUp( object sender, MouseEventArgs e )
        {
            if (e.Button == MouseButtons.Right)
            {
                _revertButtonSelection = (Control)sender;
                _cmsRevertButton.Show( _revertButtonSelection, e.Location );
            }
        }

        private void TrackChanges( Control control )
        {   
            if (control is TextBox)
            {
                ((TextBox)control).TextChanged += Control_SomethingChanged;
            }
            else if (control is CheckBox)
            {
                ((CheckBox)control).CheckedChanged += Control_SomethingChanged;
            }
            else if (control is RadioButton)
            {
                ((RadioButton)control).CheckedChanged += Control_SomethingChanged;
            }
            else if (control is NumericUpDown)
            {
                ((NumericUpDown)control).ValueChanged += Control_SomethingChanged;
            }
            else if (control is ComboBox)
            {
                ((ComboBox)control).SelectedIndexChanged += Control_SomethingChanged;
            }
            else if (control is Button)
            {
                ((Button)control).BackColorChanged += Control_SomethingChanged;
            }
        }

        private void Control_SomethingChanged( object sender, EventArgs e )
        {
            Control control = (Control)sender;
            var x = _properties[control];

            try
            {
                x.TargetValue = x.ControlValue;
                _errorProvider.Remove( control );
            }
            catch (Exception ex)
            {
                _errorProvider.Set( control, ex.Message );
            }
            finally
            {
                x.TargetValue = x.OriginalValue;
            }
        }

        private void resetButton_Click( object sender, EventArgs e )
        {
            Button resetButton = (Button)sender;

            _revertButtonSelection = (Control)resetButton.Tag;
            _cmsRevertButton.Show( resetButton, 0, resetButton.Height );
        }  

        /// <summary>
        /// Sets the new target and reads the data to the controls.
        /// </summary>                                             
        public void Read( T newTarget )
        {
            _target = newTarget;
            Read();       
        }

        /// <summary>
        /// Reads the data to the controls.
        /// </summary>
        private void Read()
        {
            foreach (var kvp in _properties)
            {
                kvp.Value.UpdateOriginal();
                kvp.Value.ControlValue = kvp.Value.TargetValue;
            }
        }

        /// <summary>
        /// Sets the new target and writes the data from the controls to the target.
        /// </summary>        
        public T Commit( T newTarget )
        {
            _target = newTarget;
            return Commit();
        }

        /// <summary>
        /// Commits the data from the controls to the target.
        /// </summary>                                             
        public T Commit()
        {
            foreach (var kvp in _properties)
            {
                kvp.Value.TargetValue = kvp.Value.ControlValue;
            }

            return _target;      
        }

        /// <summary>
        /// Changes the target without altering the controls.
        /// </summary>                                    
        public void SetTarget( T newTarget )
        {
            _target = newTarget;
        }

        /// <summary>
        /// Clears the target.
        /// Use to avoid accidentally overwriting data.
        /// </summary>                                    
        public void ClearTarget( )
        {
            _target = default( T );
        }
    }
}
