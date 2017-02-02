using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Velkoz.Modes
{
    class LaneClear : Velkoz
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady() && !(Q.ToggleState == 2 || Q.Name.Equals("VelkozQSplitActivate")) && Core.GameTickCount - LastQTick > 120)
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    Q.Cast(Minion.First());
                    LastQTick = Core.GameTickCount;
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                W.CastOnBestFarmPosition(MenuValue.LaneClear.Whit, MenuValue.General.WHitChance);
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var Minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    E.Cast(Minion.First());
                }
            }
        }
    }
}
