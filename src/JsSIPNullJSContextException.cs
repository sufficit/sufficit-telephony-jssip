using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPNullJSContextException : Exception
    {
        public override string Message => "Null Javascript Context Exception";
    }
}
