using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Alistar.Modes
{
    class PermaActive : Alistar
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    Q.Cast();
                }
            }
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var Target = W.GetKillableTarget();
                if (Target != null)
                {
                    W.Cast(Target);
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    E.Cast();
                }
            }
            if ((player.HealthPercent <= MenuValue.General.HP || MenuValue.General.EnemyCount >= player.CountEnemyChampionsInRange(500)) && R.IsReady())
            {
                R.Cast();
            }
        }
    }
}
