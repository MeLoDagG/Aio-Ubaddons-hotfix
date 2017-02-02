using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Anivia.Modes
{
    class LaneClear : Anivia
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady() && Q.ToggleState != 2 && Core.GameTickCount - LastQTick > 120)
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var farmloc = Q.GetBestConeCastPosition(Minion, MenuValue.General.QHitChance);
                    if (farmloc.HitNumber > MenuValue.LaneClear.QHit)
                    {
                        Q.Cast(farmloc.CastPosition);
                        LastQTick = Core.GameTickCount;
                    }
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
            if (MenuValue.LaneClear.UseR && R.IsReady())
            {
                var Minion = R.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var farmloc = R.GetBestCircularCastPosition(Minion);
                    if (farmloc.HitNumber >= MenuValue.LaneClear.RHit)
                    {
                        R.Cast(farmloc.CastPosition);
                    }
                }
            }
        }
    }
}
