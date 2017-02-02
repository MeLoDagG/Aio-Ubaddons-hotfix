using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Twitch.Modes
{
    class JungleClear : Twitch
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.JungleClear.ManaLimit) return;
            var jungmobs = E.GetJungleMobs();
            if (!jungmobs.Any()) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                Q.Cast();
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                W.Cast(jungmobs.First());
            }
            if (MenuValue.JungleClear.UseE && E.IsReady() && jungmobs.Any(x => IsKillable(x, false)))
            {
                E.Cast();
            }
        }
    }
}
