using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Lulu.Modes
{
    class Combo : Lulu
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                var target2 = QPix.GetTarget(Champ);
                if (target != null || target2 != null)
                {
                    PredictionResult pred = null;
                    PredictionResult pred2 = null;
                    if (target != null)
                    {
                        pred = Q.GetPrediction(target);
                    }
                    if (target2 != null)
                    {
                        pred2 = QPix.GetPrediction(target2);
                    }
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false) || pred2.CanNext(QPix, MenuValue.General.QHitChance, false))
                    {
                        if (pred.HitChance > pred2.HitChance)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                        else
                        {
                            QPix.Cast(pred2.CastPosition);
                        }
                        
                    }
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                switch(MenuValue.Combo.Wlogic)
                {
                    case 0:
                        {
                            var target = EntityManager.Heroes.Allies.Where(x => x.IsAlly && x.IsValidTarget(W.Range) && !x.IsDead && !x.IsZombie).OrderByDescending(x => x.TotalAttackDamage).FirstOrDefault();
                            if (target != null)
                            {
                                W.Cast(target);
                            }
                        }
                        break;
                    case 1:
                        {
                            var target = W.GetTarget(Champ);
                            if (target != null)
                            {
                                W.Cast(target);
                            }
                        }
                        break;
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                switch (MenuValue.Combo.Elogic)
                {
                    case 0:
                        {
                            var target = EntityManager.Heroes.Allies.Where(x => x.IsAlly && x.IsValidTarget(E.Range) && !x.IsDead && !x.IsZombie).OrderBy(x => x.Health).FirstOrDefault();
                            if (target != null)
                            {
                                E.Cast(target);
                            }
                        }
                        break;
                    case 1:
                        {
                            var target = E.GetTarget(Champ);
                            if (target != null)
                            {
                                E.Cast(target);
                            }
                        }
                        break;
                    case 2:
                        {
                            var target = E.GetTarget(Champ);
                            if (target != null)
                            {
                                var whomoreas = EntityManager.Heroes.Allies.Where(x => x.IsAlly && x.IsValidTarget(W.Range)).OrderByDescending(x => x.AttackSpeedMod).FirstOrDefault();
                                if (HandleDamageIndicator(target, SpellSlot.E) > HowAA(whomoreas, 6) * PixDamage(target))
                                {
                                    E.Cast(target);
                                }
                                else
                                {
                                    E.Cast(whomoreas);
                                }
                            }
                        }
                        break;
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(R.Range)))
                {
                    if (ally != null)
                    {
                        if (ally.CountEnemyHeroesInRangeWithPrediction(350, R.CastDelay) >= MenuValue.Combo.Rhit)
                        {
                            R.Cast(ally);
                        }
                    }
                }
            }
        }
    }
}
