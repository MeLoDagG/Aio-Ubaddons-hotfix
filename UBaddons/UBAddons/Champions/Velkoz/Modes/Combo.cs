using EloBuddy.SDK;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;

namespace UBAddons.Champions.Velkoz.Modes
{
    class Combo : Velkoz
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady() && !(Q.ToggleState == 2 || Q.Name.Equals("VelkozQSplitActivate")) && Core.GameTickCount - LastQTick > 120)
            {
                var target = Champ != null ? Q.GetTarget(Champ) : Q.GetTarget(400);
                if (target != null)
                {
                    QCast(target);
                    LastQTick = Core.GameTickCount;
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    var pred = W.GetPrediction(target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false))
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
            if (MenuValue.Combo.UseR && R.IsReady())
            {
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
