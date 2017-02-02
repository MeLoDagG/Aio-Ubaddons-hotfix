using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Xerath.Modes
{
    class JungleClear : Xerath
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    var farmLoc = Q.GetBestLinearCastPosition(JungleMob);
                    if (farmLoc.HitNumber > 0)
                    {
                        if (!Q.IsCharging)
                        {
                            Q.StartCharging();
                        }
                        if (Q.Cast(farmLoc.CastPosition))
                        {
                            return;
                        }
                    }
                }
            }
            if (!Q.IsCharging)
            {
                if (MenuValue.JungleClear.UseW && W.IsReady())
                {
                    var JungleMob = W.GetJungleMobs();
                    if (JungleMob.Any())
                    {
                        W.Cast(JungleMob.First());
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
            }
        }
    }
}
