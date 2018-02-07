using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extension;

namespace Utilities
{
    public class ThreadHelper
    {
        public static void EndInvokeAction(IAsyncResult result)
        {
            Action action = result.AsyncState as Action;
            if (action.IsNull())
            {
                return;
            }
            
            action.EndInvoke(result);
        }

        public static T EndInvokeFunc<T>(IAsyncResult result)
        {
            Func<T> func = result.AsyncState as Func<T>;
            if (func.IsNull())
            {
                return default(T);
            }

            return func.EndInvoke(result);
        }
    }
}
