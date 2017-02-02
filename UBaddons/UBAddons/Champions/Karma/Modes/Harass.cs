using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karma.Modes
{
    class Harass : Karma
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseR && R.IsReady())
            {
                var Qtarget = Q.GetTarget(Champ);
                if (Qtarget != null && !MenuValue.Combo.ShouldRW && MenuValue.Combo.UseRQ && Q.IsReady())
                {
                    var pred = Q.GetPrediction(Qtarget);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        R.Cast();
                    }
                }
                var Wtarget = W.GetTarget(Champ);
                if (Wtarget != null && MenuValue.Harass.ShouldRW && W.IsReady())
                {
                    R.Cast();
                }
            }
            var DisableQ = (R.IsReady() || player.HasBuff("KarmaMantra")) && W.IsReady() && MenuValue.Combo.ShouldRW;
            if (MenuValue.Harass.UseQ && Q.IsReady() && !DisableQ)
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
                    W.Cast(target);
                }
            }
        }
    }
}
