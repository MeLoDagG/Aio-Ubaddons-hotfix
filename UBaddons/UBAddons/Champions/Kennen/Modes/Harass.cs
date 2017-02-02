using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kennen.Modes
{
    class Harass : Kennen
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.HasBuff("kennenmarkofstorm"));
            var Champ2 = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var target = MenuValue.General.QFocus ? Q.GetTarget(Champ) : Q.GetTarget(Champ2);
            if (MenuValue.Harass.UseQ)
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

            if (MenuValue.Harass.UseW)
            {
                var Count = EntityManager.Heroes.Enemies.Count(x => x.IsValid && W.IsInRange(x) && x.HasBuff("kennenmarkofstorm"));
                if (Count >= MenuValue.Harass.Whit && W.IsReady())
                {
                    W.Cast();
                }
            }
        }
    }
}
