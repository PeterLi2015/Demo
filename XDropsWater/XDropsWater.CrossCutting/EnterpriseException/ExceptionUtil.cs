using XDropsWater.CrossCutting.Logging;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.EnterpriseException
{
    public class ExceptionUtil
    {
        private static readonly bool _IsTrackException = Convert.ToBoolean(ConfigurationManager.AppSettings["ExceptionIsTracking"]);
        private static readonly string _ExceptionPolicy = ConfigurationManager.AppSettings["ExceptionPolicy"];

        private ExceptionUtil()
        {

        }

        public static bool HandleException(Exception exceptionToHandle)
        {
            TrackException(exceptionToHandle);

            return ExceptionPolicy.HandleException(exceptionToHandle, _ExceptionPolicy);
        }

        public static bool HandleException(Exception exceptionToHandle, out Exception exceptionToThrow)
        {
            TrackException(exceptionToHandle);

            return ExceptionPolicy.HandleException(exceptionToHandle, _ExceptionPolicy, out exceptionToThrow);
        }

        private static void TrackException(Exception exceptionToHandle)
        {
            if (_IsTrackException)
            {
                LogUtil.Write(string.Format("ExceptionMessage: {0}, InnerExceptionMessage: {1}.",
                    exceptionToHandle.Message,
                    exceptionToHandle.InnerException == null ? string.Empty : exceptionToHandle.InnerException.Message));
            }
        }
    }
}
