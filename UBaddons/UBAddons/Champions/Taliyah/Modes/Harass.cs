using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Taliyah.Modes
{
    class Harass : Taliyah
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
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    CastW(target, MenuValue.Combo.WLogic);
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    var pred = E.GetPrediction(target);
                    if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }  
        }
    }
}
