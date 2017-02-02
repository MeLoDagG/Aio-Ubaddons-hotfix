using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using EloBuddy;
using SharpDX;

namespace UBAddons.Champions.Ekko.Modes
{
    class Harass : Ekko
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var target = Q.GetTarget(Champ);
            if (MenuValue.Harass.UseQ)
            {
                if (target != null)
                {
                    if (!E.IsReady() || !MenuValue.Harass.UseE)
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    else
                    {
                        if (target.Distance(player) > 300) return;
                        var pred = Q.GetPrediction(target);
                        if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
            if (MenuValue.Harass.UseE)
            {
                var Etar = E.GetTarget(425);
                if (Etar != null)
                {
                    switch (MenuValue.Harass.LogicE)
                    {
                        case 0:
                            {
                                if (Champ != null || player.CountEnemyChampionsInRange(1200) <= 2)
                                {
                                    E.Cast(player.Position.Extend(Game.CursorPos, E.Range).To3DWorld());
                                }
                            }
                            break;
                        case 1:
                            {
                                if (Champ != null || player.CountEnemyChampionsInRange(1200) <= 2)
                                {
                                    Vector2[] Pos = player.Position.CirclesIntersection(E.Range, Etar.Position, 400).OrderBy(x => x.Distance(Game.CursorPos)).ToArray();
                                    if (Pos.Length == 0) return;
                                    Vector3 EPos = Pos.First().To3DWorld();
                                    E.Cast(EPos);
                                }
                            }
                            break;
                        case 2:
                            {
                                if (Champ != null || player.CountEnemyChampionsInRange(1200) <= 2)
                                {
                                    E.Cast(player.Position.Extend(Etar, E.Range).To3DWorld());
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
