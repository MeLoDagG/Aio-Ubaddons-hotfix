﻿using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using UBAddons.General;

namespace UBAddons.Champions.Syndra.Modes
{
    class Combo : Syndra
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var Target = Q.GetTarget(Champ, TargetSeclect.SpellTarget);
                if (Target != null)
                {
                    var pred = Q.GetPrediction(Target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var Target = W.GetTarget(Champ, TargetSeclect.SpellTarget);
                if (Target != null)
                {
                    TakeShit_and_Cast(Target);
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var Target = EQ.GetTarget(Champ, TargetSeclect.SpellTarget);
                if (Target != null)
                {
                    CastE(Target, MenuValue.Combo.UseQE, MenuValue.Combo.UseWE);
                }
            }
        }
    }
}
