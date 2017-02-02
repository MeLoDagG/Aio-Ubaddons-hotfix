using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Viktor.Modes
{
    class Flee : Viktor
    {
        public static void Execute()
        {
            if (Q.IsReady() && QUpgrade)
            {
                var target = Q.GetTarget();
                if (target != null)
                {
                    Q.Cast(target);
                }
                else
                {
                    var target2 = !Q.GetLaneMinions().Any() ? Q.GetJungleMobs() : Q.GetLaneMinions();
                    if (target2.Any())
                    {
                        Q.Cast(target2.FirstOrDefault());
                    }
                }
            }
        }
    }
}
