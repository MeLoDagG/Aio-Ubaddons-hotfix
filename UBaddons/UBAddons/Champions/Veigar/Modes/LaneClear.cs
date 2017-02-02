using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Veigar.Modes
{
    class LaneClear : Veigar
    {
        public static void Execute()
        {
            if (MenuValue.LaneClear.APEnable)
            {
                if (Q.IsReady())
                {
                    var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(Q.Range) && m.Health <= QDamage(m));
                    var LCMinion = Orbwalker.LaneClearMinionsList;
                    var allyminion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Ally);
                    if (minions.Any())
                    {
                        switch (minions.Count())
                        {
                            case 0:
                                {
                                    Orbwalker.ForcedTarget = LCMinion.Where(x => x != Orbwalker.LaneClearMinion).OrderBy(x => x.Distance(Orbwalker.LaneClearMinion)).First();
                                }
                                break;
                            case 1:
                                {
                                    if (minions.First().CountEnemyMinionsInRange(Q.Width) == 0 || minions.First().IsSiegeMinion())
                                    {
                                        Q.Cast(minions.First());
                                    }
                                    else
                                    {
                                        if (allyminion != null)
                                        {
                                            if (Orbwalker.PriorityLastHitWaitingMinion != null)
                                            {
                                                Orbwalker.ForcedTarget = LCMinion.Where(m => m.IsValid && m.Health >= QDamage(m) && m != Orbwalker.PriorityLastHitWaitingMinion).OrderBy(m => m.Distance(Orbwalker.PriorityLastHitWaitingMinion)).FirstOrDefault();
                                            }
                                            else
                                            {
                                                Orbwalker.ForcedTarget = LCMinion.Where(m => m.IsValid && m.Health >= QDamage(m) && m != minions.FirstOrDefault()).OrderBy(m => m.Distance(minions.FirstOrDefault())).FirstOrDefault();
                                            }
                                        }
                                        else
                                        {
                                            Orbwalker.ForcedTarget = LCMinion.Where(m => m.IsValid && m.Health > QDamage(m) && m != minions.FirstOrDefault()).OrderBy(m => m.Distance(minions.FirstOrDefault())).FirstOrDefault();
                                        }
                                    }
                                }
                                break;
                            default:
                                {
                                    Orbwalker.ForcedTarget = null;
                                    foreach (var minion in minions)
                                    {
                                        var pred = Q.GetPrediction(minion);
                                        var Collision = pred.CollisionObjects;
                                        var boolvalue = pred.GetCollisionObjects<AIHeroClient>().Any(x => x.IsEnemy);
                                        if (Collision.Length == 1 && Collision.First().Health < QDamage(Collision.First()) && boolvalue)
                                        {
                                            Q.Cast(pred.CastPosition);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    Orbwalker.ForcedTarget = null;
                }
            }
            else
            {
                Orbwalker.ForcedTarget = null;
                if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
                if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                    && MenuValue.LaneClear.EnableIfNoEnemies)) return;
                if (MenuValue.LaneClear.UseQ && Q.IsReady())
                {
                    var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                    if (Minion.Any())
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
                        var farmloc = Q.GetBestLinearCastPosition(Minion);
                        if (farmloc.HitNumber >= MenuValue.LaneClear.WHit)
                        {
                            W.Cast(farmloc.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
