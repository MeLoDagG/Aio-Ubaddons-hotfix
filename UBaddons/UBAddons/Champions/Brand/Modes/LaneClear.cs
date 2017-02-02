using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Brand.Modes
{
    class LaneClear : Brand
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any() && Minion.Count() > MenuValue.LaneClear.Qhit)
                {
                    var farmloc = Q.GetBestLinearCastPosition(Minion);
                    Q.Cast(farmloc.CastPosition);
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var farmloc = W.GetBestCircularCastPosition(Minion);
                    if (farmloc.HitNumber > MenuValue.LaneClear.Whit)
                    {
                        W.Cast(farmloc.CastPosition);
                    }
                }
            } 
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var Minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any() && Minion.Count(x => HasPassiveBuff(x)) > MenuValue.LaneClear.Ehit)
                {
                    E.Cast(Minion.FirstOrDefault(x => HasPassiveBuff(x)));
                }
            }
        }
    }
}
