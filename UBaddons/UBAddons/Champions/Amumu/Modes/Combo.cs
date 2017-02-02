using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Amumu.Modes
{
    class Combo : Amumu
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
                var target = W.GetTarget();
                //Turn on and turn off repeat
                if (target != null)
                {
                    if (MenuValue.Combo.WLogics == 0 || W.ToggleState != 2)
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
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var target = E.GetTarget();
                if (target != null)
                {
                    E.Cast();
                }                
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                var Count = player.CountEnemyHeroesInRangeWithPrediction((int)R.Range, R.CastDelay);
                if (Count >= MenuValue.Combo.RHit)
                {
                    R.Cast();
                }
            }
        }
    }
}
