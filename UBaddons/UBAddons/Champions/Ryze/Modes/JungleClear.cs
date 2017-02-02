using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ryze.Modes
{
    class JungleClear : Ryze
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.JungleClear.ManaLimit) return;
            bool JustQ = !Q.IsReady();
            if (MenuValue.JungleClear.UseQ && Q.IsReady())
            {
                var JungleMob = Q.GetJungleMobs();
                if (JungleMob.Any())
                {
                    Q.Cast(JungleMob.First());
                    JustQ = true;
                }
            }
            if (MenuValue.JungleClear.UseW && W.IsReady() && JustQ && !Q.IsReady() && !E.IsReady())
            {
                var JungleMob = W.GetJungleMobs();
                if (JungleMob.Any())
                {
                    W.Cast(JungleMob.First());
                    JustQ = false;
                }
            } 
            if (MenuValue.JungleClear.UseE && E.IsReady() && JustQ && !Q.IsReady())
            {
                var JungleMob = E.GetJungleMobs();
                if (JungleMob.Any())
                {
                    E.Cast(JungleMob.First());
                    JustQ = false;
                }
            }
        }
    }
}
