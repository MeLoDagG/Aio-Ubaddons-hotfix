using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Alistar.Modes
{
    class LaneClear : Alistar
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Count() >= MenuValue.LaneClear.QHit)
                {
                    Q.Cast();
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    W.Cast(minion.First());
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    E.Cast(minion.First());
                }
            }
        }
    }
}
