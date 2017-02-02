using EloBuddy.SDK;
using EloBuddy;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ekko.Modes
{
    class JungleClear : Ekko
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            var JungleMob = Q.GetJungleMobs();
            if (JungleMob == null || !JungleMob.Any()) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                Q.Cast(JungleMob.First());
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                W.Cast(JungleMob.First());
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                E.Cast(player.Position.Extend(Game.CursorPos, E.Range).To3DWorld());
            }
        }
    }
}
