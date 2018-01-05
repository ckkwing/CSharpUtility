using NLogger;
using System;
using System.Text;

namespace NLogger
{
    class LogImpl:ILog
    {
        private string name;
        private enumLogLevel logLevel;
        private NAdvLogWrapper advLog;
        private bool isReleased = false;

        public LogImpl(string name)
        {
            this.name = name;
            advLog = new NAdvLogWrapper(name);
            this.logLevel = advLog.GetLogLevel();
        }
        
        public void Release()
        {
            isReleased = true;
            advLog.Dispose();
            advLog = null;
            logLevel = enumLogLevel.Off;
        }

        private string GetExceptionFormat(object message, Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append(message);
            if (ex != null)
            {
                msg.Append(", Exception.Message:");
                msg.Append(ex.Message);
                msg.Append("\n==Exception.StackTrace:\n");
                msg.Append(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    msg.Append("\n==InnerException.Message:");
                    msg.Append(ex.InnerException.Message);
                    msg.Append("\n==InnerException.StackTrace:\n");
                    msg.Append(ex.InnerException.StackTrace);
                }
            }
            return msg.ToString();
        }

        #region ILog Members

        public bool IsDebugEnabled
        {
            get { return (this.logLevel <= enumLogLevel.Debug); }
        }

        public bool IsInfoEnabled
        {
            get { return (this.logLevel <= enumLogLevel.Info); }
        }

        public bool IsWarnEnabled
        {
            get { return (this.logLevel <= enumLogLevel.Warn); }
        }

        public bool IsErrorEnabled
        {
            get { return (this.logLevel <= enumLogLevel.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return (this.logLevel <= enumLogLevel.Fatal); }
        }

        public void Debug(object message)
        {
            if (!isReleased && IsDebugEnabled && message != null)
            {
                advLog.Debug(message.ToString());
            }
        }

        public void Debug(object message, Exception exception)
        {
            if (!isReleased && IsDebugEnabled && message != null)
            {
                string msg = GetExceptionFormat(message, exception);
                advLog.Debug(msg);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (!isReleased && IsDebugEnabled)
            {
                string message = string.Format(format, args);
                advLog.Debug(message);
            }
        }

        public void Info(object message)
        {
            if (!isReleased && IsInfoEnabled && message != null)
            {
                advLog.Info(message.ToString());
            }
        }

        public void Info(object message, Exception exception)
        {
            if (!isReleased && IsInfoEnabled && message != null)
            {
                string msg = GetExceptionFormat(message, exception);
                advLog.Info(msg);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (!isReleased && IsInfoEnabled)
            {
                string message = string.Format(format, args);
                advLog.Info(message);
            }
        }

        public void Warn(object message)
        {
            if (!isReleased && IsWarnEnabled && message != null)
            {
                advLog.Warn(message.ToString());
            }
        }

        public void Warn(object message, Exception exception)
        {
            if (!isReleased && IsWarnEnabled && message != null)
            {
                string msg = GetExceptionFormat(message, exception);
                advLog.Warn(msg);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (!isReleased && IsWarnEnabled)
            {
                string message = string.Format(format, args);
                advLog.Warn(message);
            }
        }

        public void Error(object message)
        {
            if (!isReleased && IsErrorEnabled && message != null)
            {
                advLog.Error(message.ToString());
            }
        }

        public void Error(object message, Exception exception)
        {
            if (!isReleased && IsErrorEnabled && message != null)
            {
                string msg = GetExceptionFormat(message, exception);
                advLog.Error(msg);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (!isReleased && IsErrorEnabled)
            {
                string message = string.Format(format, args);
                advLog.Error(message);
            }
        }

        public void Fatal(object message)
        {
            if (!isReleased && IsFatalEnabled && message != null)
            {
                advLog.Fatal(message.ToString());
            }
        }

        public void Fatal(object message, Exception exception)
        {
            if (!isReleased && IsFatalEnabled && message != null)
            {
                string msg = GetExceptionFormat(message, exception);
                advLog.Fatal(msg);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (!isReleased && IsFatalEnabled)
            {
                string message = string.Format(format, args);
                advLog.Fatal(message);
            }
        }

        public void Log(LogType logType, object message)
        {
            if (!isReleased)
            {
                advLog.Log(logType, message.ToString());
            }
        }

        #endregion
    }
}
