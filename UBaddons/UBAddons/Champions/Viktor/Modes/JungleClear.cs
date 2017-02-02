using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Viktor.Modes
{
    class JungleClear : Viktor
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
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var mob = E.GetJungleMobs();
                if (mob.Any())
                {
                    var creep = mob.OrderBy(x => x.Distance(player)).ToArray();
                    var source = E1.IsInRange(creep[0]) ? creep[0].Position.To2D() : player.Position.Extend(creep[0], E1.Range - 10);
                    var line = EntityManager.MinionsAndMonsters.GetLineFarmLocation(mob, E.Width, 500, source);
                    E.CastStartToEnd(line.CastPosition, source.To3DWorld());
                }
            }
        }
    }
}
