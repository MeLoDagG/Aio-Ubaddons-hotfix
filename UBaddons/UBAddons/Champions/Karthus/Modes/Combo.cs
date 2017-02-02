using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karthus.Modes
{
    class Combo : Karthus
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    var pred = W.GetPrediction(target);
                    W.Cast(pred.CastPosition);
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var target = E.GetTarget();
                //Turn on and turn off repeat
                if (target != null)
                {
                    if (MenuValue.JungleClear.ELogics == 0 || E.ToggleState != 2)
                    {
                        E.Cast();
                    }
                }
                else
                {
                    //Turn off if no enemy
                    if (E.ToggleState.Equals(2))
                    {
                        E.Cast();
                    }
                }
            }
        }
    }
}
