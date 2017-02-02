using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kassadin.Modes
{
    class Combo : Kassadin
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    var pred = E.GetPrediction(target);
                    if (pred.CanNext(E, MenuValue.General.EHitChance, true))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseR && player.HealthPercent > MenuValue.Combo.MyHP && R.IsReady())
            {
                var target = R.GetTarget(Champ);
                if (target != null && target.HealthPercent <= MenuValue.Combo.EnemyHP)
                {
                    var pred = R.GetPrediction(target);
                    if (pred.CanNext(R, MenuValue.General.EHitChance, true))
                    {
                        if (pred.CastPosition.CountEnemyChampionsInRange(500) <= 2 || Champ != null)
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
