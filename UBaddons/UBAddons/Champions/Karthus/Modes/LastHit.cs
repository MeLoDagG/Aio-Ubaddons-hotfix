using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karthus.Modes
{
    class LastHit : Karthus
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minion = Q.GetLaneMinions(true);
                if (Minion.Any())
                {
                    Q.Cast(Minion.First());
                }
            }
        }
    }
}
