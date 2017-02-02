using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.TwistedFate.Modes
{
    class Combo : TwistedFate
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var target = Q.GetTarget(Champ);
            if (MenuValue.Combo.UseQ)
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
            LogicPickedCard(MenuValue.Combo.UseW, MenuValue.Combo.WLogic);
        }
    }
}
