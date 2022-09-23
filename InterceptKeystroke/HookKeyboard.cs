using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace InterceptKeystroke
{
    public class HookKeyboard : IDisposable
    {
        private bool _disposed;
        private Key _keyToIntercept;
        private int _keyCode;
        private IntPtr _hookHandle = IntPtr.Zero;
        private Action<int, ulong, ulong> _onEvent;

        public HookKeyboard(Key keyToIntercept, Action<int, ulong, ulong> onEvent)
        {
            _keyToIntercept = keyToIntercept;
            _keyCode = KeyInterop.VirtualKeyFromKey(keyToIntercept);
            _onEvent = onEvent;

            _hookHandle = Native.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, new Native.HookProc(HookProc), IntPtr.Zero, 0);
        }

        IntPtr HookProc(int code, ulong wParam, ulong lParam)
        {
            if (code >= 0 && (wParam == Native.WM_KEYDOWN) && lParam != 0)
            {
                object? returned = Marshal.PtrToStructure((IntPtr)lParam, typeof(Native.KBDLLHOOKSTRUCT));

                if (returned != null)
                {
                    Native.KBDLLHOOKSTRUCT kbd = (Native.KBDLLHOOKSTRUCT)returned;

                    if (kbd.vkCode == _keyCode)
                    {
                        _onEvent?.Invoke(code, wParam, lParam);
                        return (IntPtr)1;
                    }
                }
            }

            return Native.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_hookHandle != IntPtr.Zero)
                {
                    Native.UnhookWindowsHookEx(_hookHandle);
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
