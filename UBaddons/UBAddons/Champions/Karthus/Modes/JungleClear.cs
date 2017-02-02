using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karthus.Modes
{
    class JungleClear : Karthus
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
            if (MenuValue.JungleClear.UseE && E.IsReady())
            {
                var JungleMob = E.GetJungleMobs();
                if (JungleMob.Any())
                {
                    if (MenuValue.JungleClear.ELogics == 0 || E.ToggleState != 2)
                    {
                        E.Cast();
                    }
                }
                else
                {
                    //Turn off if no monster
                    if (E.ToggleState.Equals(2))
                    {
                        E.Cast();
                    }
                }
            }
        }
    }
}
