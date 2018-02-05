using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extension;

namespace Utilities.Common
{
    public class ShellWin32Window : System.Windows.Forms.IWin32Window
    {
        IntPtr _handle;
        public ShellWin32Window(System.Windows.Interop.HwndSource source = null)
        {
            if (!source.IsNull())
                _handle = source.Handle;
        }
        public ShellWin32Window(IntPtr handle)
        {
            _handle = handle;
        }

        #region IWin32Window Members
        IntPtr System.Windows.Forms.IWin32Window.Handle
        {
            get { return _handle; }
        }
        #endregion
    }
}
