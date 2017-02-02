using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Anivia.Modes
{
    class Harass : Anivia
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady() && Q.ToggleState != 2 && Core.GameTickCount - LastQTick > 120)
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                        LastQTick = Core.GameTickCount;
                    }
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    if (QMissile == null)
                    {
                        var pred = W.GetPrediction(target);
                        W.Cast(pred.CastPosition);
                    }
                    else
                    {
                        if (target.Distance(QMissile) <= 300)
                        {
                            var pos = QMissile.Position.Extend(target, QMissile.Distance(target) + 10).To3DWorld();
                            if (W.IsInRange(pos))
                            {
                                W.Cast(pos);
                            }
                        }
                    }
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null && (target.HasBuff(IcedName) || !MenuValue.Harass.OnlyIced))
                {
                    E.Cast(target);
                }
            }
            if (MenuValue.Harass.UseR && R.IsReady() && Storm == null)
            {
                var target = R.GetTarget(Champ);
                if (target != null)
                {
                    var pred = R.GetPrediction(target);
                    R.Cast(pred.CastPosition);
                }
            }
        }
    }
}
