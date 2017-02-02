using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Annie.Modes
{
    class JungleClear : Annie
    {
        public static void Execute()
        {
            if ((Has_Stun && MenuValue.LastHit.Stop) || (Passive_Count <= MenuValue.LastHit.Stopifhas)) return;
            if (player.Mana < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    Q.Cast(JungleMob.First());
                }
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                var JungleMob = W.GetJungleMobs();
                if (JungleMob.Any())
                {
                    W.Cast(JungleMob.First());
                }
            }
        }
    }
}
