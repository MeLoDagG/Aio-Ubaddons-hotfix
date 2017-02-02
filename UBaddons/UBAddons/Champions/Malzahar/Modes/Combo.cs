using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using UBAddons.General;

namespace UBAddons.Champions.Malzahar.Modes
{
    class Combo : Malzahar
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
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
                var target = W.GetTarget(250);
                if (target != null)
                {
                    CastW(target);
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    E.Cast(target);
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                var target = R.GetTarget(Champ, TargetSeclect.Default);
                if (target != null && player.CountAllyChampionsInRange(1000) >= player.CountEnemyChampionsInRange(1000))
                {
                    R.Cast(target);
                }
            }
        }
    }
}
