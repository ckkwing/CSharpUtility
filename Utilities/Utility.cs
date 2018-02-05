using Microsoft.Win32;
using NLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Common;
using Utilities.WindowsAPI;

namespace Utilities
{
    public class Utility
    {
        public static string GetProductVersion()
        {
            string version = string.Empty;
            try
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                version = fvi.FileVersion;
            }
            catch (Exception ex)
            {
                LogHelper.UILogger.Debug("GetEntryAssembly:", ex);
            }

            if (string.IsNullOrEmpty(version))
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    version = fvi.FileVersion;
                }
                catch (Exception ex)
                {
                    LogHelper.UILogger.Debug("GetExecutingAssembly:", ex);
                    version = GetProductVersionFromRegistry();
                }
            }
            return version;
        }

        public static string GetProductVersionFromRegistry()
        {
            const string VERSION = "Version";
            string result = GetRegistryValue<string>(VERSION);
            return result;
        }

        private static T GetRegistryValue<T>(string keyName)
        {
            T result = default(T);

            try
            {
                using (RegistryKey regLocalMachine = Registry.LocalMachine)
                {
                    using (RegistryKey regBIU = regLocalMachine.OpenSubKey(StringConstant.REGISTRY_KEY_MEDIACENTER))
                    {
                        if (regBIU != null)
                            result = (T)regBIU.GetValue(keyName);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.UILogger.Debug("Registry value", ex);
            }
            return result;
        }

        public static void PreventSleep(bool bPrevent)
        {
            if (bPrevent)
                NativeFileAPI.SetThreadExecutionState(NativeFileAPI.ES_CONTINUOUS | NativeFileAPI.ES_SYSTEM_REQUIRED);
            else
                NativeFileAPI.SetThreadExecutionState(NativeFileAPI.ES_CONTINUOUS);
        }

        public static string BrowseFolder(string description, string selectedPath, System.Windows.Interop.HwndSource source)
        {
            string folderPath = string.Empty;
            try
            {
                Environment.SpecialFolder rootFolder = Environment.SpecialFolder.Desktop;

                System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowser.Description = description;
                folderBrowser.RootFolder = rootFolder;
                folderBrowser.SelectedPath = selectedPath;
                System.Windows.Forms.IWin32Window iWin32Window = new ShellWin32Window(source);

                System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog(iWin32Window);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    folderPath = folderBrowser.SelectedPath;
                }
                
            }
            catch (System.Exception ex)
            {
                folderPath = null;
                LogHelper.UILogger.Debug("BrowseFolder Failed,", ex);
            }
            finally
            {
            }
            return folderPath;
        }
    }
}
