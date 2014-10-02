using System;

namespace SAMRT.Common
{
    public class HandledException : Exception
    {
        public HandledException(string message, params object[] args) : base(string.Format(message, args))
        {
        }
    }
}
