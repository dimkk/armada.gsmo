using Microsoft.SharePoint.Administration;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ITB.SP.Tools
{
    public class Log
    {
        public static string Event(string category, TraceSeverity level, EventSeverity type, string message, params object[] data)
        {
            SPDiagnosticsService.Local.WriteEvent(0,
                            new SPDiagnosticsCategory(category, level, type),
                            type,
                            message,
                            data);
            return string.Format(message, data);
        }

        public static string Event(string category, TraceSeverity level, EventSeverity type, Exception e, string message, params object[] data)
        {
            string resultMessage;

            if (e is AggregateException)
                resultMessage = (e as AggregateException).ToString(message);
            else
                resultMessage = new Exception(message, e).ToString();

            return Event(category, level, type, resultMessage, data);
        }

        public static string Error(string category, string message, params object[] data)
        {
            return Event(category, TraceSeverity.High, EventSeverity.Error, message, data);
        }

        public static string Error(string category, Exception e, string message, params object[] data)
        {
            return Event(category, TraceSeverity.High, EventSeverity.Error, e, message, data);
        }

        public static string Error(Exception e, string message, params object[] data)
        {
            MethodBase method = new StackFrame(1, true).GetMethod();
            string category = method.ReflectedType.FullName;

            return Error(category, e, message, data);
        }

        public static string Error(string message, params object[] data)
        {
            MethodBase method = new StackFrame(1, true).GetMethod();
            string category = method.ReflectedType.FullName;

            return Error(category, message, data);
        }
        
        public static string Unexpected(string category, string message, params object[] data)
        {
            return Event(category, TraceSeverity.Unexpected, EventSeverity.Error, message, data);
        }

        public static string Unexpected(string category, Exception e, string message, params object[] data)
        {
            return Event(category, TraceSeverity.Unexpected, EventSeverity.Error, e, message, data);
        }

        public static string Unexpected(Exception e, string message, params object[] data)
        {
            MethodBase method = new StackFrame(1, true).GetMethod();
            string category = method.ReflectedType.FullName;

            return Unexpected(category, e, message, data);
        }
    }
}
