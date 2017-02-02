using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using SharpDX;
using System;

namespace UBAddons.Champions.Malzahar.Modes
{
    class LaneClear : Malzahar
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minions = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minions.Any())
                {
                    var Minion = (from minion in Minions
                                  let end = minion.Position.To2D()
                                  let rectangle = new Geometry.Polygon.Rectangle(end + (400 - end).Rotated((float)Math.PI / 2), end + (400 - end).Rotated(-(float)Math.PI / 2), Q.Width)
                                  where Minions.Count(x => x.IsValidTarget() && rectangle.IsInside(x)) >= MenuValue.LaneClear.QHit
                                  orderby Minions.Count(x => x.IsValidTarget() && rectangle.IsInside(x)) descending
                                  select minion).FirstOrDefault();
                    if (Minion != null)
                    {
                        Q.Cast(Minion);
                    }
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var Minion = W.GetLaneMinions();
                if (Minion.Any() && Minion.Count() >= MenuValue.LaneClear.WHit)
                {
                    W.Cast(Minion.First());
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
