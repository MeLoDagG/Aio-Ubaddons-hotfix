using EloBuddy;
using SharpDX;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ziggs.Modes
{
    class PermaActive : Ziggs
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var target = Q3.GetKillableTarget();
                if (target != null)
                {
                    CastQ3(target);
                }
            }
            if (MenuValue.Misc.WKS && W.IsReady() && W.ToggleState != 2)
            {
                var target = W.GetKillableTarget();
                if (target != null)
                {
                    var pred = W.GetPrediction(target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false))
                    {
                        if (W.Cast(pred.CastPosition))
                        {
                            Core.DelayAction(() => Player.CastSpell(SpellSlot.W), W.CastDelay + (int)player.Distance(pred.CastPosition) / W.Speed);
                        }
                    }
                }
                var turret = EntityManager.Turrets.Enemies.Where(x => x.IsValidTarget(W.Range) && x.HealthPercent < 22.5 + 2.5 * W.Level).FirstOrDefault();
                if (turret != null)
                {
                    if (W.Cast(turret.Position))
                    {
                        Core.DelayAction(() => Player.CastSpell(SpellSlot.W), W.CastDelay + (int)player.Distance(turret.Position) / W.Speed);
                    }
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    var pred = E.GetPrediction(target);
                    if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
            {
                var target = R.GetKillableTarget();
                if (target != null && !Q.IsInRange(target))
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
