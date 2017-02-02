using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kennen.Modes
{
    class PermaActive : Kennen
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
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var Target = W.GetKillableTarget();
                if (Target != null)
                {
                    W.Cast();
                }
            }
            if (E.GetTarget() != null && E.IsReady() && (E.ToggleState == 1 || !player.HasBuff("KennenLightningRush")))
            {
                E.Cast();
            }
        }
    }
}
