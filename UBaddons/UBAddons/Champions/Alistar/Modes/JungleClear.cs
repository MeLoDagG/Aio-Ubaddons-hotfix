using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Alistar.Modes
{
    class JungleClear : Alistar
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                if (Q.GetJungleMobs().Any())
                {
                    Q.Cast();
                }
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                var JungMob = W.GetJungleMobs();
                if (JungMob.Any())
                {
                    W.Cast(JungMob.First());
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var junglemobs = E.GetJungleMobs();
                if (junglemobs.Any())
                {
                    E.Cast();
                }
            }
        }
    }
}
