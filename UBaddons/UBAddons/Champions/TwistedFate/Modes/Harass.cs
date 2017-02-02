using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.TwistedFate.Modes
{
    class Harass : TwistedFate
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var target = Q.GetTarget(Champ);
            if (MenuValue.Harass.UseQ)
            {
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            LogicPickedCard(MenuValue.Harass.UseW, MenuValue.Harass.WLogic);       
        }
    }
}
