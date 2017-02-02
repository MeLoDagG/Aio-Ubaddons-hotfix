using UBAddons.Libs;

namespace UBAddons.Champions.Taliyah.Modes
{
    class PermaActive : Taliyah
    {
        public static void Execute()
        {
            if (R.Level == 2 && R.Range < 4500)
            {
                R.Range = 4500;
            }
            if (R.Level == 3 && R.Range < 6000)
            {
                R.Range = 6000;
            }
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
                    CastW(Target);
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = E.GetKillableTarget();
                if (Target != null)
                {
                    E.Cast(Target);
                }
            }
        }
    }
}
