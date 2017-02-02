using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ekko.Modes
{
    class LaneClear : Ekko
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
            if (Minion.Any() && MenuValue.LaneClear.UseQ && Q.IsReady())
            {                
                Q.Cast(Minion.First());                
            }
            var minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
            if (MenuValue.LaneClear.UseE && E.IsReady() && minion.Any())
            {
                E.Cast(player.Position.Extend(Game.CursorPos, E.Range).To3DWorld());
            }
        }
    }
}
