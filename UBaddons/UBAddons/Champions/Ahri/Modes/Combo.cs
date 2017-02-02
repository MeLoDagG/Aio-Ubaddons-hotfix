using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using UBAddons.General;

namespace UBAddons.Champions.Ahri.Modes
{
    class Combo : Ahri
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health <= HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ)
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
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
                var target = W.GetTarget(-50);
                if (target != null)
                {
                    W.Cast();
                }
            }
            if (MenuValue.Combo.UseE)
            {
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    var pred = E.GetPrediction(target);
                    if (pred.CanNext(E, MenuValue.General.QHitChance, false))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseR)
            {
                var target = R.GetTarget(Champ, TargetSeclect.Default);
                if (target != null)
                {
                    DashLogic.DashPos(target);
                }
            }
        }
    }
}
