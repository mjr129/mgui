﻿using System;
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
using Binder = MGui.Datatypes.Binder;

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
        private T _target;
        private BinderCollection _binderCollection;                  

        [DefaultValue( true )]
        public bool TestOnEdit { get; set; } = true;

        /// <summary>
        /// Gets or sets the BinderCollection.
        /// The default is a copy of <see cref="BinderCollection.Default"/>.
        /// </summary>
        [Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public BinderCollection BinderCollection
        {
            get
            {
                if (_binderCollection == null)
                {
                    _binderCollection = new BinderCollection( true );
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

        public override string ToString()
        {
            return $"{typeof( CtlBinder<T> ).ToUiString()}: {_properties.Count} bindings";
        }

        public class CtrlInfo
        {
            public CtlBinder<T> Owner;
            public PropertyPath<T, object> Path;
            public readonly Control Control;
                                  
            private object _originalValue;
            private readonly Binder Binder;

            public CtrlInfo( CtlBinder<T> owner, Control control, PropertyPath<T, object> path, Binder binder )
            {
                this.Owner = owner;
                this.Control = control;
                this.Path = path;
                this.Binder = binder;
            }

            public override string ToString()
            {
                return $"Binds {{{Control.Name}}} to {{{Path}}}.";
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

            /// <summary>
            /// Gets or sets the value stored in the control.
            /// </summary>
            public object ControlValue
            {
                get
                {
                    return Binder.Get( Control, Path.Last.PropertyType );
                }
                set
                {
                    if (value!=null)
                    {
                        Debug.Assert( Path.Last.PropertyType.IsAssignableFrom( value.GetType() ), "Attempt to set a new value on a control which is not of the datatype the binder was intialised using." );
                    }

                    try
                    {
                        Binder.Set( Control, value, Path.Last.PropertyType );
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show( Control.FindForm(), ex.ToString() );
                    }
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

        /// <summary>
        /// Sets the value of a bound control.
        /// </summary>
        /// <remarks>
        /// Useful for cases where the user doesn't know how the binder is managing the data.
        /// For instance Color data is stored in the BackgroundColor of a button.
        /// </remarks>
        /// <param name="control">Control to set the value for</param>
        /// <param name="value">Value to set. Must match the type of the property or field bound to this control.</param>
        public void SetValue( Control control, object value )
        {
            this._properties[control].ControlValue = value;
        }

        /// <summary>
        /// Get the value of a bound control.
        /// </summary>
        /// <remarks>
        /// Useful for cases where the user doesn't know how the binder is managing the data.
        /// For instance Color data is stored in the BackgroundColor of a button.
        /// </remarks>
        /// <param name="control">Control to get the value for</param>
        public object GetValue( Control control )
        {
            return this._properties[control].ControlValue;
        }

        /// <summary>
        /// Get the value of a bound control as a value of the specified type.
        /// </summary>
        /// <remarks>
        /// Useful for cases where the user doesn't know how the binder is managing the data.
        /// For instance Color data is stored in the BackgroundColor of a button.
        /// </remarks>
        /// <param name="control">Control to get the value for</param>
        public TResult GetValue<TResult>( Control control )
        {
            return (TResult)this._properties[control].ControlValue;
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
            _mnuHelp.Click += _mnuHelp_Click;
        }

        private void _mnuHelp_Click( object sender, EventArgs e )
        {
            ContextMenuStrip menu = (ContextMenuStrip)((ToolStripMenuItem)sender).Owner;
            Control control = menu.SourceControl;
            MessageBox.Show( control.FindForm(), toolTip1.GetToolTip( control ), "Details", MessageBoxButtons.OK, MessageBoxIcon.Information );
        }

        private void _cmsRevertButton_Opening( object sender, CancelEventArgs e )
        {
            ContextMenuStrip menu = (ContextMenuStrip)sender;
            _mnuSetToDefault.Visible = _properties[menu.SourceControl].Path.HasDefaultValue;
        }

        private void _mnuUndoChanges_Click( object sender, EventArgs e )
        {
            ContextMenuStrip menu = (ContextMenuStrip)((ToolStripMenuItem)sender).Owner;
            var ctrlInfo = _properties[menu.SourceControl];
            ctrlInfo.ControlValue = ctrlInfo.OriginalValue;
        }

        private void _mnuSetToDefault_Click( object sender, EventArgs e )
        {
            ContextMenuStrip menu = (ContextMenuStrip)((ToolStripMenuItem)sender).Owner;
            var ctrlInfo = _properties[menu.SourceControl];
            ctrlInfo.ControlValue = ctrlInfo.Path.DefaultValue;
        }

        public TableLayoutPanel AutoBind( )
        {
            return AutoBind( PropertyPath.ReflectAll<T>() );
        }

        public TableLayoutPanel AutoBind( params Expression<PropertyPath<T, object>.Property>[] properties )
        {
            return AutoBind( properties.Select( z => new PropertyPath<T, object>( z ) ).ToArray() );
        }

        public TableLayoutPanel AutoBind( params PropertyPath<T, object>[] properties )
        {
            TableLayoutPanel container = new TableLayoutPanel()
            {
                Visible = true,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };

            container.ColumnStyles.Add( new ColumnStyle( SizeType.AutoSize, 100.0f ) );
            container.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 100.0f ) );
            container.ColumnStyles.Add( new ColumnStyle( SizeType.AutoSize, 100.0f ) );

            for (int n=0; n < properties.Length; n++)
            {
                PropertyPath<T, object> path = properties[n];
                Type type = path.Last.PropertyType;
                Binder binder = this.BinderCollection.FindSuitableBinderOrNull( type );

                if (binder == null)
                {
                    // Ignore since we are AUTO binding
                    continue;
                }

                Label label = new Label()
                {
                    Text = GetPropertyName( path ),
                    Visible = true,
                    AutoSize = true,
                };

                container.RowStyles.Add( new RowStyle( SizeType.AutoSize, 100.0f ) );
                container.Controls.Add( label, 0, n );
                           
                Control control = binder.CreateControl( type );
                control.Visible = true;
                container.Controls.Add( control, 1, n );

                Bind( control, path ); 
            }

            return container;
        }

        public void Bind( Control control, Expression<PropertyPath<T, object>.Property> property )
        {
            var path = new PropertyPath<T, object>( property );
            Bind( control, path );
        }

        /// <summary>
        /// Whether to call the binders to intiailse the controls (e.g. populate comboboxes with items)
        /// even when the control already exists.
        /// Use false if you have already populated the controls.
        /// </summary>
        public bool InitialiseControls { get; set; } = true;

        public void Bind( Control control, PropertyPath<T, object> path )
        {
            // Get property path                                       
            var propType = path.Last.PropertyType;
            var binder = this.BinderCollection.FindSuitableBinder( control, path.Last.PropertyType );

            if (InitialiseControls)
            {
                binder.InitialiseControl( control, path.Last.PropertyType );
            }                                       

            _properties.Add( control, new CtrlInfo( this, control, path, binder ) );

            // Track changes
            if (TestOnEdit)
            {
                TrackChanges( control );
            }

            // Create tooltops          
            StringBuilder toolTipText = new StringBuilder();
            CategoryAttribute category = path.Last.GetCustomAttribute<CategoryAttribute>();
            string description = path.Last.Member.ToDescription();

            if (category != null)
            {
                toolTipText.Append( category.Category.ToBold() + ": " );
            }

            toolTipText.Append( path.ToUiString().ToBold() );

            if (description != null)
            {
                toolTipText.AppendLine();
                toolTipText.Append( description );
            }

            toolTip1.SetToolTip( control, toolTipText.ToString() );

            control.ContextMenuStrip = _cmsRevertButton;
        }    

        private static string GetPropertyName( PropertyPath<T, object> path )
        {
            DisplayNameAttribute namer = path.Last.GetCustomAttribute<DisplayNameAttribute>();

            if (namer != null)
            {
                return( namer.DisplayName );
            }
            else
            {
                return( path.Last.Name );
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
            bool success = TryCommit();

            if (!success)
            {
                throw new InvalidOperationException( "Commit failed. There is at least one error." );
            }

            return _target;
        }

        public bool TryCommit()
        {
            this._errorProvider.Clear();
            bool success = true;

            foreach (var kvp in _properties)
            {
                object v;

                try
                {
                    v = kvp.Value.ControlValue;
                    kvp.Value.TargetValue = v;
                }
                catch (Exception ex)
                {
                    _errorProvider.Set( kvp.Key, ex.Message );
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Gets or sets the target without altering the controls.
        /// </summary>    
        public T Target
        {
            get  
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        public ToolTip ToolTipControl => toolTip1;

        [Obsolete("Use Target instead.")]                                
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
