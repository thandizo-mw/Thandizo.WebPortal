using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thandizo.WebPortal.Helpers
{
    public class OutputHandler
    {
        //public OutputHandler();

        public bool IsErrorOccured { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public string ResponseCode { get; set; }
    }
}
