using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Velkoz.Modes
{
    class Harass : Velkoz
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady() && !(Q.ToggleState == 2 || Q.Name.Equals("VelkozQSplitActivate")) && Core.GameTickCount - LastQTick > 120)
            {
                var target = Champ != null ? Q.GetTarget(Champ) : Q.GetTarget(400);
                if (target != null)
                {
                    QCast(target);
                    LastQTick = Core.GameTickCount;
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    var pred = W.GetPrediction(target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false))
                    {
                        W.Cast(pred.CastPosition);
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
