using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karma.Modes
{
    class JungleClear : Karma
    {
        public static void Execute()
        {
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
