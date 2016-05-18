using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGui.Datatypes
{
    public class SwitchException : Exception
    {
        public SwitchException()
            : base( $"This switch statement does not have a handler for the type presented. Please check the value and/or the switch." )
        {
        }

        public SwitchException( object switchValue ) 
            : base( $"This switch statement does not have a handler for the type presented: {switchValue}. Please check the value and/or the switch." )
        {
        }

        public SwitchException( string switchName, object switchValue )
            : base( $"The switch statement \"{switchName}\" does not have a handler for the type presented: {switchValue}. Please check the value and/or the switch." )
        {
        }                                                            
    }
}
