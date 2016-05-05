using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            int b = (colour.R * 2 + colour.G * 2 + colour.B ) / 5;

            return b > 128 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Converts a colour to its name.
        /// </summary>                    
        public static string ColourToName( Color colour, bool useClosest = false )
        {
            if (colour.IsNamedColor)
            {
                return colour.Name;
            }

            if (__colourNames == null)
            {
                __colourNames = new Dictionary<int, string>();

                foreach (PropertyInfo kc in typeof( Color ).GetProperties( BindingFlags.Static | BindingFlags.Public ))
                {
                    Color c = (Color)kc.GetValue( null );
                    __colourNames[c.ToArgb()] = c.Name;
                }
            }

            string name;
            if (__colourNames.TryGetValue( colour.ToArgb(), out name ))
            {
                return name;
            }

            if (useClosest)
            {
                int cd = int.MaxValue;
                string cname = null;

                foreach (var value in __colourNames)
                {
                    Color c = Color.FromArgb( value.Key );
                    int rd = (c.R - colour.R);
                    int gd = (c.G - colour.G);
                    int bd = (c.B - colour.B);

                    int dist = rd * rd + gd * gd + bd * bd;

                    if (dist < cd)
                    {
                        cname = value.Value;
                        cd = dist;
                    }
                }

                return cname + "ish";
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
