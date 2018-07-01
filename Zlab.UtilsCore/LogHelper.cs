using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;

namespace Zlab.UtilsCore
{
    public static class LogHelper
    {
        private static readonly ILog Loginfo;// = LogManager.GetLogger("loginfo");
        private static readonly ILog Logerror;// = LogManager.GetLogger("logerror");
        static LogHelper()
        {
            var repository = LogManager.CreateRepository("NETCoreRepository");
            var fileInfo = new FileInfo("log4net.config");
            XmlConfigurator.Configure(repository, fileInfo);
            BasicConfigurator.Configure(repository);
            Loginfo = LogManager.GetLogger(repository.Name, "loginfo");
            Logerror = LogManager.GetLogger(repository.Name, "logerror");
        }
        public static void Log(string info)
        {
            if (Loginfo.IsInfoEnabled)
            {
                Loginfo.Info(info);
            }
        } 

        /// <summary>
        /// 错误记录
        /// </summary>
        /// <param name="info">附加信息</param>
        /// <param name="ex">错误</param>
        public static void Error(string info, Exception ex)
        {
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(ex);
            }

            if (!string.IsNullOrEmpty(info) && ex == null)
            {
                Logerror.ErrorFormat("【附加信息】 : {0}\r\n", new object[] { info });
            }
            else if (!string.IsNullOrEmpty(info) && ex != null)
            {
                string errorMsg = BeautyErrorMsg(ex);
                Logerror.ErrorFormat("【附加信息】 : {0}\r\n{1}", new object[] { info, errorMsg });
            }
            else if (string.IsNullOrEmpty(info) && ex != null)
            {
                string errorMsg = BeautyErrorMsg(ex);
                Logerror.Error(errorMsg);
            }
        }

        public static void Error(Exception ex)
        {
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(ex);
            }
        }
        /// <summary>
        /// 美化错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>错误信息</returns>
        private static string BeautyErrorMsg(Exception ex)
        {
            return string.Format("【异常类型】：{0} \r\n【异常信息】：{1} \r\n【堆栈调用】：{2}", new object[] { ex.GetType().Name, ex.Message, ex.StackTrace });
        }
    }
}
