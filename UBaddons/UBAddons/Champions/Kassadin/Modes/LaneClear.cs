using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kassadin.Modes
{
    class LaneClear : Kassadin
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
                    Q.Cast(Minion.First());
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                E.CastOnBestFarmPosition(MenuValue.LaneClear.Ehit, MenuValue.General.EHitChance);
            }
            if (MenuValue.LaneClear.UseR && R.IsReady() && RStack < MenuValue.LaneClear.RStack)
            {
                R.CastOnBestFarmPosition(MenuValue.LaneClear.Rhit, MenuValue.General.RHitChance);
            }
        }
    }
}
