using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Logger.Interfaces
{
    public interface ILogger
    {
        void Info(string origen, string message, [CallerMemberName]string sourceMember = "", [CallerLineNumber]int lineNumber = 0, [CallerFilePath]string filePath = "");
        void Warning(string origen, string message, [CallerMemberName]string sourceMember = "", [CallerLineNumber]int lineNumber = 0, [CallerFilePath]string filePath = "");
        void Error(string origen, string message = "", Exception ex = null, [CallerMemberName]string sourceMember = "", [CallerLineNumber]int lineNumber = 0, [CallerFilePath]string filePath = "", bool global = false, bool generateMessage = true);
    }
}
