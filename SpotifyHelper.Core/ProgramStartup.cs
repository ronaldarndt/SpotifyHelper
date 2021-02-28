using Microsoft.Win32;

namespace SpotifyHelper.Core
{
    public class ProgramStartup
    {
        const string REG_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private string m_appName;
        private string m_appPath;

        public ProgramStartup(string appName, string appPath)
        {
            m_appName = appName;
            m_appPath = appPath;
        }

        public void Toggle()
        {
            var key = GetRegistryKey();

            if (GetStatus(key))
            {
                key.DeleteValue(m_appName, false);
            }
            else
            {
                key.SetValue(m_appName, m_appPath);
            }
        }

        public bool GetStatus()
        {
            return GetStatus(GetRegistryKey());
        }

        private bool GetStatus(RegistryKey registryKey)
        {
            return registryKey.GetValue(m_appName) != null;
        }

        private RegistryKey GetRegistryKey()
        {
            return Registry.CurrentUser.OpenSubKey(REG_PATH, true);
        }
    }
}
