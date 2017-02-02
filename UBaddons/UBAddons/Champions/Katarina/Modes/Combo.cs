using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;

namespace UBAddons.Champions.Katarina.Modes
{
    class Combo : Katarina
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
                switch (KilledAnEnemy)
                {
                    case false:
                        {
                            if (MenuValue.Combo.EToEnemyChamp)
                            {
                                var target = E.GetTarget(Champ);
                                if (target != null && !target.IsUnderHisturret())
                                {
                                    if (Dagger.Any(x => x.Key.Distance(target) < 400 && x.Value.Item1))
                                    {
                                        //var dagger = Dagger.Keys.OrderBy(x => x.Distance(target));
                                        if (MenuValue.Combo.UseW && W.IsReady())
                                        {
                                            if (MenuValue.Combo.Safe)
                                            {
                                                if (W.Cast())
                                                {
                                                    var pred = E.GetPrediction(target);
                                                    E.Cast(pred.CastPosition);
                                                }
                                            }
                                            else
                                            {
                                                var pred = E.GetPrediction(target);
                                                E.Cast(pred.CastPosition);
                                            }
                                        }
                                        else
                                        {
                                            var pred = E.GetPrediction(target);
                                            E.Cast(pred.CastPosition);
                                        }
                                    }
                                }
                            }
                            if (MenuValue.Combo.EToDagger)
                            {
                                var dagger = Dagger.Where(x => x.Value.Item1).OrderBy(x => x.Key.Distance(TargetSelector.GetTarget(500, DamageType.Magical, x.Key.Position, true)));
                                if (dagger.Any() && (!dagger.FirstOrDefault().Key.Position.IsUnderEnemyTurret() || !MenuValue.Combo.DontETurret
                                    || TargetSelector.GetTarget(500, DamageType.Magical, dagger.FirstOrDefault().Key.Position).Health <
                                    HandleDamageIndicator(TargetSelector.GetTarget(500, DamageType.Magical, dagger.FirstOrDefault().Key.Position))))
                                {
                                    switch (dagger.Count())
                                    {
                                        case 1:
                                            {
                                                if (MenuValue.Combo.UseW && W.IsReady())
                                                {
                                                    if (MenuValue.Combo.Safe)
                                                    {
                                                        if (W.Cast())
                                                        {
                                                            E.Cast(dagger.FirstOrDefault().Key.Position);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        E.Cast(dagger.FirstOrDefault().Key.Position);
                                                    }
                                                }
                                                else
                                                {
                                                    E.Cast(dagger.FirstOrDefault().Key.Position);
                                                }
                                            }
                                            break;
                                        default:
                                            {
                                                E.Cast(dagger.FirstOrDefault().Key.Position);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case true:
                        {
                            switch (MenuValue.Combo.ENextLogic)
                            {
                                case 0:
                                    {
                                        var target = E.GetTarget(Champ, TargetSeclect.Default);
                                        if (target != null)
                                        {
                                            var pred = E.GetPrediction(target);
                                            E.Cast(pred.CastPosition);
                                        }
                                        else
                                        {
                                            if (!player.Position.IsSafePosition())
                                            {
                                                if (Dagger.Keys.Any(x => E.IsInRange(x.Position)))
                                                {
                                                    E.Cast(Dagger.Keys.FirstOrDefault(x => x.Position.IsSafePosition()).Position);
                                                }
                                                else
                                                {
                                                    var ally = EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(E.Range) && x.Position.IsSafePosition()).OrderByDescending(x => x.Distance(player)).First();
                                                    if (ally != null)
                                                    {
                                                        var pred = E.GetPrediction(ally);
                                                        E.Cast(pred.CastPosition);
                                                    }
                                                    else
                                                    {
                                                        var minion = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(x => x.IsValidTarget(E.Range) && x.Position.IsSafePosition()).OrderByDescending(x => x.Distance(player)).First();
                                                        if (minion != null)
                                                        {
                                                            E.Cast(minion);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Dagger.Keys.Any(x => E.IsInRange(x.Position)))
                                                {
                                                    E.Cast(Dagger.Keys.FirstOrDefault(x => x.Position.IsSafePosition()).Position);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        var unit = ObjectManager.Get<Obj_AI_Base>().OrderBy(x => x.Distance(Game.CursorPos)).First();
                                        Modes.Flee.FleeTo();
                                    }
                                    break;
                                case 2:
                                    {
                                        var target = E.GetTarget(Champ);
                                        if (target != null)
                                        {
                                            E.Cast(target);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget(Champ);
                if (target != null)
                {
                    W.Cast();
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                var target = R.GetTarget(Champ, TargetSeclect.Default);
                if (target != null)
                {
                    if (W.IsReady() && MenuValue.Combo.UseW)
                    {
                        //E ready => no need W for refesh
                        if (E.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            if (W.Cast())
                            {
                                R.Cast();
                            }
                        }
                    }
                    else
                    {
                        R.Cast();
                    }
                }
            }
        }
    }
}
