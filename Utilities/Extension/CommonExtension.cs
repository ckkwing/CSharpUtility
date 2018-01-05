using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extension
{
    public static class CommonExtension
    {
        public static bool IsNull(this object obj)
        {
            bool result = obj == null;
            return result;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            bool result = list == null || !list.Any();
            return result;
        }

        public static bool EqualIgnoreCase(this string stringA, string stringB)
        {
            bool result = false;
            if (stringA.IsNullOrEmpty() && stringB.IsNullOrEmpty())
            {
                result = true;
            }

            if (!stringA.IsNullOrEmpty() && !stringB.IsNullOrEmpty())
            {
                result = stringA.Equals(stringB, StringComparison.InvariantCultureIgnoreCase);
            }
            return result;
        }

        public static bool StartWithIgnoreCase(this string stringA, string stringB)
        {
            bool result = false;
            if (stringA.IsNullOrEmpty() && stringB.IsNullOrEmpty())
            {
                result = true;
            }

            if (!stringA.IsNullOrEmpty() && !stringB.IsNullOrEmpty())
            {
                result = stringA.StartsWith(stringB, StringComparison.InvariantCultureIgnoreCase);
            }
            return result;
        }

        #region For string extension method
        /// <summary>
        /// same as string.IsNullOrEmpty
        /// </summary>
        /// <param name="StrA"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string StrA)
        {
            return string.IsNullOrEmpty(StrA);
        }

        /// <summary>
        /// PathA is same as PathB
        /// </summary>
        /// <param name="StrA"></param>
        /// <param name="StrB"></param>
        /// <returns></returns>
        public static bool IsPathEqual(this string pathA, string pathB)
        {
            if (pathA.IsNullOrEmpty() && pathB.IsNullOrEmpty()) return true;
            if (pathA.IsNullOrEmpty() || pathB.IsNullOrEmpty()) return false;

            var pathAArray = pathA.Split(new string[] { "\\", ":" }, StringSplitOptions.RemoveEmptyEntries);
            var PathBArray = pathB.Split(new string[] { "\\", ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (pathAArray.Length != PathBArray.Length) return false;
            for (int i = 0; i < pathAArray.Length; i++)
            {
                if (!string.Equals(pathAArray[i], PathBArray[i], StringComparison.CurrentCultureIgnoreCase)) return false;
            }
            return true;
        }

        /// <summary>
        /// PathA is same as PathB
        /// </summary>
        /// <param name="StrA"></param>
        /// <param name="StrB"></param>
        /// <returns></returns>
        public static bool IsPathRoot(this string pathA)
        {
            if (pathA.IsNullOrEmpty()) return false;
            var pathAArray = pathA.Split(new string[] { "\\", ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (pathAArray.Length != 1)
                return false;

            return true;
        }


        /// <summary>
        /// PathA is same as PathB
        /// </summary>
        /// <param name="StrA"></param>
        /// <param name="StrB"></param>
        /// <returns></returns>
        public static bool IsPathRootEqual(this string pathA, string pathB)
        {
            if (pathA.IsNullOrEmpty() && pathB.IsNullOrEmpty()) return true;
            if (pathA.IsNullOrEmpty() || pathB.IsNullOrEmpty()) return false;

            var pathAArray = pathA.Split(new string[] { "\\", ":" }, StringSplitOptions.RemoveEmptyEntries);
            var pathBArray = pathB.Split(new string[] { "\\", ":" }, StringSplitOptions.RemoveEmptyEntries);

            if (pathAArray.Length == 0 && pathBArray.Length == 0)
            {
                return true;
            }

            if (pathAArray.Length == 0 || pathBArray.Length == 0)
            {
                return false;
            }

            if (!string.Equals(pathAArray[0], pathBArray[0], StringComparison.CurrentCultureIgnoreCase))
                return false;
            return true;
        }


        /// <summary>
        /// check this is server name or not
        /// as  '//cn-c-w-005', if have more than  2 '/', this is not server  name
        /// </summary>
        /// <param name="webPath"></param>
        /// <returns></returns>
        public static bool IsServerName(this string webPath)
        {
            if (webPath.IsNullOrEmpty()) return false;
            var pathAArray = webPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            if (pathAArray.Length > 1)
                return false;
            else
                return true;
        }

        /// <summary>
        /// if path is start with "::", such as "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}" , this is shell Path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsShellPath(this string path)
        {
            if (path.IsNullOrEmpty()) return false;
            if (path.IsStartsWith("::")) return true;
            return false;
        }

        /// <summary>
        ///  same as string.equal, IgnoreCase
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static bool IsEqual(this string strA, string strB)
        {
            if (strA.IsNullOrEmpty() && strB.IsNullOrEmpty()) return true;
            if (strA.IsNullOrEmpty() || strB.IsNullOrEmpty()) return false;
            return string.Compare(strA.Trim(), strB.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// same as string.StartsWith, IgnoreCase
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static bool IsStartsWith(this string strA, string strB)
        {
            if (string.IsNullOrEmpty(strA) || string.IsNullOrEmpty(strB)) return false;
            return strA.Trim().StartsWith(strB.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// same as string.contains, IgnoreCase
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="testValue"></param>
        /// <returns></returns>
        public static bool IsContains(this string sourceValue, string testValue)
        {
            if (string.IsNullOrEmpty(sourceValue) || string.IsNullOrEmpty(testValue)) return false;
            return sourceValue.Trim().ToUpperInvariant().Contains(testValue.Trim().ToUpperInvariant());
        }

        /// <summary>
        /// convert string to long
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static long ToLong(this string param)
        {
            long ret;
            if (long.TryParse(param, out ret))
            {
                return ret;
            }
            else
                return -1;
        }

        /// <summary>
        /// convert string to date 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string param)
        {
            DateTime ret;
            if (DateTime.TryParse(param, out ret))
                return ret;
            else
                return DateTime.MinValue;
        }

        #endregion

        #region for IEnumberable extension method
        /// <summary>
        /// same as Contains, IgnoreCase
        /// </summary>
        /// <param name="StrArray"></param>
        /// <param name="StrB"></param>
        /// <returns></returns>
        public static bool IsContains(this IEnumerable<string> StrArray, string StrB)
        {
            if (StrArray.IsNull()) return false;
            if (string.IsNullOrEmpty(StrB)) return true;
            return StrArray.Any(o => o.IsEqual(StrB));
        }

        /// <summary>
        /// array= null & arry.count==0 
        /// </summary>
        /// <param name="StrArray"></param>
        /// <param name="StrB"></param>
        /// <returns></returns>
        public static bool IsNull(this IEnumerable<object> objArray)
        {
            try
            {
                if (objArray == null) return true;
                if (objArray.FirstOrDefault() == null) return true;
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// array!= null & arry.count>0 
        /// </summary>
        /// <param name="StrArray"></param>
        /// <param name="StrB"></param>
        /// <returns></returns>
        public static bool IsNotNull(this IEnumerable<object> objArray)
        {
            return !objArray.IsNull();
        }

        /// <summary>
        /// is a list null or empty
        /// </summary>
        /// <param name="StrArray"></param>
        /// <param name="StrB"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IEnumerable<object> objArray)
        {
            return objArray.IsNull() || objArray.Count() == 0;
        }


        #endregion
    }
}
