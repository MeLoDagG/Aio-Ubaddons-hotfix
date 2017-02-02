using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Brand.Modes
{
    class Combo : Brand
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null && (HasPassiveBuff(target) || !MenuValue.Combo.OnlyStun))
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }              
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null && (HasPassiveBuff(target) || !MenuValue.Combo.OnlyExtra))
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
                if (target != null && (HasPassiveBuff(target) || !MenuValue.Combo.OnlySpread))
                {
                    E.Cast(target);
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                var target = R.GetTarget();
                if (target != null && (EntityManager.Heroes.Enemies.Count(x => x.Distance(target) < 500 && x.IsValidTarget() && (x.HasBuff(BrandDetonate) || x.HasBuff(BrandPassive))) >= MenuValue.Combo.RCount) 
                    || target.IsKillable(SpellSlot.R))
                {
                    R.Cast(target);
                }
            }
        }
    }
}
