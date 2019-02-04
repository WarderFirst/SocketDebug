using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkClasses
{
    [System.Serializable]
    public class DebugData
    {
        public string logString;
        public string stackTrace;
        public LogType type;
    }

    [System.Serializable]
    public enum LogType
    {
        // Summary:
        //     LogType used for Errors.
        Error = 0,
        //
        // Summary:
        //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
        Assert = 1,
        //
        // Summary:
        //     LogType used for Warnings.
        Warning = 2,
        //
        // Summary:
        //     LogType used for regular log messages.
        Log = 3,
        //
        // Summary:
        //     LogType used for Exceptions.
        Exception = 4
    }
}
