using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Amumu.Modes
{
    class JungleClear : Amumu
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.JungleClear.ManaLimit) return;
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    Q.Cast(JungleMob.First());
                }
            }
            if (MenuValue.JungleClear.UseW && W.IsReady())
            {
                var JungleMob = W.GetJungleMobs();
                if (JungleMob.Any())
                {
                    if (MenuValue.JungleClear.WLogics == 0 || E.ToggleState != 2)
                    {
                        E.Cast();
                    }
                }
                else
                {
                    //Turn off if no monster
                    if (W.ToggleState.Equals(2))
                    {
                        W.Cast();
                    }
                }
            }
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var JungleMob = E.GetJungleMobs();
                if (JungleMob.Any())
                {
                    E.Cast();
                }
            }
        }
    }
}
