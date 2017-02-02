using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kennen.Modes
{
    class Combo : Kennen
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.HasBuff("kennenmarkofstorm"));
            var Champ2 = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var target = MenuValue.General.QFocus ? Q.GetTarget(Champ) : Q.GetTarget(Champ2);
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

            if (MenuValue.Combo.UseW)
            {
                var Count = EntityManager.Heroes.Enemies.Count(x => x.IsValid && W.IsInRange(x) && x.HasBuff("kennenmarkofstorm"));
                if (Count >= MenuValue.Combo.Whit && W.IsReady())
                {
                    W.Cast();
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady() && (E.ToggleState == 1 || !player.HasBuff("KennenLightningRush")) && target.CountEnemyChampionsInRange((float)(player.MoveSpeed * 2).FixToCappedMovementSpeed()) > 0)
            {
                E.Cast();
            }

            if (MenuValue.Combo.UseR && R.IsReady())
            {
                var Count = player.CountEnemyHeroesInRangeWithPrediction((int)R.Range);
                if (Count >= MenuValue.Combo.Rhit)
                {
                    R.Cast();
                }
            }
        }
    }
}
