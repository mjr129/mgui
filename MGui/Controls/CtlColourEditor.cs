using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;          

namespace MGui.Controls
{
    /// <summary>
    /// A button that allows the user to change the colour.
    /// The colour is get and set using <see cref="CtlColourEditor.SelectedColor"/>.
    /// </summary>
    public class CtlColourEditor : Button
    {
        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public CtlColourEditor()
        {
            UpdateAppearance();
        }

        /// <summary>
        /// Gets or sets the selected colour.
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                UpdateAppearance();
            }
        }

        /// <summary>
        /// This should really be hidden but wraps to <see cref="SelectedColor"/> for legacy reasons.
        /// </summary>
        [EditorBrowsable( EditorBrowsableState.Never ), Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public override Color BackColor
        {   
            get
            {
                return this.SelectedColor;
            } 
            set
            {
                this.SelectedColor = value;
            }
        }

        /// <summary>
        /// Do not use.
        /// This property is controled automatically.
        /// </summary>
        [EditorBrowsable( EditorBrowsableState.Never ), Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public override string Text
        {
            get { return base.Text; }
            set { /* N/A */ }
        }

        /// <summary>
        /// Do not use.
        /// This property is controled automatically.
        /// </summary>
        [EditorBrowsable( EditorBrowsableState.Never ), Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public override Color ForeColor
        {
            get { return base.ForeColor; }     
            set { /* N/A */ }
        }


        /// <summary>
        /// Do not use.
        /// This property is controled automatically.
        /// </summary>
        [EditorBrowsable( EditorBrowsableState.Never ), Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public new Size Size
        {
            get { return base.Size; }
            set { /* N/A */ }
        }

        /// <summary>
        /// Updates the fixed properties of the colour editor.
        /// </summary>
        private void UpdateAppearance()
        {
            base.ForeColor = ColourHelper.ComplementaryColour( base.BackColor );
            base.Text = ColourHelper.ColourToName( base.BackColor );
            base.Size = new Size( 128, 29 );
        }

        /// <summary>
        /// Shows the colour chooser when the button is clicked.
        /// </summary>                                          
        protected override void OnClick( EventArgs e )
        {                               
            Color colour = BackColor;

            if (ColourHelper.EditColor( ref colour ))
            {
                BackColor = colour;
            }                                                 
        }
    }
}
