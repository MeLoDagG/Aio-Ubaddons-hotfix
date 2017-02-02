using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ahri.Modes
{
    class JungleClear : Ahri
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                Q.CastOnBestFarmPosition(MenuValue.JungleClear.QHit, MenuValue.General.QHitChance);
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                var JungMob = W.GetJungleMobs();
                if (JungMob.Any())
                {
                    W.Cast();
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var junglemobs = E.GetJungleMobs();
                if (junglemobs.Any())
                {
                    E.Cast(junglemobs.First());
                }
            }
        }
    }
}
