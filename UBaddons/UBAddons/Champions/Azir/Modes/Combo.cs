using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Azir.Modes
{
    class Combo : Azir
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (target.IsEscapeFrom(player.Position))
                    {
                        var Pos = player.Position.Extend(pred.CastPosition, player.Distance(pred.CastPosition) + target.MoveSpeed * 0.15f).To3DWorld();
                        if (Q.IsInRange(Pos))
                        {
                            Q.Cast(Pos);
                        }
                        else
                        {
                            if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                if (Orbwalker.AzirSoldiers.Count < MenuValue.Combo.MaxSoldier)
                {
                    if (Orbwalker.AzirSoldiers.Count == 1)
                    {
                        var target = Q.GetTarget();
                        if (target != null && Q.IsReady())
                        {
                            W.Cast(W.IsInRange(target) ? target.Position : target.Position.Extend(player, W.Range).To3DWorld());
                        }
                    }
                    else
                    {
                        var target = Q.IsReady() ? Q.GetTarget() : W.GetTarget();
                        if (target != null)
                        {
                            W.Cast(W.IsInRange(target) ? target.Position : target.Position.Extend(player, W.Range).To3DWorld());
                        }
                    }
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var _ESoldier = (from soldier in Orbwalker.AzirSoldiers
                               let rec = new Geometry.Polygon.Rectangle(player.Position, soldier.Position, E.Width)
                               let enemyinside = EntityManager.Heroes.Enemies.Where(x => rec.IsInside(E.GetPrediction(x).UnitPosition)).OrderBy(x => x.Health)
                               where enemyinside != null
                               select soldier).FirstOrDefault();
                if (_ESoldier != null)
                {
                    var rectangle = new Geometry.Polygon.Rectangle(player.Position, _ESoldier.Position, E.Width);
                    var target = EntityManager.Heroes.Enemies.Where(x => x.IsValid && !x.IsDead && !x.IsZombie && rectangle.IsInside(E.GetPrediction(x).UnitPosition));
                    if (target != null)
                    {
                        if (target.First().Health < HandleDamageIndicator(target.First()) || !MenuValue.Combo.UseEKillable)
                        {
                            E.Cast(_ESoldier);
                        }
                    }
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {

            }
        }
    }
}
