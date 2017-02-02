using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Amumu.Modes
{
    class Harass : Amumu
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady())
            {
                var target = W.GetTarget();
                if (target != null)
                {
                    if (MenuValue.Harass.WLogics == 0 || W.ToggleState != 2)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    //Turn off if no enemy
                    if (W.ToggleState.Equals(2))
                    {
                        W.Cast();
                    }
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
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
