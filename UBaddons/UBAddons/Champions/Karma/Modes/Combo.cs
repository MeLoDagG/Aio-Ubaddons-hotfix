using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karma.Modes
{
    class Combo : Karma
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                if (E.IsReady())
                {
                    foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(E.Range)).OrderByDescending(x => x.CountAllyChampionsInRange(600)))
                    {
                        if (ally != null && ally.CountAllyChampionsInRange(600) > MenuValue.Combo.EShield)
                        {
                            R.Cast();
                        }
                    }
                }
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
                if (Wtarget != null && MenuValue.Combo.ShouldRW && W.IsReady())
                {
                    R.Cast();
                }
            }
            var DisableQ = (R.IsReady() || player.HasBuff("KarmaMantra")) && W.IsReady() && MenuValue.Combo.ShouldRW;
            if (MenuValue.Combo.UseQ && Q.IsReady() && !DisableQ)
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
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    W.Cast(target);
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                if (!player.HasBuff("KarmaMantra"))
                {
                    foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(E.Range) && x.HealthPercent <= 80).OrderBy(x => x.Health))
                    {
                        if (ally != null)
                        {
                            E.Cast(ally);
                        }
                    }
                }
                else
                {
                    foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(E.Range) && x.CountAllyChampionsInRange(600) > MenuValue.Combo.EShield).OrderBy(x => x.Health))
                    {
                        if (ally != null)
                        {
                            E.Cast(ally);
                        }
                    }
                }
            }
        }
    }
}
