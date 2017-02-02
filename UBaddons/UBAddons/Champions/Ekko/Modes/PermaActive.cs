using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ekko.Modes
{
    class PermaActive : Ekko
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    var pred = Q.GetPrediction(Target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = TargetSelector.GetTarget(
                    EntityManager.Heroes.Enemies.Where(t => t != null
                        && t.IsValidTarget()
                        && !t.HasUndyingBuff(true)
                        && !t.HasBuffOfType(BuffType.SpellImmunity)
                        && t.IsInRange(player, Q.Range + E.Range)
                        && t.Health <= DamageIndicator.DamageDelegate(t, SpellSlot.R)), DamageType.Magical);
                if (Target != null)
                {
                    E.Cast(player.Position.Extend(Target, E.Range).To3DWorld());
                }
            }
            if (Ekko_Kage_Bunshin != null && R.IsReady())
            {
                if (MenuValue.Misc.RKS && R.IsReady())
                {
                    var Target = TargetSelector.GetTarget(
                        EntityManager.Heroes.Enemies.Where(t => t != null
                            && t.IsValidTarget()
                            && !t.HasUndyingBuff(true)
                            && !t.HasBuffOfType(BuffType.SpellImmunity)
                            && t.IsInRange(Ekko_Kage_Bunshin, R.Range)
                            && t.Health <= DamageIndicator.DamageDelegate(t, SpellSlot.R)), DamageType.Magical);
                    if (Target != null)
                    {
                        var pred = Prediction.Position.PredictUnitPosition(Target, 500);
                        if (pred.IsInRange(Ekko_Kage_Bunshin, R.Range))
                        {
                            R.Cast();
                        }
                    }
                }
            }
            if (Orbwalker.ActiveModes.Combo.IsOrb() || !MenuValue.Auto.NotCombo)
            {
                if (Ekko_Kage_Bunshin.CountEnemyHeroesInRangeWithPrediction((int)R.Range, 500) >= MenuValue.Auto.ChampHit)
                {
                    R.Cast();
                }
                if ((MenuValue.Auto.EnablePred ? Prediction.Health.GetPrediction(player, 500) : player.Health) < MenuValue.Auto.HP)
                {
                    R.Cast();
                }
            }
        }
    }
}
