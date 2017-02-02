using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using SharpDX;
using UBAddons.Libs.Base;
using EloBuddy.SDK.Enumerations;

namespace UBAddons.Libs
{
    /// <summary>
    /// Credit for Icreative
    /// </summary>
    class ImmobileTracker : IModuleBase
    {
        private const int Width = 300;
        private const string BlitzcrankBuffName = "RocketGrab";
        private const string ThreshBuffName = "ThreshQ";
        private const string ThreshBuffName2 = "threshqfakeknockup";
        private const string AatroxDeath = "AatroxPassiveDeath";
        private const string AniviaRebirth = "rebirth";
        private const string ZileanBuffRevive = "chronorevive";
        private const string FioraW = "fioraw";
        private const string AngleRevive = "willrevive";
        private const string ZhonyasBuffName = "zhonyasringshield";
        private const string BardRBuffName = "bardrstasis";
        private const int ThreshKnockupDistance = 430;
        private const string TeleportName = "global_ss_teleport_target_red.troy";
        private static GameObject _lastTeleportObject;
        private static float _lastTeleportTime;

        private static readonly Dictionary<AIHeroClient, BuffInstance> BlitzcrankSenders =
            new Dictionary<AIHeroClient, BuffInstance>();

        private static readonly Dictionary<AIHeroClient, Tuple<BuffInstance, Vector3>> ThreshSenders =
            new Dictionary<AIHeroClient, Tuple<BuffInstance, Vector3>>();

        private static bool _containsThresh;
        private static bool _containsBard;
        private static Menu Menu
        {
            get
            {
                if (MainMenu.MenuInstances.Any(x => x.Value.Any(y => y.UniqueMenuId.Equals("UBAddon.MainMenuViktor"))))
                {
                    return MainMenu.MenuInstances.Values.FirstOrDefault(x => x.Any(y => y.UniqueMenuId.Equals("UBAddon.MainMenuViktor"))).FirstOrDefault();
                }
                return null;
            }
        }
        private static Spell.Skillshot W;
        private static bool UseOnCC => Menu.VChecked("UBAddons.Viktor.W.CC");
        private static bool UseOnTelePort => Menu.VChecked("UBAddons.Viktor.W.Teleport");
        private static bool UseOnRevive => Menu.VChecked("UBAddons.Viktor.W.Revive");

        public bool ShouldExecuted()
        {
            return Player.Instance.Hero.Equals(Champion.Viktor);
        }

