using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using UBAddons.General;

namespace UBAddons.Champions.Xerath.Modes
{
    class Combo : Xerath
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (!player.Spellbook.IsChanneling || Q.IsCharging)
            {
                if (MenuValue.Combo.UseQ && (Q.IsReady() || Q.IsCharging))
                {
                    var target = Q.GetTarget(Champ);
                    if (target != null)
                    {
                        if (Q.IsCharging)
                        {
                            var pred = Q.GetPrediction(target);
                            if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                        else
                        {
                            Q.StartCharging();
                        }
                    }
                }
                if (!Q.IsCharging)
                {
                    if (MenuValue.Combo.UseW && W.IsReady())
                    {
                        var target = W.GetTarget(Champ);
                        if (target != null)
                        {
                            var pred = W.GetPrediction(target);
                            if (pred.CanNext(W, MenuValue.General.WHitChance, true))
                            {
                                W.Cast(pred.CastPosition);
                            }
                        }
                    }
                    if (MenuValue.Combo.UseE && E.IsReady())
                    {
                        var target = E.GetTarget(Champ);
                        if (target != null)
                        {
                            var pred = E.GetPrediction(target);
                            if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                            {
                                E.Cast(pred.CastPosition);
                            }
                        }
                    }
                    if (MenuValue.Combo.UseR && R.IsReady() && player.CountEnemyChampionsInRange(1000) < float.Epsilon)
                    {
                        var target = R.GetKillableTarget();
                        if (target != null)
                        {
                            if (R.IsInRange(Prediction.Position.PredictUnitPosition(target, 2400).To3DWorld()))
                            {
                                Player.CastSpell(SpellSlot.R);
                            }
                        }
                    }
                }
            }
            else
            {
                var target = R.GetTarget(Champ, TargetSeclect.LeastHP);
                if (target != null)
                {
                    var pred = R.GetPrediction(target);
                    if (pred.CanNext(R, MenuValue.General.RHitChance, true))
                    {
                        R.Cast(pred.CastPosition);
                    }
                }
            }
        }
    }
}
