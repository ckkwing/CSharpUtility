using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class FileUtility
    {
        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>null == there is no file to delete, true == file has been deleted, false == delete file failed</returns>
        public static bool? DeleteFile(string fileName)
        {
            bool? isFileDeleted = null;
            if (string.IsNullOrEmpty(fileName))
                return isFileDeleted;
            if (!File.Exists(fileName))
            {
                return isFileDeleted;
            }

            try
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
                isFileDeleted = true;
            }
            catch (Exception e)
            {
                NLogger.LogHelper.UILogger.DebugFormat("DeleteFile exception: {0}", e.Message);
                isFileDeleted = false;
            }

            return isFileDeleted;
        }

        public static void ForceDeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            try
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
            }
            catch (Exception e)
            {
                NLogger.LogHelper.UILogger.DebugFormat("ForceDeleteFile exception: {0}", e.Message);
            }
        }

        public static void RetryDeleteFile(string fileName, int retryCount = 3)
        {
            int count = 0;
            while (count < retryCount)
            {
                if (true != FileUtility.DeleteFile(fileName))
                    count++;
                else
                    break;
            }
        }
    }
}
