using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Libs.Base;
using UBAddons.Log;

namespace UBAddons.UBCore.BaseUlt
{
    class BaseUlt : IModuleBase
    {
        internal static Menu BaseMenu;
        public static bool Initialized { get; private set; }
        private static bool IsActiving;
        private static bool IsEnable;
        private static bool PreventCombo;
        internal static readonly Spell.Skillshot SpellBase;
        private static readonly Dictionary<int ,Teleport_Infomation> EnemyList = new Dictionary<int, Teleport_Infomation>();
        private static Dictionary<int, Teleport_Infomation> Teleporting = new Dictionary<int, Teleport_Infomation>();
        private static bool HasATarget
        {
            get
            {
                return Teleporting.Any(x => IsKillable(x.Value));
            }
        }

        static BaseUlt()
        {
            var me = Database.Ulti.FirstOrDefault(hero => hero.Champion == Player.Instance.Hero);
            if (me != null)
            {
                SpellBase = new Spell.Skillshot(me.Slot, me.Range, me.Type, me.CastDelay, me.Speed, me.Width)
                {
                    AllowedCollisionCount = me.AllowedCollisionCount,
                };
            }
            else
            {
                SpellBase = null;
            }

            if (MainMenu.MenuInstances.FirstOrDefault(x => x.Key.Contains("UBAddons")).Value == null)
            {
                Debug.Print("There's has a problem when trying to inject BaseUlt / Reacall Tracker. Please report to Uzumaki Boruto", Console_Message.Error);
            }
            else
            {
                try
                {
                    var Menu = MainMenu.MenuInstances.FirstOrDefault(x => x.Key.Contains("UBAddons")).Value.FirstOrDefault();
                    BaseMenu = Menu.AddSubMenu("BaseUlt", "UBAddons.BaseUlt", "Recall Tracker / Base Ult for UBAddons");
                    {
                        BaseMenu.AddGroupLabel("Recall / Teleport Tracker");
                        BaseMenu.Add("Tracker.Enabled", new CheckBox("Enabled tracker"));
                        BaseMenu.Add("Tracker.Enabled.Ally", new CheckBox("Enabled with ally"));

                        //BaseMenu.Add("Font.Color", new ColorPicker("Font Color", ColorReader.Load("Font.Color", Color.FromArgb(255, 0, 67))));
                        if (Database.Ulti.Any(x => Player.Instance.Hero.Equals(x.Champion)))
                        {
                            BaseMenu.AddGroupLabel("Base Ult");
                            var auto = BaseMenu.Add("Base.Enabled", new CheckBox("Auto Base Ult", false));
                            IsEnable = auto.CurrentValue;
                            auto.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                            {
                                IsEnable = args.NewValue;
                            };
                            var combo = BaseMenu.Add("Base.Combo", new CheckBox("Disable while combo", true));
                            PreventCombo = combo.CurrentValue;
                            combo.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                            {
                                PreventCombo = args.NewValue;
                            };
                            var Key = BaseMenu.Add("Base.Key.Enabled", new KeyBind("Base Ult Key", false, KeyBind.BindTypes.HoldActive, 'Z'));
                            Key.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                            {
                                if (args.NewValue && SpellBase.IsReady() && HasATarget)
                                {
                                    IsActiving = true;
                                    Core.DelayAction(() => IsActiving = false, 8000);
                                }
                            };
                            BaseMenu.AddLabel("It will has a notification when can kill an enemy, just press key when notification");
                            if (!EntityManager.Heroes.Enemies.Any())
                            {
                                BaseMenu.AddLabel("Couldn't load any champ, some thing wrong");
                            }
                            else
                            {
                                BaseMenu.Add("Base.Time", new Slider("Max {0} second(s) from last seen", 15, 1, 60));
                                foreach (var enemy in EntityManager.Heroes.Enemies)
                                {
                                    BaseMenu.Add("Base." + enemy.ChampionName, new CheckBox("Ult on " + enemy.ChampionName));
                                    EnemyList.Add(enemy.NetworkId, new Teleport_Infomation(enemy, false));
                                }
                            }
                        }
                        else
                        {
                            BaseMenu.AddLabel(Player.Instance.ChampionName + " is not supported yet");
                        }
                    }
                    TextureDraw.Initialize();
                    Teleport.OnTeleport += Teleport_OnTeleport;
                    Drawing.OnEndScene += Drawing_OnEndScene;
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString(), Console_Message.Error);
                }
            }
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender?.Type != GameObjectType.AIHeroClient)
                return;
            var hero = sender as AIHeroClient;
            if (hero == null || (!hero.IsEnemy && !BaseMenu.VChecked("Tracker.Enabled.Ally")))
                return;

