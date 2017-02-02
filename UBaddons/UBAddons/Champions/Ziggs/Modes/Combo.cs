using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using UBAddons.Libs.Dictionary;

namespace UBAddons.Champions.Ziggs.Modes
{
    class Combo : Ziggs
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q3.GetTarget(Champ);
                if (target != null)
                {                   
                    CastQ3(target);
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady() && W.ToggleState != 2)
            {
                var target = W.GetTarget(Champ);
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
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                R.CastIfItWillHit(MenuValue.Combo.RHit, MenuValue.General.RHitChance);
                var target = R.GetKillableTarget();
                if (target != null && !target.BrainIsCharged())
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
