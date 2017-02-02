using EloBuddy;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UBAddons.Log
{
    class UpdateChecker
    {
        public static System.Version CurrentVersion = new System.Version("0.0.0.0");
        public static void CheckForUpdates()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        var OnlineVersion = webClient.DownloadString("https://raw.githubusercontent.com/Uzumaki-Boruto/AIO/master/UBAddons/UBAddons/Properties/AssemblyInfo.cs");
                        var match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(OnlineVersion);
                        if (match.Success)
                        {
                            CurrentVersion = new System.Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]));
                        }
                    }                  
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString(), General.Console_Message.Error);
                }
            });
        }
    }

}
