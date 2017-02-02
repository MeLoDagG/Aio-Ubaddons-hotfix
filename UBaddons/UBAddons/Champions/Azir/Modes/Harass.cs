using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Azir.Modes
{
    class Harass : Azir
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
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
            if (MenuValue.Harass.UseW && W.IsReady())
            {
                if (Orbwalker.AzirSoldiers.Count < MenuValue.Harass.MaxSoldier)
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
        }
    }
}
