using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Logging
{
    public sealed class LogUtil
    {
        private static LogWriter Writer;
        public LogUtil()
        {

        }

        private static void Init()
        {
            if (Writer == null)
            {
                Writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            }
        }

        public static void Write(object message)
        {
            Init();

            Writer.Write(message);
        }

        public static void Write(object message, IDictionary<string, object> properties)
        {
            Init();

            Writer.Write(message, properties);
        }

        public static void Write(object message, IEnumerable<string> categories)
        {
            Init();

            Writer.Write(message, categories);
        }

        public static void Write(object message, string category)
        {
            Init();

            Writer.Write(message, category);
        }

        public static void Write(object message, IEnumerable<string> categories, IDictionary<string, object> properties)
        {
            Init();

            Writer.Write(message, categories, properties);
        }

        public static void Write(object message, IEnumerable<string> categories, int priority)
        {
            Init();

            Writer.Write(message, categories, priority);
        }

        public static void Write(object message, string category, IDictionary<string, object> properties)
        {
            Init();

            Writer.Write(message, category, properties);
        }

        public static void Write(object message, string category, int priority)
        {
            Init();

            Writer.Write(message, category, priority);
        }

        public static void Write(object message, IEnumerable<string> categories, int priority, IDictionary<string, object> properties)
        {
            Init();

            Writer.Write(message, categories, priority, properties);
        }

        public static void Write(object message, IEnumerable<string> categories, int priority, int eventId)
        {
            Init();

            Writer.Write(message, categories, priority, eventId);
        }

        public static void Write(object message, string category, int priority, IDictionary<string, object> properties)
        {
            Init();

            Writer.Write(message, category, priority, properties);
        }

        public static void Write(object message, string category, int priority, int eventId)
        {
            Init();

            Writer.Write(message, category, priority, eventId);
        }

        public static void Write(object message, IEnumerable<string> categories, int priority, int eventId, TraceEventType severity)
        {
            Init();

            Writer.Write(message, categories, priority, eventId, severity);
        }

        public static void Write(object message, string category, int priority, int eventId, TraceEventType severity)
        {
            Init();

            Writer.Write(message, category, priority, eventId, severity);
        }

        public static void Write(object message, IEnumerable<string> categories, int priority, int eventId, TraceEventType severity, string title)
        {
            Init();

            Writer.Write(message, categories, priority, eventId, severity, title);
        }

        public static void Write(object message, string category, int priority, int eventId, TraceEventType severity, string title)
        {
            Init();

            Writer.Write(message, category, priority, eventId, severity, title);
        }

        public static void Write(object message, IEnumerable<string> categories, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties)
        {
            Init();

            Writer.Write(message, categories, priority, eventId, severity, title, properties);
        }

        public static void Write(object message, string category, int priority, int eventId, TraceEventType severity, string title, IDictionary<string, object> properties)
        {
            Init();

            Writer.Write(message, category, priority, eventId, severity, title, properties);
        }
    }
}
