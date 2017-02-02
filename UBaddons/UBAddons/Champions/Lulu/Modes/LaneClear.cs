using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Lulu.Modes
{
    class LaneClear : Lulu
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    Q.CastOnBestFarmPosition(MenuValue.LaneClear.QHit, MenuValue.General.QHitChance);
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var Minion = W.GetLaneMinions();
                if (Minion.Any())
                {
                    W.Cast(player);
                }
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
