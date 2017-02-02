using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;

namespace UBAddons.Champions.Cassiopeia.Modes
{
    class Combo : Cassiopeia
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ)
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
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false) && !pred.CastPosition.IsInRange(player, WMinRange))
                    {
                        W.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var poisoned = Champ.Where(x => x.HasBuffOfType(BuffType.Poison));
                var target = E.GetTarget(poisoned);
                if (target != null)
                {
                    if (target.HasBuffOfType(BuffType.Poison) || !MenuValue.Combo.PoisonOnly)
                    {
                        E.Cast(target);
                    }
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                var target = R.GetTarget(Champ, TargetSeclect.Default);
                if (target != null)
                {
                    if (MenuValue.Combo.RHit == 1)
                    {
                        if (target.IsFacing(player))
                        {
                            var pred = R.GetPrediction(target);
                            if (pred.CanNext(R, MenuValue.General.RHitChance, true))
                            {
                                R.Cast(pred.CastPosition);
                            }
                        }
                    }
                    else
                    {
                        if (target.IsFacing(player))
                        {
                            R.CastIfItWillHit(MenuValue.Combo.RHit, MenuValue.General.RHitChance);
                        }
                    }
                }
            }
        }
    }
}
