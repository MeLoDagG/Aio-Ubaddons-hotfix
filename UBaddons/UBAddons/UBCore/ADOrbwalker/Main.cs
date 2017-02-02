using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Libs.Base;
using UBAddons.Log;

namespace UBAddons.UBCore.ADOrbwalker
{
    class Main : IModuleBase
    {
        internal static Menu OrbMenu;
        public static bool Initialized { get; private set; }
        static Main()
        {
            if (MainMenu.MenuInstances.FirstOrDefault(x => x.Key.Contains("UBAddons")).Value == null)
            {
                Debug.Print("There's has a problem when trying to inject Force Target. Please report to Uzumaki Boruto", Console_Message.Error);
            }
            else
            {
                try
                {
                    var Menu = MainMenu.MenuInstances.FirstOrDefault(x => x.Key.Contains("UBAddons")).Value.FirstOrDefault();
                    OrbMenu = Menu.AddSubMenu("Force", "UBAddons.Force", "Orbwallker for UBAddons");
                    {
                        OrbMenu.AddGroupLabel("Intro");
                        OrbMenu.AddLabel("Hello, This misc is about hitting minion while combo for more life steal");
                        OrbMenu.AddLabel("It just my idia, because sometime I fell it is necessary");
                        OrbMenu.AddGroupLabel("Setting");
                        OrbMenu.Add("Enable", new CheckBox("Enable Smart Orb"));
                        OrbMenu.Add("LifeSteal", new Slider("Enable Only my lifesteal more than", 30));
                        OrbMenu.Add("CritChance", new Slider("Enable Only my crit chance more than", 50));
                        OrbMenu.Add("MyHP", new Slider("Enable if My HP below {0}", 20));
                        OrbMenu.Add("MoreAttack", new Slider("Don't do this if enemy can kill with {0} attack more", 4, 1, 10));
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString(), Console_Message.Error);
                }
            }
        }
        internal static void Force()
        {
            if (Player.Instance.PercentPhysicalLifeStealMod() < OrbMenu.VSliderValue("LifeSteal") || Player.Instance.FlatCritChanceMod * 100 < OrbMenu.VSliderValue("CritChance")
                || !Orbwalker.ActiveModes.Combo.IsOrb() || !Orbwalker.LaneClearMinionsList.Any() || Player.Instance.HealthPercent > OrbMenu.VSliderValue("MyHP") || Orbwalker.GetTarget() == null)
            {
                Orbwalker.ForcedTarget = null;
                return;
            }
            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target == null ||target.Health <= OrbMenu.VSliderValue("MoreAttack") * Player.Instance.GetAutoAttackDamage(target, true))
            {
                Orbwalker.ForcedTarget = null;
                return;
            }
            Orbwalker.ForcedTarget = Orbwalker.LaneClearMinionsList.FirstOrDefault(x => x.IsValidTarget(Player.Instance.GetAutoAttackRange(x)) && !x.IsInvulnerable);
        }

        public bool ShouldExecuted()
        {
            return Variables.IsADC;
        }

        public void OnLoad()
        {
            Initialize();
        }

        public void Execute()
        {
            Force();
        }

        public static void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            Initialized = true;
        }
    }
}
