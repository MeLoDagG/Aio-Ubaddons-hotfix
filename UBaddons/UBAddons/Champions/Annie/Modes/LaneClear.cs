using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Annie.Modes
{
    class LaneClear : Annie
    {
        public static void Execute()
        {
            if ((Has_Stun && MenuValue.LastHit.Stop) || (Passive_Count <= MenuValue.LastHit.Stopifhas)) return;
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
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
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var farmloc = W.GetBestConeCastPosition(Minion, MenuValue.General.WHitChance);
                    if (farmloc.HitNumber > MenuValue.LaneClear.Whit)
                    {
                        W.Cast(farmloc.CastPosition);
                    }
                }
            }
        }
    }
}
