using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karthus.Modes
{
    class LaneClear : Karthus
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var farmloc = Q.GetBestCircularCastPosition(Minion);
                    if (farmloc.HitNumber >= MenuValue.LaneClear.QHit)
                    {
                        Q.Cast(farmloc.CastPosition);
                    }
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var Minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Count() >= MenuValue.LaneClear.EHit)
                {
                    if (MenuValue.LaneClear.ELogics == 0 || E.ToggleState != 2)
                    {
                        E.Cast();
                    }
                }
                else
                {
                    //Turn off if no enemy
                    if (E.ToggleState.Equals(2))
                    {
                        E.Cast();
                    }
                }
            }
        }
    }
}
