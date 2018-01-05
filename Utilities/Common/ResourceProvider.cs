using NLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Utilities.Common
{
    public class ResourceProvider
    {
        public static string AppTitle 
        {
            get { return StringConstant.ApplicationName; }
        }

        public static string ProductName
        {
            get { return StringConstant.ProductName; }
        }

        public static string ReplacePlaceHolder(string rawString, string placeHolderKey, string replaseString)
        {
           return rawString.Replace(placeHolderKey, replaseString);
        }

        public static string LoadString(string resource)
        {
            try
            {
                // MI: Must try first to find the resource otherwise application will crash.
                if (Application.Current != null)
                {
                    object obj = Application.Current.TryFindResource(resource);
                    if (obj != null)
                    {
                        obj = Application.Current.FindResource(resource);
                        return obj.ToString();
                    }
                }
            }
            catch (ResourceReferenceKeyNotFoundException e)
            {
                LogHelper.UILogger.Debug("LoadString ResourceReferenceKeyNotFoundException", e);
            }
            catch (ArgumentNullException ex)
            {
                LogHelper.UILogger.Debug("LoadString ArgumentNullException", ex);
            }

            return resource;
        }

        /// <summary>
        ///  AH: function to load strings from Localizaed dictionary
        /// </summary>
        public static object LoadResource(object resourceKey)
        {
            if (resourceKey == null)
            {
                return null;
            }

            object obj = null;
            try
            {
                // MI: Must try first to find the resource otherwise application will crash.
                if (Application.Current != null)
                {
                    obj = Application.Current.TryFindResource(resourceKey);
                    if (obj != null)
                    {
                        obj = Application.Current.FindResource(resourceKey);
                    }
                }
            }
            catch (ResourceReferenceKeyNotFoundException exc)
            {
                LogHelper.UILogger.Debug("ResourceReferenceKeyNotFoundException:", exc);
            }
            catch (ArgumentNullException exc)
            {
                LogHelper.UILogger.Debug("ArgumentNullException:", exc);
            }
            
            return obj;
        }

        public static ImageSource LoadImageSource(object resourceKey)
        {
            ImageSource source = null;
            try
            {
                if (Application.Current != null)
                {
                    var obj = Application.Current.TryFindResource(resourceKey);
                    if (obj != null)
                        source = (ImageSource)obj;
                }
            }
            catch (ResourceReferenceKeyNotFoundException e)
            {
                LogHelper.UILogger.Debug("LoadImageSource ResourceReferenceKeyNotFoundException", e);
            }
            catch (ArgumentNullException ex)
            {
                LogHelper.UILogger.Debug("LoadImageSource ArgumentNullException", ex);
            }
            return source;
        }
    }
}
