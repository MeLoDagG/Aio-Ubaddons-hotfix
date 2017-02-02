using EloBuddy.SDK;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Libs.Dictionary;

namespace UBAddons.Champions.Viktor.Modes
{
    class Combo : Viktor
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
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                W.CastIfItWillHit(MenuValue.Combo.Whit, MenuValue.General.WHitChance);
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    CastE(target);
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                R.CastIfItWillHit(MenuValue.Combo.Rhit, MenuValue.General.RHitChance);
                var target = R.GetTarget(Champ, TargetSeclect.Default);
                if (target != null)
                {
                    if (target.BrainIsCharged())
                    {
                        return;
                    }
                    else
                    {
                        var pred = R.GetPrediction(target);
                        if (pred.CanNext(R, MenuValue.General.RHitChance, true))
                        {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
