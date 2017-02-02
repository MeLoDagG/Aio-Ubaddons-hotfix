using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ziggs.Modes
{
    class LaneClear : Ziggs
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
                    var farmloc = Q.GetBestCircularCastPosition(Minion);
                    if (farmloc.HitNumber >= MenuValue.LaneClear.QHit)
                    {
                        Q.Cast(farmloc.CastPosition);
                    }
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var farmloc = W.GetBestCircularCastPosition(Minion);
                    if (farmloc.HitNumber >= MenuValue.LaneClear.WHit)
                    {
                        W.Cast(farmloc.CastPosition);
                    }
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var Minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var farmloc = E.GetBestCircularCastPosition(Minion);
                    if (farmloc.HitNumber >= MenuValue.LaneClear.EHit)
                    {
                        E.Cast(farmloc.CastPosition);
                    }
                }
            }
        }
    }
}
