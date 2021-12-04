using Microsoft.Win32;

namespace SpotifyHelper.Core.Utils;

public class ProgramStartup
{
    private const string REG_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    private readonly string m_appName;
    private readonly string m_appPath;

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
            key.DeleteValue(m_appName, throwOnMissingValue: false);
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
        return Registry.CurrentUser.OpenSubKey(REG_PATH, writable: true)
            ?? Registry.CurrentUser.CreateSubKey(REG_PATH, writable: true);
    }
}
