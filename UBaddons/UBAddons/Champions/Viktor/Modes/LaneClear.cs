using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Viktor.Modes
{
    class LaneClear : Viktor
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    Q.Cast(minion.First());
                }
            }
            if (MenuValue.LaneClear.UseW && WUpgrade && W.IsReady())
            {
                W.CastOnBestFarmPosition(MenuValue.LaneClear.Whit, MenuValue.General.WHitChance);
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    var creep = minion.OrderBy(x => x.CountEnemyMinionsInRange(500)).FirstOrDefault();
                    var source = E1.IsInRange(creep) ? creep.Position.To2D() : player.Position.Extend(creep, E1.Range - 5);
                    var line = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minion, E.Width, 495, source);
                    if (line.HitNumber > MenuValue.LaneClear.Ehit)
                    {
                        E.CastStartToEnd(line.CastPosition, source.To3DWorld());
                    }
                }
            }
        }
    }
}
