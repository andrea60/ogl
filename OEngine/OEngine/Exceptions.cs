using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL
{
    public class NameInUseException : Exception
    {
        public override string Message => "The specified name is already used by another first-level GameObject";
    }
}
