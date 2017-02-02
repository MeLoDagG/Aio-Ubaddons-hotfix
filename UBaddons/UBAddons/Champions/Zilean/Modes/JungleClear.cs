using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Zilean.Modes
{
    class JungleClear : Zilean
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            var monster = Q.GetJungleMobs();
            if (monster == null || !monster.Any()) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                Q.Cast(monster.First());
            }
            if (MenuValue.JungleClear.UseW && W.IsReady() && !Q.IsReady(1200))
            {
                W.Cast();
            }
        }
    }
}
