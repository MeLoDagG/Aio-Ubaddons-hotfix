using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Orianna.Modes
{
    class LaneClear : Orianna
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
                    var farmloc = Q.GetBestLinearCastPosition(Minion);
                    if (farmloc.HitNumber >= MenuValue.LaneClear.QHit)
                    {
                        Q.Cast(farmloc.CastPosition);
                    }
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any() && Minion.Count() >= MenuValue.LaneClear.WHit)
                {
                    W.Cast();
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any() && Minion.Count() >= MenuValue.LaneClear.EHit)
                {
                    var CurrentBallPos = Q.SourcePosition;
                    foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(E.Range) && x.Distance(CurrentBallPos.Value) > 30))
                    {
                        Geometry.Polygon.Rectangle rectangle = new Geometry.Polygon.Rectangle(CurrentBallPos.Value, ally.Position, Q.Width);
                        if (Minion.Count(x => rectangle.IsInside(x)) > MenuValue.LaneClear.EHit)
                        {
                            E.Cast(ally);
                        }
                    }
                }
            }
        }
    }
}
