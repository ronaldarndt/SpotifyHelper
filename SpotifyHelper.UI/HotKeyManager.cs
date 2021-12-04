using System.Runtime.InteropServices;
using System.Threading;

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

    private static readonly Window s_window = new();
    private static int s_id = 0;

    public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
    {
        var id = Interlocked.Increment(ref s_id);

        RegisterHotKey(s_window.Handle, id, (uint)modifiers, (uint)key);

        return id;
    }

    public static void UnregisterHotKey(int id)
    {
        UnregisterHotKey(s_window.Handle, id);
    }

    private static void OnHotKeyPressed(HotKeyEventArgs e)
    {
        HotKeyPressed?.Invoke(null, e);
    }

    public class HotKeyEventArgs : EventArgs
    {
        public HotkeyConfig Keys { get; init; }

        public HotKeyEventArgs(IntPtr hotKeyParam)
        {
            var param = (uint)hotKeyParam.ToInt64();

            Keys = new HotkeyConfig((Keys)((param & 0xffff0000) >> 16), (KeyModifiers)(param & 0x0000ffff));
        }
    }

    private class Window : NativeWindow
    {
        private const int WM_HOTKEY = 0x0312;

        public Window() => CreateHandle(new CreateParams());

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                OnHotKeyPressed(new HotKeyEventArgs(m.LParam));
            }
        }
    }
}
