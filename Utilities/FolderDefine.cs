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
        public static FolderDefine Instance
        {
            get { return Nested.instance; }
        }

        class Nested
        {
            static Nested() { }
            internal static readonly FolderDefine instance = new FolderDefine();
        }

        FolderDefine()
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

        public void Init(string companyName, string productName)
        {
            this.companyName = companyName;
            if (!string.IsNullOrEmpty(productName))
            {
                this.productName = productName;
            }
        }

        public void Uninit()
        {
        }
        

        private string productName = string.Empty;
        public string ProductName
        {
            get { return this.productName; }
        }

        private string companyName = string.Empty;

        public string CompanyName
        {
            get { return this.companyName; }
        }

        private string companyDataFolder;

        public string CompanyDataFolder
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

        private string userDataFolder;

        public string UserDataFolder
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

        private string databaseFolder;

        public string DatabaseFolder
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

        private string tempFolder;

        public string TempFolder
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

        private string GetTempFolder(bool isCreate)
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

        public void CleanTempFolder()
        {
            string tmpFolder = GetTempFolder(false);
            if (Directory.Exists(tmpFolder))
            {
                Directory.Delete(tmpFolder, true);
            }
        }
    }
}
