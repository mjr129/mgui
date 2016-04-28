using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MGui
{
    public static class ColourHelper
    {
        private static Dictionary<int, string> __colourNames;

        public static Color ComplementaryColour( Color colour )
        {
            return colour.GetBrightness() > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Converts a colour to its name.
        /// </summary>                    
        public static string ColourToName( Color colour )
        {
            if (colour.IsNamedColor)
            {
                return colour.Name;
            }

            if (__colourNames == null)
            {
                __colourNames = new Dictionary<int, string>();

                foreach (KnownColor kc in Enum.GetValues( typeof( KnownColor ) ))
                {
                    Color c = Color.FromKnownColor( kc );
                    __colourNames[c.ToArgb()] = c.Name;
                }
            }

            string name;
            if (__colourNames.TryGetValue( colour.ToArgb(), out name ))
            {
                return name;
            }

            return colour.R.ToString() + ", " + colour.G + ", " + colour.B;
        }

        public static bool EditColor( ref Color colour )
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = colour;

                if (cd.ShowDialog() == DialogResult.OK)
                {
                    colour = cd.Color;
                    return true;
                }
            }

            return false;
        }
    }
}
