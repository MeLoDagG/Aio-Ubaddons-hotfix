using System;
using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using SharpDX;

namespace UBAddons.Champions.Twitch.Modes
{
    class Combo : Twitch
    {
        public static void Execute()
        {
            if (MenuValue.Combo.UseQ && Q.IsReady() && !player.HasBuff("twitchhideinshadowsbuff"))
            {
                var target = TargetSelector.GetTarget(300, DamageType.Mixed, Game.CursorPos);
                if (target != null && player.Distance(target) <= 1500)
                {
                    Q.Cast();
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var target = W.GetTarget();
                if (target != null)
                {
                    var pred = W.GetPrediction(target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false))
                    {
                        W.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var Enemies = EntityManager.Heroes.Enemies;
                switch (MenuValue.Combo.ELogic)
                {
                    case 0:
                        {
                            if (Enemies.Any(x => IsKillable(x, false)))
                            {
                                E.Cast();
                            }
                        }
                        break;
                    case 1:
                        {
                            if (Math.Max(EntityManager.Heroes.Allies.Count(), Enemies.Count()) >= MenuValue.Combo.HeroCount)
                            {
                                if (Enemies.Any(x => IsKillable(x, false)))
                                {
                                    E.Cast();
                                }
                            }
                            else
                            {

                                if (Enemies.Count(x => IsKillable(x, false)) < 2 && Enemies.Count(x => IsNearToKillable(x, true)) >= 1)
                                {
                                    Orbwalker.ForcedTarget = Enemies.FirstOrDefault(x => IsNearToKillable(x, true));
                                }
                                else if (Enemies.Any(x => IsKillable(x, x.IsUnderHisturret() && MenuValue.Combo.SmartE)))
                                {
                                    E.Cast();
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            if (Enemies.Any(x => GetECount(x) >= MenuValue.Combo.BuffCount))
                            {
                                E.Cast();
                            }
                        }
                        break;
                    case 3:
                        {
                            if (Enemies.Count(x => GetECount(x) >= MenuValue.Combo.BuffCount) >= MenuValue.Combo.HeroCount)
                            {
                                E.Cast();
                            }
                        }
                        break;
                    default:
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {

                AIHeroClient[] entities = EntityManager.Heroes.Enemies.Where(new Func<AIHeroClient, bool>(R.CanCast)).ToArray();
                Spell.Skillshot.BestPosition castPos = R.GetBestLinearCastPosition(entities);
                if (castPos.CastPosition != Vector3.Zero && castPos.HitNumber >= MenuValue.Combo.RHit)
                {
                    Player.CastSpell(SpellSlot.R);
                }
                var target = R.GetTarget();
                if (target != null && !IsKillable(target, true) && MenuValue.Combo.UseROut && !player.IsInAutoAttackRange(target))
                {
                    if (target.TotalShieldHealth() <= player.GetAutoAttackDamage(target, true) * Math.Pow(player.AttackDelay, -1) + (E.IsReady() ? EDamage(target, GetECount(target) + (int)Math.Pow(player.AttackDelay, -1)) : 0))
                    {
                        Player.CastSpell(SpellSlot.R);
                    }
                }
            }
        }
    }
}
