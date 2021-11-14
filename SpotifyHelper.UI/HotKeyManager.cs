using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SpotifyHelper.UI;

public static class HotKeyManager
{
    #region External imports

    [DllImport("user32", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    #endregion

    #region Events

    public static event EventHandler<HotKeyEventArgs>? HotKeyPressed;

    #endregion

    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        NoRepeat = 0x4000
    }

    public record HotkeyConfig(Keys Keys, KeyModifiers Modifiers);

    private static readonly ManualResetEvent s_windowReadyEvent = new(initialState: false);
    private static MessageWindow s_msgWindow = new();
    private static int s_id = 0;

    static HotKeyManager()
    {
        new Thread(MessageLoopThread)
        {
            Name = nameof(MessageLoopThread),
            IsBackground = true
        }.Start();

        static void MessageLoopThread()
        {
            s_msgWindow = new();
            s_windowReadyEvent.Set();

            Application.Run(s_msgWindow);
        }
    }

    public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
    {
        s_windowReadyEvent.WaitOne();

        var id = Interlocked.Increment(ref s_id);

        s_msgWindow.Invoke(RegisterHotKeyInternal, s_msgWindow.Handle, id, (uint)modifiers, (uint)key);

        return id;
    }

    public static void UnregisterHotKey(int id)
    {
        s_msgWindow.Invoke(UnRegisterHotKeyInternal, s_msgWindow.Handle, id);
    }

    private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
    {
        RegisterHotKey(hwnd, id, modifiers, key);
    }

    private static void UnRegisterHotKeyInternal(IntPtr hwnd, int id)
    {
        UnregisterHotKey(s_msgWindow.Handle, id);
    }

    private static void OnHotKeyPressed(HotKeyEventArgs e)
    {
        HotKeyPressed?.Invoke(null, e);
    }

    public class HotKeyEventArgs : EventArgs
    {
        public readonly Keys Key;
        public readonly KeyModifiers Modifiers;

        public HotKeyEventArgs(IntPtr hotKeyParam)
        {
            uint param = (uint)hotKeyParam.ToInt64();

            Key = (Keys)((param & 0xffff0000) >> 16);

            Modifiers = (KeyModifiers)(param & 0x0000ffff);
        }
    }

    private class MessageWindow : Form
    {
        private const int WM_HOTKEY = 0x312;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                OnHotKeyPressed(new(m.LParam));
            }

            base.WndProc(ref m);
        }

        protected override void SetVisibleCore(bool value) => base.SetVisibleCore(false);
    }
}
