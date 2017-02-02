using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Annie.Modes
{
    class LastHit : Annie
    {
        public static void Execute()
        {
            if ((Has_Stun && MenuValue.LastHit.Stop) || (Passive_Count <= MenuValue.LastHit.Stopifhas)) return;
            if (Q.IsReady() && MenuValue.LastHit.UseQ)
            {
                var minion = Q.GetLaneMinions(true);
                if (minion.Any())
                {
                    Q.Cast(minion.First());
                }
            }
        }
    }
}
