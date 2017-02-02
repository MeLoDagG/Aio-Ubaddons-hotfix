using EloBuddy.SDK;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;

namespace UBAddons.Champions.Annie.Modes
{
    class Combo : Annie
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
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
            if (MenuValue.Combo.UseR && R.IsReady() && (Passive_Count >= 3 || Has_Stun))
            {
                R.CastIfItWillHit(MenuValue.Combo.RHit, MenuValue.General.RHitChance);
                var target = R.GetTarget(Champ, TargetSeclect.Default);
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
