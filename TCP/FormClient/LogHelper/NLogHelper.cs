using NLog;
using System.Reflection;

namespace LogHelper
{
    /// <summary>
    /// logger.Debug("调试");
    /// logger.Info("信息");
    /// logger.Warn("警告");
    /// logger.Error("错误");
    /// logger.Fatal("重大错误");
    /// </summary>
    public static class NLogHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Error(string msg)
        {
            logger.Error(msg);
        }

        public static void Fatal(string msg)
        {
            logger.Fatal(msg);
        }

        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        public static void Debug(string msg)
        {
            logger.Debug(msg);
        }
    }
}
