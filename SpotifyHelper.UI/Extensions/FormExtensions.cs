using System;
using System.Windows.Forms;

namespace SpotifyHelper.UI.Extensions
{
    public static class FormExtensions
    {
        public static T Invoke<T>(this Form form, Func<T> func)
        {
            return (T)form.Invoke(func);
        }
    }
}
