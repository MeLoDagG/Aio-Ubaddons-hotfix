using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Veigar.Modes
{
    class JungleClear : Veigar
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
                    W.Cast(mob.First());
                }
            }
        }
    }
}
