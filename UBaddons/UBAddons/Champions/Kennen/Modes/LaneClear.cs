using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kennen.Modes
{
    class LaneClear : Kennen
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange) 
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;
            var Minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
            if (!Minion.Any()) return;
            if (MenuValue.LaneClear.UseE && E.IsReady() && (E.ToggleState == 1 || !player.HasBuff("KennenLightningRush")))
            {
                E.Cast();
            }
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                Q.Cast(Minion.First());
            }
            var minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
            if (MenuValue.LaneClear.UseW && W.IsReady() && minion.Any())
            {
                if (minion.Count(x => x.HasBuff("kennenmarkofstorm")) >= MenuValue.LaneClear.Whit)
                {
                    W.Cast();
                }
            }
        }
    }
}
