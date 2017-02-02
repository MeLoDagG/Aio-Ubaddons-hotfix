using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ryze.Modes
{
    class Harass : Ryze
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var JustQ = !Q.IsReady();
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                        JustQ = true;
                    }
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady() & !Q.IsReady() && JustQ)
            {
                var target = W.GetTarget(Champ);
                if (target != null && !target.HasBuff("RyzeE"))
                {
                    W.Cast(target);
                    JustQ = false;
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady() & !Q.IsReady() && JustQ && (!W.IsReady() || Math.Abs(player.PercentCooldownMod) >= 35))
            {
                var Eobj = (from obj in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget(E.Range) && (x.HasBuff("RyzeE") || x.Health < HandleDamageIndicator(x, SpellSlot.E)))
                            let champ = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(300, false, obj.Position) && x.Health < HandleDamageIndicator(x))
                            let target = TargetSelector.GetTarget(champ, DamageType.Magical)
                            where target != null
                            select obj).FirstOrDefault();
                if (Eobj != null)
                {
                    E.Cast(Eobj);
                    JustQ = false;
                }
                else
                {
                    var target = E.GetTarget(Champ);
                    if (target != null)
                    {
                        E.Cast(target);
                        JustQ = false;
                    }
                }
            }
        }
    }
}
