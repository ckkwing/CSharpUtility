using NLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class FolderDefine
    {
        static FolderDefine()
        {
            try
            {
                var exeAssembly = Assembly.GetEntryAssembly();
                if (exeAssembly != null)
                {
                    var productAttrs = exeAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                    var productAttr = productAttrs.FirstOrDefault() as AssemblyProductAttribute;
                    if (productAttr != null && !string.IsNullOrEmpty(productAttr.Product))
                        productName = productAttr.Product;
                }
            }
            catch (Exception ex)
            {
                LogHelper.UILogger.Debug("Unable to get product name from assembly", ex);
            }
        }

        private static string productName = "Media Center";
        public static string ProductName
        {
            get { return FolderDefine.productName; }
        }

        private static string companyName = "Eric";

        public static string CompanyName
        {
            get { return FolderDefine.companyName; }
        }

        private static string companyDataFolder;

        public static string CompanyDataFolder
        {
            get
            {
                if (string.IsNullOrEmpty(companyDataFolder))
                {
                    string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    companyDataFolder = Path.Combine(folder, CompanyName);
                    if (!Directory.Exists(companyDataFolder))
                    {
                        try
                        {
                            Directory.CreateDirectory(companyDataFolder);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.UILogger.Error("CompanyDataFolder: ", ex);
                            //if create roaming foler failed, using local folder instead
                            folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                            companyDataFolder = Path.Combine(folder, CompanyName);
                            if (!Directory.Exists(companyDataFolder))
                            {
                                Directory.CreateDirectory(companyDataFolder);
                            }
                        }
                    }
                }

                return companyDataFolder;
            }
        }

        private static string userDataFolder;

        public static string UserDataFolder
        {
            get
            {
                if (string.IsNullOrEmpty(userDataFolder))
                {
                    string folder = CompanyDataFolder;
                    userDataFolder = Path.Combine(folder, ProductName);
                    if (!Directory.Exists(userDataFolder))
                    {
                        Directory.CreateDirectory(userDataFolder);
                    }
                }

                return userDataFolder;
            }
        }

        private static string databaseFolder;

        public static string DatabaseFolder
        {
            get
            {
                if (string.IsNullOrEmpty(databaseFolder))
                {
                    databaseFolder = Path.Combine(UserDataFolder, "Database");
                    if (!Directory.Exists(databaseFolder))
                    {
                        Directory.CreateDirectory(databaseFolder);
                    }
                }
                return databaseFolder;
            }
        }

        private static string tempFolder;

        public static string TempFolder
        {
            get
            {
                if (string.IsNullOrEmpty(tempFolder))
                {
                    tempFolder = GetTempFolder(true);
                }
                return tempFolder;
            }
        }

        private static string GetTempFolder(bool isCreate)
        {
            string tempFolder = Path.Combine(UserDataFolder, "temp");
            if (isCreate)
            {
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
            }
            return tempFolder;
        }

        public static void CleanTempFolder()
        {
            string tmpFolder = GetTempFolder(false);
            if (Directory.Exists(tmpFolder))
            {
                Directory.Delete(tmpFolder, true);
            }
        }
    }
}
