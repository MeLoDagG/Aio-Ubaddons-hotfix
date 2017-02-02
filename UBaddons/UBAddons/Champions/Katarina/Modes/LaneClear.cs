using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Katarina.Modes
{
    class LaneClear : Katarina
    {
        public static void Execute()
        {
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    Q.Cast(Minion.First());
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                if (W.GetLaneMinions().Count() >= MenuValue.LaneClear.Ehit)
                {
                    W.Cast();
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                foreach(var dagger in Dagger.Keys)
                {
                    if (dagger.CountEnemyMinionsInRange(340) >= MenuValue.LaneClear.Ehit)
                    {
                        E.Cast(dagger.Position);
                    }
                }
            }
        }
    }
}
