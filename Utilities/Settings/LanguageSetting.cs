using Microsoft.Win32;
using NLogger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Utilities.Settings
{
    public class LanguageSetting
    {
        private const string KEY_LANGUAGE = "Language";

        private static CultureInfo _appCulture;

        public static CultureInfo GetAppCulture()
        {
            if (_appCulture == null)
            {
                LoadLanguage();
            }
            return _appCulture;
        }

        public static void SetAppCulture(int cultureCode)
        {
            RegistryKey hkcu = Registry.CurrentUser;
            try
            {
                RegistryKey hkcuBIU = hkcu.CreateSubKey(StringConstant.REGISTRY_KEY_MEDIACENTER);
                if (hkcuBIU != null)
                {
                    hkcuBIU.SetValue(KEY_LANGUAGE, cultureCode, RegistryValueKind.DWord);
                    hkcuBIU.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.UILogger.Warn("Set App Culture Setting Exception", ex);
            }
            finally
            {
                if (null != hkcu)
                    hkcu.Close();
            }
        }

        private static RegistryKey OpenSubKey(RegistryKey key1, string keyName)
        {
            if (key1 == null)
            {
                return null;
            }

            RegistryKey key2 = null;
            string[] names = keyName.Split('\\');
            foreach (string name in names)
            {
                key2 = key1.OpenSubKey(name);
                key1 = key2;
                if (key2 == null)
                {
                    key2 = null;
                    break;
                }
            }

            return key2;
        }

        private static IList<int> _supportedLanguageCodes = new List<int>()
        {
            0x0804,//Chinese (Simplified)
            0x0404,//Chinese (Traditional)
            //0x0405,//Czech
            //0x0406,//Danish
            0x0413,//Dutch
            0x0409,//English (United States)
            //0x040B,//Finnish
            0x040C,//French (Standard)
            0x0407,//German
            //0x0408,//Greek
            //0x040E,//Hungarian
            0x0410,//Italian
            0x0411,//Japanese
            0x0412,//Korean
            //0x0414,//Norwegian
            0x0415,//Polish
            //0x0816,//Portuguese (Standard)
            0x0416,//Portuguese (Brazilian)
            0x0419,//Russian
            0x0C0A,//Spanish
            //0x041D,//Swedish
            //0x041E,//Thai
            0x041F,//Turkish
        };

        public static IList<int> SupportedLanguageCodes
        {
            get { return _supportedLanguageCodes; }
            private set { _supportedLanguageCodes = value; }
        }

        private static void LoadLanguage()
        {
            LogHelper.UILogger.Debug("LoadLanguage");
            try
            {
                int cultureID = 0;
                RegistryKey key = OpenSubKey(Registry.CurrentUser, StringConstant.REGISTRY_KEY_MEDIACENTER);
                if (key != null)
                {
                    cultureID = Convert.ToInt32(key.GetValue(KEY_LANGUAGE, 0));
                    key.Close();
                }

                if (cultureID == 0)
                {
                    key = OpenSubKey(Registry.LocalMachine, StringConstant.REGISTRY_KEY_MEDIACENTER);
                    if (key != null)
                    {
                        cultureID = Convert.ToInt32(key.GetValue(KEY_LANGUAGE, 0));
                        key.Close();
                    }
                }

                if (!SupportedLanguageCodes.Contains(cultureID))
                {
                    cultureID = System.Globalization.CultureInfo.CurrentUICulture.LCID;
                    if (!SupportedLanguageCodes.Contains(cultureID))
                    {
                        cultureID = 1033;
                    }
                }

                if (cultureID == 0)
                {
                    cultureID = 1033;
                }
                _appCulture = new System.Globalization.CultureInfo(cultureID);
            }
            catch (System.Exception ex)
            {
                int xcultureID = System.Globalization.CultureInfo.CurrentUICulture.LCID;
                if (!SupportedLanguageCodes.Contains(xcultureID))
                {
                    xcultureID = 1033;
                }
                _appCulture = new System.Globalization.CultureInfo(xcultureID);
                LogHelper.UILogger.Warn("Load Language Exception", ex);
            }

            try
            {
                LogHelper.UILogger.InfoFormat("Application Version: [{0}]", Utility.GetProductVersion());
                LogHelper.UILogger.InfoFormat("Application Culture, LCID:{0}", _appCulture.LCID);
                LogHelper.UILogger.InfoFormat("Application Culture, Name:{0}", _appCulture.Name);
                LogHelper.UILogger.InfoFormat("Application Culture, NativeName:{0}", _appCulture.NativeName);
                LogHelper.UILogger.InfoFormat("Application Culture, EnglishName:{0}", _appCulture.EnglishName);
                LogHelper.UILogger.InfoFormat("Application Culture, DisplayName:{0}", _appCulture.DisplayName);
                LogHelper.UILogger.InfoFormat("Application Culture, ThreeLetterWindowsLanguageName:{0}", _appCulture.ThreeLetterWindowsLanguageName);
            }
            catch (Exception ex)
            {
                LogHelper.UILogger.Info("Output Languagelog failed:", ex);
            }
        }

        public static ResourceDictionary LoadLocalizedCommonDictionary()
        {
            LogHelper.UILogger.Debug("LoadLocalizedCommonDictionary Start");
            ResourceDictionary lanRes = new ResourceDictionary() { Source = new Uri("/LocalizedResources/CommonResource.xaml", UriKind.Relative) };
            CultureInfo curCluture = GetAppCulture();
            string LanResPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalizedResources");
            string en_USResFile = System.IO.Path.Combine(LanResPath, "CommonResource_en-US.xaml");

            string resFile = String.Format("CommonResource_{0}.xaml", curCluture.IetfLanguageTag);
            string curLanRes = System.IO.Path.Combine(LanResPath, resFile);
            ResourceDictionary localizedlanRes = null;
            LogHelper.UILogger.InfoFormat("try to load CommonResource file:{0}", curLanRes);
            if (File.Exists(curLanRes))
            {
                try
                {
                    using (var fs = new FileStream(curLanRes, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Read in an EnhancedResourceDictionary File or preferably an GlobalizationResourceDictionary file
                        localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.UILogger.Error("load CommonResource file Failed", ex);
                }
            }

            if (localizedlanRes == null)
            {
                LogHelper.UILogger.DebugFormat("try to load CommonResource file:{0}", en_USResFile);
                if (File.Exists(en_USResFile))
                {
                    try
                    {
                        using (var fs = new FileStream(en_USResFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.UILogger.Error("load CommonResource file Failed", ex);
                    }
                }
            }

            if (localizedlanRes != null)
            {
                foreach (Object dicKey in localizedlanRes.Keys)
                {
                    if (lanRes.Contains(dicKey))
                    {
                        lanRes[dicKey] = localizedlanRes[dicKey];
                    }
                }
            }
            LogHelper.UILogger.Debug("CommonResource End");
            return lanRes;
        }

        public static ResourceDictionary LoadLanguageResource()
        {
            LogHelper.UILogger.Debug("LoadLanguageResource Start");
            ResourceDictionary lanRes = new ResourceDictionary() { Source = new Uri("/LocalizedResources/Language.xaml", UriKind.Relative) };
            CultureInfo curCluture = GetAppCulture();
            string LanResPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalizedResources");
            string en_USResFile = System.IO.Path.Combine(LanResPath, "Language_en-US.xaml");

            string resFile = String.Format("Language_{0}.xaml", curCluture.IetfLanguageTag);
            string curLanRes = System.IO.Path.Combine(LanResPath, resFile);
            ResourceDictionary localizedlanRes = null;
            LogHelper.UILogger.DebugFormat("try to load language file:{0}", curLanRes);
            if (File.Exists(curLanRes))
            {
                try
                {
                    using (var fs = new FileStream(curLanRes, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Read in an EnhancedResourceDictionary File or preferably an GlobalizationResourceDictionary file
                        localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.UILogger.Error("load language file Failed", ex);
                }
            }

            if (localizedlanRes == null)
            {
                LogHelper.UILogger.DebugFormat("try to load language file:{0}", en_USResFile);
                if (File.Exists(en_USResFile))
                {
                    try
                    {
                        using (var fs = new FileStream(en_USResFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.UILogger.Error("load language file Failed", ex);
                    }
                }
            }

            if (localizedlanRes != null)
            {
                foreach (Object dicKey in localizedlanRes.Keys)
                {
                    if (lanRes.Contains(dicKey))
                    {
                        lanRes[dicKey] = localizedlanRes[dicKey];
                    }
                }
            }
            ReplacePlaceHolder(lanRes);
            LogHelper.UILogger.Debug("LoadLanguageResource End");
            return lanRes;
        }

        private static void ReplacePlaceHolder(ResourceDictionary lanRes)
        {
            LogHelper.UILogger.Debug(">>ReplacePlaceHolder Start");
            Dictionary<String, String> ControlCodesReplacement = new Dictionary<String, String>();
            ControlCodesReplacement.Add("\\n", "\n");
            ControlCodesReplacement.Add("\\t", "\t");
            ControlCodesReplacement.Add("\\\"", "\"");
            ControlCodesReplacement.Add("\\v", "\v");
            ControlCodesReplacement.Add("\\r", "\r");
            ControlCodesReplacement.Add("\\\\", "\\");
            ControlCodesReplacement.Add("[THIS_APPLICATION_NAME]", StringConstant.ApplicationName);
            ControlCodesReplacement.Add("[PRODUCT_NAME]", StringConstant.ProductName);
            ControlCodesReplacement.Add("[FEATURENAME]", StringConstant.ApplicationName);
            //ControlCodesReplacement.Add("[APPLICATION_NAME_ANDROID]", StringConstant.BackItUpAndroidAppName);

            foreach (Object newDictKey in lanRes.Keys)
            {
                Object newDictValue = null;
                if (!lanRes.Contains(newDictKey))
                {
                    continue;
                }

                newDictValue = lanRes[newDictKey];
                if (newDictValue is String)
                {
                    String tempString = (String)newDictValue;
                    foreach (String ControlCode in ControlCodesReplacement.Keys)
                    {
                        tempString = tempString.Replace(ControlCode, ControlCodesReplacement[ControlCode]);
                    }
                    lanRes[newDictKey] = tempString;
                }
            }
            // AH: Excluded Flow Direction from LocalizedDictionary.xaml and adding it programmatically in ResourceDictionary
            lanRes.Add("IDL_FLOWDIRECTION", FlowDirection.LeftToRight);

            LogHelper.UILogger.Debug(">>ReplacePlaceHolder End");
        }
    }
}
