using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Anivia.Modes
{
    class JungleClear : Anivia
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady() && Q.ToggleState != 2 && Core.GameTickCount - LastQTick > 120)
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    Q.Cast(JungleMob.First()); 
                    LastQTick = Core.GameTickCount;
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var JungleMob = E.GetJungleMobs();
                if (JungleMob.Any())
                {
                    E.Cast(JungleMob.First());
                }
            }
            if (MenuValue.JungleClear.UseR && R.IsReady())
            {
                var JungleMob = R.GetJungleMobs();
                if (JungleMob.Any())
                {
                    R.Cast(JungleMob.First());
                }
            }
        }
    }
}
