using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Syndra.Modes
{
    class JungleClear : Syndra
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
            if (MenuValue.JungleClear.UseW && W.IsReady() && !Q.IsReady() && !E.IsReady())
            {
                var JungleMob = W.GetJungleMobs();
                if (JungleMob.Any())
                {
                    TakeShit_and_Cast(JungleMob.First());
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady() && !Q.IsReady())
            {
                var JungleMob = E.GetJungleMobs();
                if (JungleMob.Any())
                {
                    E.Cast(JungleMob.First());
                }
            }
        }
    }
}