            switch (args.Status)
            {
                case TeleportStatus.Start:
                    {
                        if (BaseMenu.VChecked("Tracker.Enabled"))
                        {
                            var text = args.Type.Equals(TeleportType.Recall) ? "recalling." : "teleporting.";
                            UBNotification.ShowNotif("Recall tracker notification", hero.Hero + " is " + text, "recall", 2500);
                        }
                        if (Teleporting.ContainsKey(sender.NetworkId))
                        {
                            Teleporting[sender.NetworkId] = new Teleport_Infomation(sender, args.Type.Equals(TeleportType.Recall)) { Duration = args.Duration, Started = args.Start, End = args.Duration + args.Start };
                        }
                        else
                        {
                            Teleporting.Add(sender.NetworkId, new Teleport_Infomation(sender, args.Type.Equals(TeleportType.Recall)) { Duration = args.Duration, Started = args.Start, End = args.Duration + args.Start });
                        }
                    }
                    break;
                case TeleportStatus.Abort:
                    {
                        if (BaseMenu.VChecked("Tracker.Enabled"))
                        {
                            var text = args.Type.Equals(TeleportType.Recall) ? "recall." : "teleport.";
                            UBNotification.ShowNotif("Recall tracker notification", hero.Hero + " canceled " + text, "recall", 2500);
                        }
                        Teleporting.Remove(hero.NetworkId);
                    }                 
                    break;
                case TeleportStatus.Finish:
                    {
                        if (BaseMenu.VChecked("Tracker.Enabled"))
                        {
                            var text = args.Type.Equals(TeleportType.Recall) ? "recall." : "teleport.";
                            UBNotification.ShowNotif("Recall tracker notification", hero.Hero + " finished " + text, "recall", 2500);
                        }
                        Teleporting.Remove(hero.NetworkId);
                    }
                    break;
                case TeleportStatus.Unknown:
                    {
                        Teleporting.Remove(hero.NetworkId);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(args.Status));
            }
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!BaseMenu.VChecked("Tracker.Enabled")) return;
            Teleporting = Teleporting.OrderByDescending(x => x.Value.End).ToDictionary(x => x.Key, x => x.Value);
            TextureDraw.Draw(Teleporting);
        }

        private static void Game_OnUpdate()
        {
            foreach (var info in EnemyList.Where(e => e.Value.Enemy.IsHPBarRendered && !e.Value.Enemy.IsDead))
            {
                info.Value.Lastseen = Core.GameTickCount;
            }

            foreach (var info in Teleporting.Where(d => CanKill(d.Value) && BaseMenu.VChecked("Base." + ((AIHeroClient)d.Value.Enemy)?.ChampionName) && d.Value.Duration > 0))
            {
                DoBaseUlt(info.Value);
            }
        }
        private static void DoBaseUlt(Teleport_Infomation info)
        {
            var CountDown = info.Started + info.Duration - Core.GameTickCount;
            var Traveltime = GetTravelTime(info.Enemy);
        
            if ((IsEnable && (!PreventCombo || !Orbwalker.ActiveModes.Combo.IsOrb())) || IsActiving)
            {
                if (SpellBase.IsReady() && CountDown >= Traveltime && CanKill(info))
                {
                    if (CountDown - Traveltime < 60)
                    {
                        SpellBase.Cast(info.Enemy.Home());
                    }
                }
            }
        }

        internal static bool IsKillable(Teleport_Infomation info)
        {
            var enemy = EnemyList.FirstOrDefault(e => e.Value.Enemy.NetworkId.Equals(info.Enemy.NetworkId));
            return GetDamage(enemy.Value?.Enemy) >= GetHealth(info);
        }
        internal static float GetHealth(Teleport_Infomation info)
        {
            if (info.Enemy.IsHPBarRendered)
            {
                return info.Enemy.TotalShieldHealth();
            }

            var invisibleTime = Core.GameTickCount - info.Lastseen;

            #warning HPRegenRate not true now
            //var healthregen = info.Enemy.HPRegenRate * (invisibleTime / 1000);
            var healthregen = 5f * (invisibleTime / 1000);

            var result = info.Enemy.TotalShieldHealth() + healthregen;

            return Math.Min(info.Enemy.TotalShieldMaxHealth(), result);
        }
        internal static float GetTravelTime(Obj_AI_Base target)
        {
            var hero = Player.Instance.Hero;
            var pos = target.Home();
            var distance = Player.Instance.Distance(pos);
            var speed = SpellBase.Speed;
            var delay = SpellBase.CastDelay;

            switch (hero)
            {
                case Champion.Lux:
                case Champion.Karthus:
                case Champion.Pantheon:
                case Champion.Gangplank:
                    return delay;
                case Champion.Jinx:
                    if (distance <= 1700)
                    {
                        return (distance / SpellBase.Speed + SpellBase.CastDelay) * 1000;
                    }
                    var addition = 1700 / SpellBase.Speed + SpellBase.CastDelay;
                    return ((distance - 1700) / 2230 + addition) * 1000;
                default:
                    return ((distance / speed) * 1000f) + delay;
            }
        }
        internal static float GetDamage(Obj_AI_Base target)
        {
            if (SpellBase == null)
            {
                return 0;
            }
            var dmg = Player.Instance.GetSpellDamage(target, SpellSlot.R, DamageLibrary.SpellStages.Default);
            var extradmg = 0f;
            if (Player.Instance.HasBuff("GangplankRUpgrade2"))
            {
                extradmg = dmg * 3f;
            }
            return !SpellBase.IsLearned ? 0 : dmg + extradmg;
        }
        public static bool CanKill(Teleport_Infomation info)
        {
                return SpellBase != null && /*SpellBase.IsInRange(info.Enemy.Home())*/ info.Enemy.IsEnemy
                    && info.IsRecall && IsKillable(info) && !HasCollison(info.Enemy) && CanCastFromLastSeen(info);
        }

        internal static bool HasCollison(Obj_AI_Base target)
        {
            var Rectangle = new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, target.Home(), SpellBase.Width);

            return EntityManager.Heroes.Enemies.Count(e => Rectangle.IsInside(e) && e.IsValidTarget()) > SpellBase.AllowedCollisionCount;
        }
        internal static bool CanCastFromLastSeen(Teleport_Infomation info)
        {
            var enemy = EnemyList.FirstOrDefault(e => e.Value.Enemy.NetworkId.Equals(info.Enemy.NetworkId));
            int timelimit = BaseMenu.VSliderValue("Base.Time");
            if (enemy.Value != null)
            {
                if (timelimit.Equals(0))
                {
                    return true;
                }
                return Core.GameTickCount - enemy.Value.Lastseen <= timelimit * 1000;
            }
            return Core.GameTickCount - info?.Lastseen <= timelimit * 1000;
        }

        public bool ShouldExecuted()
        {
            return true;
        }

        public void OnLoad()
        {
            Initialize();
        }

        public void Execute()
        {
            Game_OnUpdate();
        }

        private static void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            Initialized = true;
        }
    }
}
