using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Malzahar.Modes
{
    class JungleClear : Malzahar
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var monster = Q.GetJungleMobs();
                if (monster.Any())
                {
                    Q.Cast(monster.First());
                }
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                var mob = W.GetJungleMobs();
                if (mob.Any())
                {
                    CastW(mob.First());
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var mob = E.GetJungleMobs();
                if (mob.Any())
                {
                    E.Cast(mob.First());
                }
            }
        }
    }
}
