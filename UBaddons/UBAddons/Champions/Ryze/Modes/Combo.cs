using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;

namespace UBAddons.Champions.Ryze.Modes
{
    class Combo : Ryze
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            var JustQ = !Q.IsReady();
            switch (MenuValue.Combo.ComboStyle)
            {
                case 0:
                    {
                        if (MenuValue.Combo.UseQ && Q.IsReady())
                        {
                            var target = Q.GetTarget(Champ, TargetSeclect.SpellTarget);
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
                        if (MenuValue.Combo.UseW && W.IsReady() & !Q.IsReady() && JustQ)
                        {
                            var target = W.GetTarget(Champ, TargetSeclect.SpellTarget);
                            if (target != null && !target.HasBuff("RyzeE"))
                            {
                                W.Cast(target);
                                JustQ = false;
                            }
                        }
                        if (MenuValue.Combo.UseE && E.IsReady() & !Q.IsReady() && JustQ && (!W.IsReady() || Math.Abs(player.PercentCooldownMod) >= 35))
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
                                var target = E.GetTarget(Champ, TargetSeclect.SpellTarget);
                                if (target != null)
                                {
                                    E.Cast(target);
                                    JustQ = false;
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    {                        
                        if (MenuValue.Combo.UseQ && Q.IsReady() &&
                            (player.HasBuff("RyzeQIconFullCharge") || player.HasBuff("RyzeQIconNoCharge")))
                        {
                            var target = Q.GetTarget(Champ, TargetSeclect.SpellTarget);
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
                        if (MenuValue.Combo.UseW && W.IsReady())
                        {
                            var target = W.GetTarget(Champ, TargetSeclect.SpellTarget);
                            if (target != null && target.HasBuff("RyzeE"))
                            {
                                W.Cast(target);
                                JustQ = false;
                            }
                        }
                        if (MenuValue.Combo.UseE && E.IsReady())
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
                                var target = E.GetTarget(Champ, TargetSeclect.SpellTarget);
                                if (target != null)
                                {
                                    E.Cast(target);
                                    JustQ = false;
                                }
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        if (player.HealthPercent <= MenuValue.Combo.HP)
                        {
                            if (MenuValue.Combo.UseQ && Q.IsReady() &&
                               (player.HasBuff("RyzeQIconFullCharge") || player.HasBuff("RyzeQIconNoCharge")))
                            {
                                var target = Q.GetTarget(Champ, TargetSeclect.SpellTarget);
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
                            if (MenuValue.Combo.UseW && W.IsReady())
                            {
                                var target = W.GetTarget(Champ, TargetSeclect.SpellTarget);
                                if (target != null && target.HasBuff("RyzeE"))
                                {
                                    W.Cast(target);
                                    JustQ = false;
                                }
                            }
                            if (MenuValue.Combo.UseE && E.IsReady())
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
                                    var target = E.GetTarget(Champ, TargetSeclect.SpellTarget);
                                    if (target != null)
                                    {
                                        E.Cast(target);
                                        JustQ = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (MenuValue.Combo.UseQ && Q.IsReady())
                            {
                                var target = Q.GetTarget(Champ, TargetSeclect.SpellTarget);
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
                            if (MenuValue.Combo.UseW && W.IsReady() & !Q.IsReady() && JustQ)
                            {
                                var target = W.GetTarget(Champ, TargetSeclect.SpellTarget);
                                if (target != null && !target.HasBuff("RyzeE"))
                                {
                                    W.Cast(target);
                                    JustQ = false;
                                }
                            }
                            if (MenuValue.Combo.UseE && E.IsReady() & !Q.IsReady() && JustQ && (!W.IsReady() || Math.Abs(player.PercentCooldownMod) >= 35))
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
                                    var target = E.GetTarget(Champ, TargetSeclect.SpellTarget);
                                    if (target != null)
                                    {
                                        E.Cast(target);
                                        JustQ = false;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }            
        }
    }
}
