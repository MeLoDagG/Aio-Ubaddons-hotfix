using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kayle.Modes
{
    class Combo : Kayle
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var target = E.GetTarget();
                if (target != null)
                {
                    E.Cast();
                }
            }
        }
    }
}
