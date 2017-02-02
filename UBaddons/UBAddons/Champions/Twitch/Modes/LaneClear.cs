using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Twitch.Modes
{
    class LaneClear : Twitch
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            if (W.IsReady() && MenuValue.LaneClear.UseW)
            {
                W.CastOnBestFarmPosition(MenuValue.LaneClear.WHit);
            }
            if (E.IsReady() && MenuValue.LaneClear.UseE)
            {
                var Minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable).Count(x => MenuValue.LaneClear.OnlyKillable ? IsKillable(x, false) : GetECount(x) > 4);
                if (Minion >= MenuValue.LaneClear.EHit)
                {
                    E.Cast();
                }
            }
        }
    }
}
