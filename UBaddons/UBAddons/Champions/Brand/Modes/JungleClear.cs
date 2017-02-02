using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Brand.Modes
{
    class JungleClear : Brand
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
            if (MenuValue.JungleClear.UseE && W.IsReady())
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
