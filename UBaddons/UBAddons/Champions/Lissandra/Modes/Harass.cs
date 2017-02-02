using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Lissandra.Modes
{
    class Harass : Lissandra
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health <= HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var Target = Q.GetTarget(Champ);
                if (Target != null)
                {
                    CastQ(Target);
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady())
            {
                var Target = W.GetTarget();
                if (Target != null)
                {
                    W.Cast();
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var Target = TargetSelector.GetTarget(200, DamageType.Magical, Game.CursorPos);
                if (Target != null && E.IsInRange(Target))
                {
                    CastE(Target, true);
                }
            }
        }
    }
}
