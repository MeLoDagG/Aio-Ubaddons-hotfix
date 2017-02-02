using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Xerath.Modes
{
    class LaneClear : Xerath
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                var farmLoc = Q.GetBestLinearCastPosition(Minion, MenuValue.General.WHitChance);
                if (farmLoc.HitNumber >= MenuValue.LaneClear.Qhit)
                {
                    Q.Cast(farmLoc.CastPosition);
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady() && !Q.IsCharging)
            {
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                var farmLoc = W.GetBestCircularCastPosition(Minion, MenuValue.General.WHitChance);
                if (farmLoc.HitNumber >= MenuValue.LaneClear.Whit)
                {
                    W.Cast(farmLoc.CastPosition);
                }
            }
        }
    }
}
