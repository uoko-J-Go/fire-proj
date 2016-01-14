using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.Infrastructure.Exception
{
    public sealed class TipInfoException : System.Exception
    {

        public TipInfoException(string tipMsg, object errorInfo = null) : base(tipMsg)
        {
            this.InitErrorInfo(errorInfo);
        }

        public TipInfoException(string tipMsg, System.Exception innerException, object errorInfo = null) : base(tipMsg, innerException)
        {
            this.InitErrorInfo(errorInfo);
        }

        private void InitErrorInfo(object errorInfo)
        {
            this.Data["TipForUI"] = errorInfo;
        }
    }
}
