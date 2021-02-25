using Microsoft.Win32;
using System.Diagnostics;

namespace SpotifyHelper.Core
{
    public class Browser
    {
        public static Process OpenDefault(string url)
        {
            var progId = GetSystemDefaultBrowserProgId();

            var path = GetSystemDefaultBrowserPath(progId);
            var newWindowArg = GetNewWindowArgument(progId);

            return Process.Start(new ProcessStartInfo()
            {
                FileName = path,
                Arguments = url + " " + newWindowArg
            });
        }

        private static string GetNewWindowArgument(string progId)
        {
            return progId switch
            {
                "IE.HTTP" => "-new",
                "FirefoxURL" => "-new-instance",
                "OperaStable" => "-newwindow",
                "SafariHTML" => "$1",
                _ => "--new-window --user-data-dir=\"temp/spotifyhelper\"" //ChromeHTML or any other browser (i'm assuming all chromium based)
            };
        }

        private static string GetSystemDefaultBrowserProgId()
        {
            using var userChoiceKey = Registry.CurrentUser
                .OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice", false);

            return userChoiceKey?.GetValue("Progid")?.ToString();
        }

        private static string GetSystemDefaultBrowserPath(string progId)
        {
            using var registryKey = Registry.ClassesRoot.OpenSubKey($"{progId}\\shell\\open\\command", false);

            return registryKey
                .GetValue(null)
                .ToString()
                .ToLower()
                .Replace("\"", "")
                .Split(".exe")[0] + ".exe";
        }
    }
}
