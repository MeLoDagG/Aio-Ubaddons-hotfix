using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Malzahar.Modes
{
    class PermaActive : Malzahar
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var target = Q.GetKillableTarget();
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                //get target E is better than
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    CastW(target);
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    E.Cast(target);
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
            {
                var target = R.GetKillableTarget();
                if (target != null && player.CountAllyChampionsInRange(1000) >= player.CountEnemyChampionsInRange(1000))
                {
                    R.Cast(target);
                }
            }
        }
    }
}
