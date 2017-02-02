using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ekko.Modes
{
    class Combo : Ekko
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var target = Q.GetTarget(Champ);
            if (MenuValue.Combo.UseQ)
            {
                if (target != null)
                {
                    //if (!E.IsReady() || !MenuValue.Combo.UseE)
                    //{
                    //    var pred = Q.GetPrediction(target);
                    //    if (pred.CanNext(Q, MenuValue.General.QHitChance))
                    //    {
                    //        Q.Cast(pred.UnitPosition);
                    //    }
                    //}
                    if (Q.IsReady())
                    {
                        //if (target.Distance(player) > 300) return;
                        var pred = Q.GetPrediction(target);
                        if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
            if (MenuValue.Combo.UseE)
            {
                var Etar = E.GetTarget(425);
                if (Etar != null)
                {
                    switch (MenuValue.Combo.LogicE)
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
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var Wtar = W.GetTarget(Champ);
                if (Wtar != null)
                {
                    switch (MenuValue.Combo.Prediction)
                    {
                        case true:
                            {
                                W.Cast(W.GetPrediction(Wtar).CastPosition);
                            }
                            break;
                        case false:
                            {
                                var pos = player.Distance(Wtar) < 1200 || player.Position.IsGrass() ? Wtar.Direction : ObjectManager.Get<Obj_AI_Turret>().OrderBy(x => x.Distance(Wtar)).First(x => x.IsValid && !x.IsDead && x.IsEnemy).Position;
                                W.Cast(Wtar.Position.Extend(pos, 550).To3DWorld());
                            }
                            break;
                    }
                }
            }
        }
    }
}
