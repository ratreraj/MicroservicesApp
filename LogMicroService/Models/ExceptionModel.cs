using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogMicroService.Models
{
    public class ExceptionModel: Exception
    {
        public string ClassName { get; set; }
        public string Message { get; set; }
        public string StackTraceString { get; set; }
        public string Source { get; set; }
    }
}
