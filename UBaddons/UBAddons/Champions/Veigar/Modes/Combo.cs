using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Veigar.Modes
{
    class Combo : Veigar
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
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    if (target.GetMovementBlockedDebuffDuration() > 0 || MenuValue.Combo.WLogic == 0)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.CanNext(W, MenuValue.General.WHitChance, false))
                        {
                            W.Cast(pred.CastPosition);
                        }
                    }
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                E.CastIfItWillHit(MenuValue.Combo.EHit, MenuValue.General.EHitChance);
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    CastE(target);
                }
            }
        }
    }
}
