using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ziggs.Modes
{
    class Harass : Ziggs
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var target = Q3.GetTarget(Champ);
                if (target != null)
                {
                    CastQ3(target);
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady() && W.ToggleState != 2)
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    var pred = W.GetPrediction(target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false))
                    {
                        Core.DelayAction(() => Player.CastSpell(SpellSlot.W), W.CastDelay + (int)player.Distance(pred.CastPosition) / W.Speed);
                    }
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
