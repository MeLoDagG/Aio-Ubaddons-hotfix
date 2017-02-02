using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Taliyah.Modes
{
    class LaneClear : Taliyah
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
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var Minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    var Location = W.GetBestCircularCastPosition(Minion, MenuValue.General.WHitChance);
                    var StartPos = E_Object != null ? Location.CastPosition.Extend(E_Object.Position, Location.CastPosition.Distance(E_Object.Position) + 200f).To3DWorld() : player.Position;
                    W.CastStartToEnd(StartPos, Location.CastPosition);
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var Minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (Minion.Any())
                {
                    E.Cast(Minion.First());
                }
            }
        }
    }
}
