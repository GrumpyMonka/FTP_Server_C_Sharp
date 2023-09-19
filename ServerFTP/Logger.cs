using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerFTP
{
    delegate void LoggerFunc ( string str );
    internal class Logger
    {  
        public static LoggerFunc loggerFunc { get; set; }
        public static void Log ( in string str )
        {
            loggerFunc?.Invoke( DateTime.Now + " :   " + str );
        }
    }
}
