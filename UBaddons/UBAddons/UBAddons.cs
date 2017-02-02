using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using System;
using UBAddons.Libs;
using UBAddons.Libs.Base;
using UBAddons.Log;
using UBAddons.General;

namespace UBAddons
{
    class UBAddons
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
            UpdateChecker.CheckForUpdates();
            while (UpdateChecker.CurrentVersion == System.Version.Parse("0.0.0.0"))
            { }
        }
        internal static IHeroBase PluginInstance { get; private set; }
        private static void Loading_OnLoadingComplete(System.EventArgs args)
        {
            LoadPlugin();
            UBCore.CoreMenu.Initialize();
            if (PluginInstance == null)
            {
                Debug.Print("UBAddons not support " + Player.Instance.Hero + " yet", Console_Message.Outdate);
                return;
            }
            else
            {
                TargetSelector.ActiveMode = TargetSelectorMode.LeastHealth;
                UBNotification.ShowNotif("UBAddons Notification", Player.Instance.Hero + " Loaded", "notification");
                Debug.Print(Player.Instance.Hero + " Loaded. Enjoys", Console_Message.Notifications);
                Chat.Print(Variables.Roles);
            }
            Chat.Print("<font = 'Comic Sans MS'>UBAddons: Please report on my thread if its have any bugs</font>", System.Drawing.Color.FromArgb(245, 14, 147));
        }

        private static void LoadPlugin()
        {
            var typeName = "UBAddons.Champions." + Player.Instance.Hero + "." + Player.Instance.Hero;

            var type = Type.GetType(typeName);

            if (type == null) return;

            PluginInstance = (IHeroBase)Activator.CreateInstance(type);
            Libs.Dictionary.FleeSpell.Initialize();
            ShowNotification();
            Core.DelayAction(() =>
            {
                Initialize(PluginInstance);
                UtilityPlugin.OnLoad();
                if (UBCore.CoreMenu.UseOnTick)
                {
                    Game.OnTick += UtilityPlugin.OnUpdate;
                }
                if (UBCore.CoreMenu.UseOnUpdate)
                {
                    Game.OnUpdate += UtilityPlugin.OnUpdate;
                }
            }, 200);
        }
        private static void Initialize(IHeroBase addonbase)
        {
            addonbase.CreateMenu();
            Interrupter.OnInterruptableSpell += addonbase.OnInterruptable;
            Orbwalker.OnUnkillableMinion += addonbase.OnUnkillableMinion;
            Gapcloser.OnGapcloser += addonbase.OnGapcloser;
            Drawing.OnDraw += addonbase.OnDraw;
            if (UBCore.CoreMenu.UseOnTick)
            {
                Game.OnTick += addonbase.OnTick;
            }
            if (UBCore.CoreMenu.UseOnUpdate)
            {
                Game.OnUpdate += addonbase.OnTick;
            }
        }
        private static void ShowNotification()
        {
            var CurrentVersion = UpdateChecker.CurrentVersion;
            if (CurrentVersion > typeof(UBAddons).Assembly.GetName().Version)
            {
                Chat.Print("<font = 'Comic Sans MS'>Outdate. Newest verion: " + CurrentVersion + "</font>", System.Drawing.Color.OrangeRed);
                UBNotification.ShowNotif("UBAddons Notification", "Outdate. Newest verion: " + CurrentVersion, "outdate");
                Debug.Print("Outdate. Newest verion: " + CurrentVersion, Console_Message.Outdate);
            }
            if (CurrentVersion == typeof(UBAddons).Assembly.GetName().Version)
            {
                Chat.Print("<font = 'Comic Sans MS'>This is newest version: " + CurrentVersion + "</font>", System.Drawing.Color.LightGreen);
                UBNotification.ShowNotif("UBAddons Notification", "This is newest version: " + CurrentVersion, "update");
                Debug.Print("This is newest version: " + CurrentVersion, Console_Message.Notifications);
            }
            if (CurrentVersion < typeof(UBAddons).Assembly.GetName().Version)
            {
                Chat.Print("Thanks for helping me", System.Drawing.Color.LightGreen);
                UBNotification.ShowNotif("UBAddons Notification", "Thanks for helping me <3", "update");
                Debug.Print("Thanks for helping me <3", Console_Message.Notifications);
            }
        }
    }
}