using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Zilean.Modes
{
    internal class LaneClear : Zilean
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsEnemy && x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
            if (Minion == null) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                Q.Cast(Minion.First());
            }
            if (MenuValue.LaneClear.UseW && !Q.IsReady(1200) && W.IsReady())
            {
                W.Cast();
            }
        }
    }
}
