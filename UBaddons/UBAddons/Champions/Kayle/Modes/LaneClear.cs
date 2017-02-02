using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kayle.Modes
{
    class LaneClear : Kayle
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    Q.Cast(minion.First());
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var minion = E.GetLaneMinions();
                if (minion.Any())
                {
                    E.Cast();
                }
            }
        }
    }
}