        public void OnLoad()
        {
            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular, 250, int.MaxValue, 300)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            GameObject.OnCreate += GameObject_OnCreate;
            //GameObject.OnDelete += GameObject_OnDelete;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            _containsThresh = EntityManager.Heroes.Allies.Any(h => h.Hero == Champion.Thresh);
            _containsBard = EntityManager.Heroes.AllHeroes.Any(h => h.Hero == Champion.Bard);
            if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.TwistedFate)))
            {
                AddRTracker(Champion.TwistedFate, "GateMarker_red.troy", 1300); //1.5 seconds
            }
            if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.Pantheon)))
            {
                AddRTracker(Champion.Pantheon, "Pantheon_Base_R_indicator_red.troy", 2800); //3 seconds
            }
            if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.Ryze)))
            {
                AddRTracker(Champion.Ryze, "Ryze_Base_R_End_Enemy.troy", 1800); //2 seconds          
                AddRTracker(Champion.Ryze, "Ryze_Base_R_Start_Enemy.troy", 0); //2 seconds
            }
            if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.TahmKench)))
            {
                AddRTracker(Champion.TahmKench, "TahmKench_Base_R_Target_Enemy.troy", 3800); //4 seconds
            }
        }

        public void Execute()
        {
            if (W.IsReady())
            {
                foreach (var b in BlitzcrankSenders.Where(b => !b.Key.IsValidTarget() || !b.Value.IsValid))
                {
                    BlitzcrankSenders.Remove(b.Key);
                }
                foreach (var b in ThreshSenders.Where(b => !b.Key.IsValidTarget() || !b.Value.Item1.IsValid))
                {
                    ThreshSenders.Remove(b.Key);
                }
                if (UseOnCC || Orbwalker.ActiveModes.Combo.IsOrb())
                {
                    foreach (
                        var enemy in
                            EntityManager.Heroes.Enemies.Where(x => x.IsValid && WillBeImmobile(x, GetWTime()) && !BlitzcrankSenders.ContainsKey(x) && !ThreshSenders.ContainsKey(x)))
                    {
                        if (_containsThresh)
                        {
                            if (!enemy.HasBuff(ThreshBuffName2))
                            {
                                W.Cast(enemy.Position);
                            }
                        }
                        else
                        {
                            W.Cast(enemy.Position);
                        }
                    }
                    foreach (var castPosition in from dic in BlitzcrankSenders let blitz = EntityManager.Heroes.Allies.FirstOrDefault(h => h.IsValidTarget() && h.ChampionName.Equals(dic.Value.SourceName)) where blitz != null select blitz.Position.Extend(dic.Key.Position, 50).To3DWorld())
                    {
                        W.Cast(castPosition);
                    }
                    foreach (var castPosition in from tuple in ThreshSenders let thresh = EntityManager.Heroes.Allies.FirstOrDefault(h => h.IsValidTarget() && h.ChampionName.Equals(tuple.Value.Item1.SourceName)) where thresh != null let startPosition = tuple.Value.Item2 select thresh.Position + (startPosition - thresh.Position).Normalized() * Math.Max(thresh.Distance(startPosition) - ThreshKnockupDistance, 1f))
                    {
                        W.Cast(castPosition);
                    }
                }
                if (UseOnTelePort)
                {
                    if (_lastTeleportObject != null && _lastTeleportTime > 0 &&
                        Core.GameTickCount - _lastTeleportTime >= 2700 && Core.GameTickCount - _lastTeleportTime <= 4000)
                    //4 seconds
                    {
                        W.Cast(_lastTeleportObject.Position);
                    }
                }
                if (UseOnCC)
                {
                    foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && !h.IsDead && h.HasBuff(ZhonyasBuffName)))
                    {
                        var buff = enemy.GetBuff(ZhonyasBuffName);
                        if (buff.EndTime - Game.Time <= 0.2f && buff.EndTime - buff.StartTime > 0f)
                        {
                            W.Cast(enemy.Position);
                        }
                    }
                    if (_containsBard)
                    {
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && !h.IsDead && h.HasBuff(BardRBuffName)))
                        {
                            var buff = enemy.GetBuff(BardRBuffName);
                            if (buff.EndTime - Game.Time <= 0.2f && buff.EndTime - buff.StartTime > 0f)
                            {
                                W.Cast(enemy.Position);
                            }
                        }
                    }
                    if (UseOnRevive)
                    {
                        if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.Aatrox)))
                        {
                            foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && h.HasBuff(AatroxDeath)))
                            {
                                var buff = enemy.GetBuff(AatroxDeath);
                                if (buff.EndTime - Game.Time <= 0.2f && buff.EndTime - buff.StartTime > 0f)
                                {
                                    W.Cast(enemy.Position);
                                }
                            }
                        }
                        if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.Anivia)))
                        {
                            foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && h.HasBuff(AniviaRebirth)))
                            {
                                var buff = enemy.GetBuff(AniviaRebirth);
                                if (buff.EndTime - Game.Time <= 0.2f && buff.EndTime - buff.StartTime > 0f)
                                {
                                    W.Cast(enemy.Position);
                                }
                            }
                        }
                        if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.Fiora)))
                        {
                            foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && h.HasBuff(FioraW)))
                            {
                                var buff = enemy.GetBuff(FioraW);
                                if (buff.EndTime - Game.Time <= 0.2f && buff.EndTime - buff.StartTime > 0f)
                                {
                                    W.Cast(enemy.Position);
                                }
                            }
                        }
                        if (EntityManager.Heroes.Enemies.Any(x => x.Hero.Equals(Champion.Zilean)))
                        {
                            foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && h.HasBuff(ZileanBuffRevive)))
                            {
                                var buff = enemy.GetBuff(ZileanBuffRevive);
                                if (buff.EndTime - Game.Time <= 0.2f && buff.EndTime - buff.StartTime > 0f)
                                {
                                    W.Cast(enemy.Position);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddRTracker(Champion hero, string rObjectName, int timeToWait)
        {
            if (EntityManager.Heroes.Enemies.Any(h => h.Hero == hero))
            {
                GameObject rObject = null;
                var rObjectTime = 0;
                //var allyCastedR = false;
                Game.OnTick += delegate
                {
                    if (W.IsReady())
                    {
                        if (rObject != null)
                        {
                            if (rObjectTime > 0 && Core.GameTickCount - rObjectTime >= timeToWait &&
                                Core.GameTickCount - rObjectTime <= 4000)
                            {
                                if (UseOnCC || Orbwalker.ActiveModes.Combo.IsOrb())
                                {
                                    W.Cast(rObject.Position);
                                }
                            }
                        }
                    }
                };
                //Obj_AI_Base.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                //{
                //    var senderHero = sender as AIHeroClient;
                //    if (senderHero != null && senderHero.IsAlly && senderHero.Hero == hero)
                //    {
                //        if (args.Slot == SpellSlot.R)
                //        {
                //            allyCastedR = true;
                //            Core.DelayAction(delegate { allyCastedR = false; }, 10000);
                //        }
                //    }
                //};
                GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
                {
                    if (sender.Name.Equals(rObjectName)/* && !allyCastedR*/)
                    {
                        rObject = sender;
                        rObjectTime = Core.GameTickCount;
                        Core.DelayAction(delegate
                        {
                            rObject = null;
                            rObjectTime = 0;
                        }, 5200);
                    }
                };
                GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
                {
                    if (rObject != null && rObject.IdEquals(sender))
                    {
                        rObject = null;
                        rObjectTime = 0;
                    }
                };
            }
        }

        public static float GetWTime()
        {
            return 1.5f;
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var senderHero = sender as AIHeroClient;
            var casterHero = args.Buff.Caster as AIHeroClient;
            if (senderHero != null && senderHero.IsEnemy)
            {
                if (casterHero != null && casterHero.IsAlly && EntityManager.Heroes.Allies.Any(h => h.ChampionName.Equals(args.Buff.SourceName)))
                {
                    if (args.Buff.SourceName.Equals("Blitzcrank") && args.Buff.DisplayName.Equals(BlitzcrankBuffName))
                    {
                        if (BlitzcrankSenders.ContainsKey(senderHero))
                        {
                            BlitzcrankSenders.Remove(senderHero);
                        }
                        BlitzcrankSenders.Add(senderHero, args.Buff);
                        Core.DelayAction(delegate
                        {
                            if (BlitzcrankSenders.ContainsKey(senderHero))
                            {
                                BlitzcrankSenders.Remove(senderHero);
                            }
                        }, (int)(1000 * (args.Buff.EndTime - args.Buff.StartTime + 0.1f)));
                    }
                    else if (args.Buff.SourceName.Equals("Thresh") && args.Buff.DisplayName.Equals(ThreshBuffName))
                    {
                        if (ThreshSenders.ContainsKey(senderHero))
                        {
                            ThreshSenders.Remove(senderHero);
                        }
                        ThreshSenders.Add(senderHero, new Tuple<BuffInstance, Vector3>(args.Buff, senderHero.ServerPosition));
                        Core.DelayAction(delegate
                        {
                            if (ThreshSenders.ContainsKey(senderHero))
                            {
                                ThreshSenders.Remove(senderHero);
                            }
                        }, (int)(1000 * (args.Buff.EndTime - args.Buff.StartTime + 0.1f)));
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            var senderHero = sender as AIHeroClient;
            var casterHero = args.Buff.Caster as AIHeroClient;
            if (senderHero != null && senderHero.IsEnemy && casterHero != null && casterHero.IsAlly)
            {
                if (args.Buff.SourceName.Equals("Blitzcrank") && args.Buff.DisplayName.Equals(BlitzcrankBuffName))
                {
                    if (BlitzcrankSenders.ContainsKey(senderHero))
                    {
                        BlitzcrankSenders.Remove(senderHero);
                    }
                }
                else if (args.Buff.SourceName.Equals("Thresh") && args.Buff.DisplayName.Equals(ThreshBuffName))
                {
                    if (ThreshSenders.ContainsKey(senderHero))
                    {
                        ThreshSenders.Remove(senderHero);
                    }
                }
                else if (args.Buff.Name.Equals(AngleRevive) && UseOnRevive)
                {
                    W.Cast(senderHero);
                }
            }
        }
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals(TeleportName))
            {
                _lastTeleportObject = sender;
                _lastTeleportTime = Core.GameTickCount;
                Core.DelayAction(delegate
                {
                    _lastTeleportObject = null;
                    _lastTeleportTime = 0;
                }, 5200);
            }
        }

        //private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        //{
        //    if (sender.IdEquals(_lastTeleportObject))
        //    {
        //        //_lastTeleportObject = null;
        //        //_lastTeleportTime = 0;
        //    }
        //}
        public static bool WillBeImmobile(Obj_AI_Base target, float time)
        {
            var buffDuration = target.GetMovementBlockedDebuffDuration();
            return buffDuration > 0 && time <= buffDuration + (Width + target.BoundingRadius) / target.MoveSpeed;
        }
    }
}
