using Microsoft.Win32;
using System.Diagnostics;

namespace SpotifyHelper.Core
{
    public class Browser
    {
        public static Process OpenDefault(string url)
        {
            var progId = GetSystemDefaultBrowserProgId();

            var path = GetSystemDefaultBrowser(progId);
            var newWindowArg = GetNewWindowArgument(progId);

            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = path,
                Arguments = url + " " + newWindowArg
            });

            return process;
        }

        private static string GetNewWindowArgument(string progId)
        {
            return progId switch
            {
                "IE.HTTP" => "-new",
                "FirefoxURL" => "-new-instance",
                "OperaStable" => "-newwindow",
                "SafariHTML" => "$1",
                _ => "--new-window --user-data-dir=\"%temp%/spotifyhelper\"" //ChromeHTML or any other browser (i'm assuming chromium based)
            };
        }

        private static string GetSystemDefaultBrowserProgId()
        {
            using RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice");

            return userChoiceKey?.GetValue("Progid")?.ToString();
        }

        private static string GetSystemDefaultBrowser(string progId)
        {
            using var registryKey = Registry.ClassesRoot.OpenSubKey($"{progId}\\shell\\open\\command", false);

            var name = registryKey.GetValue(null).ToString().ToLower().Replace("\"", "");

            if (!name.EndsWith("exe"))
            {
                //get rid of all command line arguments (anything after the .exe must go)
                name = name.Substring(0, name.LastIndexOf(".exe") + 4);
            }

            return name;
        }
    }
}
