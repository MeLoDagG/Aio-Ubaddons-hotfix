using System.Linq;
using UBMiddle.Libs;

namespace UBMiddle.Champions.Camille.Modes
{
    class JungleClear : Camille
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    Q.Cast(JungleMob.First());
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
            if (MenuValue.JungleClear.UseR && R.IsReady() && RStack < MenuValue.JungleClear.RStack)
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
