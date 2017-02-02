using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System;
using System.Drawing;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Libs.ColorPicker;
using UBAddons.Log;
using Color = System.Drawing.Color;

namespace UBAddons.Champions.Twitch
{
    internal class Twitch : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Active Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Active E { get; set; }
        protected static Spell.Skillshot R { get; set; }


        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu LastHitMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        //protected static Vector3 vvv;
        static Twitch()
        {
            Q = new Spell.Active(SpellSlot.Q);

            W = new Spell.Skillshot(SpellSlot.W)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            E = new Spell.Active(SpellSlot.E, 1200)
            {
                CastDelay = 400,
            };

            R = new Spell.Skillshot(SpellSlot.R, DamageType.Mixed)
            {
                AllowedCollisionCount = int.MaxValue,
                Range = (uint)player.AttackRange + 300,
            };

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            Spellbook.OnCastSpell += delegate (Spellbook sender, SpellbookCastSpellEventArgs args)
            {
                if (!MenuValue.General.QStealth || !Q.IsReady() || !sender.Owner.IsMe || !args.Slot.Equals(SpellSlot.Recall)) return;
                args.Process = false;
                if (Q.Cast())
                {
                    Core.DelayAction(() => Player.CastSpell(SpellSlot.Recall), 300);
                }
            };
        }

        #region Create Menu
        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBAddons.MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.W, 60);
                Menu.AddGroupLabel("Misc");
                Menu.Add("UBAddons.Twitch.Recall.Q", new CheckBox("Enable Stealth Recall"));
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    var logic = ComboMenu.Add("UBAddons.Twitch.E.LogicBox", new ComboBox("E Logic", 1, "Only Kill Steal", "Killsteal Smart", "At stacks", "At stacks and enemy count"));
                    var slider = ComboMenu.Add("UBAddons.Twitch.E.Slider", new Slider("Use E only more than {0} enemy has passive buff", 3, 1, 5));
                    var slider2 = ComboMenu.Add("UBAddons.Twitch.E.Slider2", new Slider("Killsteal immediately if more than {0} enemy around me", 3, 1, 5));
                    var checkbox = ComboMenu.Add("UBAddons.Twitch.E.CheckBox", new CheckBox("E + Passive Logic if enemy in turret"));
                    var tips = ComboMenu.Add("UBAddons.Twitch.E.Tip", new Label(""));
                    switch (logic.CurrentValue)
                    {
                        case 0:
                            {
                                slider.IsVisible = false;
                                slider2.IsVisible = false;
                                checkbox.IsVisible = false;
                                tips.DisplayName = "E immediately if can kill enemy";
                            }
                            break;
                        case 1:
                            {
                                slider.IsVisible = true;
                                slider.DisplayName = "Killsteal immediately if more than {0} enemy/ally around me";
                                slider2.IsVisible = false;
                                checkbox.IsVisible = true;
                                tips.DisplayName = "Hold your E till and attack other target till you can double kill/ triple kill, but smart usage";
                            }
                            break;
                        case 2:
                            {
                                slider.IsVisible = false;
                                slider2.IsVisible = true;
                                slider2.MaxValue = 6;
                                slider2.DisplayName = "At Stacks";
                                checkbox.IsVisible = true;
                                tips.DisplayName = "Use E immediately if any Enemy has enough stacks";
                            }
                            break;
                        case 3:
                            {
                                slider.IsVisible = true;
                                slider2.IsVisible = true;
                                slider2.MaxValue = 6;
                                slider2.DisplayName = "At Stacks";
                                checkbox.IsVisible = true;
                                tips.DisplayName = "Use E immediately if enough Enemy has enough stacks";
                            }
                            break;
                        default:
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                    }
                    logic.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        switch (args.NewValue)
                        {
                            case 0:
                                {
                                    slider.IsVisible = false;
                                    slider2.IsVisible = false;
                                    checkbox.IsVisible = false;
                                    tips.DisplayName = "E immediately if can kill enemy";
                                }
                                break;
                            case 1:
                                {
                                    slider.IsVisible = true;
                                    slider2.IsVisible = false;
                                    slider.DisplayName = "Killsteal immediately if more than {0} enemy/ally around me";
                                    checkbox.IsVisible = true;
                                    tips.DisplayName = "Hold your E till and attack other target till you can double kill/ triple kill, but smart usage";
                                }
                                break;
                            case 2:
                                {
                                    slider.IsVisible = false;
                                    slider2.IsVisible = true;
                                    slider2.MaxValue = 6;
                                    slider2.DisplayName = "At Stacks";
                                    checkbox.IsVisible = false;
                                    tips.DisplayName = "Use E immediately if any Enemy has enough stacks";
                                }
                                break;
                            case 3:
                                {
                                    slider.IsVisible = true;
                                    slider.DisplayName = "Use E only more than {0} enemy has passive buff";
                                    slider2.IsVisible = true;
                                    slider2.MaxValue = 6;
                                    slider2.DisplayName = "At Stacks";
                                    checkbox.IsVisible = false;
                                    tips.DisplayName = "Use E immediately if enough Enemy has enough stacks";
                                }
                                break;
                            default:
                                {
                                    throw new ArgumentOutOfRangeException();
                                }
                        }
                    };
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.Add("UBAddons.Twitch.R.OutRange", new CheckBox("Use R if target out of R Range"));
                    ComboMenu.CreatSlotHitSlider(SpellSlot.R, 3, 1, 5);
                    ComboMenu.AddLabel("Only Use R out range if 90% can kill enemy");
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.W, 5, 1, 10);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.E, 5, 1, 10);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region LastHit
                LastHitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LastHitMenu.CreatLasthitOpening();
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LastHitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings", "UBAddons.Drawings" + player.Hero, "Settings your drawings below");
                {
                    DrawMenu.CreatDrawingOpening();
                    DrawMenu.Add("UBAddons.Twitch.Passive.Draw", new ComboBox("Draw Time Passive", 2, "Disable", "Circular", "Line", "Number"));
                    DrawMenu.Add("UBAddons.Twitch.Passive.ColorPicker", new ColorPicker("Dagger Color", ColorReader.Load("UBAddons.Twitch.Passive.ColorPicker", Color.GreenYellow)));
                    DrawMenu.CreatColorPicker(SpellSlot.W);
                    DrawMenu.CreatColorPicker(SpellSlot.E);
                    DrawMenu.CreatColorPicker(SpellSlot.R);
                    DrawMenu.CreatColorPicker(SpellSlot.Unknown);
                }
                #endregion

                DamageIndicator.Initalize(MenuValue.Drawings.ColorDmg);

            }
            catch (Exception exception)
            {
                Debug.Print(exception.ToString(), Console_Message.Error);
            }
        }
        #endregion

        #region Boolean
        protected override bool EnableDraw
        {
            get { return MenuValue.Drawings.EnableDraw; }
        }

        protected override bool EnableDamageIndicator
        {
            get { return MenuValue.Drawings.DrawDamageIndicator; }
        }

        protected override bool IsAutoHarass
        {
            get { return MenuValue.Harass.IsAuto; }
        }
        #endregion

        #region Misc
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsValidTarget() || !sender.IsEnemy) return;
            if (W.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : W.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.WGap)
            {
                W.Cast(sender);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            return;
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValidTarget()) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (player.ManaPercent < MenuValue.LastHit.ManaLimit) return;
            if (IsKillable(target, false) && MenuValue.LastHit.UseE && E.IsReady() && E.IsInRange(target))
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth > 0)
                {
                    E.Cast();
                }
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        protected static int GetECount(Obj_AI_Base target)
        {           
            //yep
            //return target.GetBuffCount("TwitchDeadlyVenom");
            if (target.IsDead || !target.IsEnemy || ((target.Type != GameObjectType.AIHeroClient) && (target.Type != GameObjectType.obj_AI_Minion)))
            {
                return 0;
            }

            var index = (from emitter in ObjectManager.Get<Obj_GeneralParticleEmitter>()
                         where emitter.Name.Contains("twitch_poison_counter_") &&
                             (emitter.Distance(target.ServerPosition) <=
                              (target.Type.Equals(GameObjectType.obj_AI_Minion) ? 65 : 176.7768f))
                         orderby emitter.Distance(target)
                         select emitter.Name).FirstOrDefault();

            if (index == null)
                return 0;

            int stacks;

            switch (index)
            {
                case "twitch_poison_counter_01.troy":
                    stacks = 1;
                    break;
                case "twitch_poison_counter_02.troy":
                    stacks = 2;
                    break;
                case "twitch_poison_counter_03.troy":
                    stacks = 3;
                    break;
                case "twitch_poison_counter_04.troy":
                    stacks = 4;
                    break;
                case "twitch_poison_counter_05.troy":
                    stacks = 5;
                    break;
                case "twitch_poison_counter_06.troy":
                    stacks = 6;
                    break;
                default:
                    stacks = 0;
                    break;
            }

            return stacks;
        }
        protected static bool IsKillable(Obj_AI_Base target, bool withpassive)
        {
            return !target.IsUdying() && E.IsInRange(target) && target.TotalShieldHealth() < EDamage(target) + (withpassive ? PassiveDamage(target) : 0);
        }
        protected static bool IsNearToKillable(Obj_AI_Base target, bool withpassive)
        {
            var count = GetECount(target);
            if (count >= 6)
            {
                return !IsKillable(target, withpassive) && !target.IsUdying() && E.IsInRange(target) && target.TotalShieldHealth() < EDamage(target) + (withpassive ? PassiveDamage(target) : 0);
            }
            return !IsKillable(target, withpassive) && !target.IsUdying() && E.IsInRange(target) && target.TotalShieldHealth() < EDamage(target, 6) + (withpassive ? PassiveDamage(target) : 0);
        }
        protected static float PassiveDamage(Obj_AI_Base target)
        {
            var buff = target.GetBuff("TwitchDeadlyVenom");
            if (buff == null)
            {
                return 0;
            }
            else
            {
                int dmgperstacks = (player.Level - 1) / 4 + 1;
                int remaining = (int)Math.Max(buff.EndTime - Game.Time, 0);
                return GetECount(target) * dmgperstacks * remaining;
            }
        }
        protected static float QDamage(Obj_AI_Base target)
        {
            return 0;
        }

        protected static float WDamage(Obj_AI_Base target)
        {
            return 0;
        }

        protected static float EDamage(Obj_AI_Base target, int emulator = -1)
        { 
            var buff = target.GetBuff("TwitchDeadlyVenom");
            if (buff == null)
            {
                return 0;
            }
            else
            {
                var basic = new[] { 0f, 20f, 35f, 50f, 65f, 80f }[E.Level];
                var perstack = new[] { 0f, 15f, 20f, 25f, 30f, 35f }[E.Level] + 0.25f * player.FlatPhysicalDamageMod + 0.2f * player.TotalMagicalDamage;
                if (emulator != -1)
                {
                    return player.CalculateDamageOnUnit(target, DamageType.Physical, basic + perstack * Math.Min(Math.Abs(emulator), 6));
                }
                return player.CalculateDamageOnUnit(target, DamageType.Physical, basic + perstack * GetECount(target));
            }
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            return 0;
        }
        #endregion

        internal static float HandleDamageIndicator(Obj_AI_Base target, SpellSlot? slot = null)
        {
            if (target == null)
            {
                return 0;
            }
            switch (slot)
            {
                case SpellSlot.Q:
                    return QDamage(target);
                case SpellSlot.W:
                    return WDamage(target);
                case SpellSlot.E:
                    return EDamage(target);
                case SpellSlot.R:
                    return RDamage(target);
                default:
                    {
                        float damage = PassiveDamage(target);

                        if (E.IsReady())
                        {
                            damage += EDamage(target);
                        }
                        if (Orbwalker.CanAutoAttack)
                        {
                            damage += player.GetAutoAttackDamage(target, true);
                        }
                        return damage;
                    }
            }
        }
        #endregion

        #region Modes
        protected override void PermaActive()
        {
            Modes.PermaActive.Execute();
        }

        protected override void Combo()
        {
            Modes.Combo.Execute();
        }

        protected override void Harass()
        {
            if (player.IsUnderEnemyturret()) return;
            Modes.Harass.Execute();
        }

        protected override void LaneClear()
        {
            Modes.LaneClear.Execute();
        }

        protected override void JungleClear()
        {
            Modes.JungleClear.Execute();
        }

        protected override void LastHit()
        {
            Modes.LastHit.Execute();
        }

        protected override void Flee()
        {
            Modes.Flee.Execute();
        }
        #endregion

        #region Drawings
        protected override void OnDraw(EventArgs args)
        {
            if (!MenuValue.Drawings.EnableDraw) return;
            if (MenuValue.Drawings.DrawW && (!MenuValue.Drawings.ReadyW || W.IsReady()))
            {
                W.DrawRange(MenuValue.Drawings.ColorW);
            }
            if (MenuValue.Drawings.DrawE && (!MenuValue.Drawings.ReadyE || E.IsReady()))
            {
                E.DrawRange(MenuValue.Drawings.ColorE);
            }
            if (MenuValue.Drawings.DrawR && (!MenuValue.Drawings.ReadyR || R.IsReady()))
            {
                R.DrawRange(MenuValue.Drawings.ColorR);
            }
            if (MenuValue.Drawings.DrawPassive != 0)
            {
                foreach (var hero in EntityManager.Heroes.Enemies.Where(x => x.Position.IsOnScreen() && x.HasBuff("TwitchDeadlyVenom") && x.IsValidTarget()))
                {
                    var buff = hero.GetBuff("TwitchDeadlyVenom");
                    var time = Math.Max(buff.EndTime - Game.Time, 0);
                    var PercentRemaining = -time / 6f * 100;
                    switch (MenuValue.Drawings.DrawPassive)
                    {
                        case 1:
                            {
                                UBDrawings.DrawCircularTimer(hero.Position, 150, MenuValue.Drawings.ColorPassive, 0, PercentRemaining);
                            }
                            break;
                        case 2:
                            {
                                UBDrawings.DrawLinearTimer(hero.Position.WorldToScreen(), PercentRemaining, MenuValue.Drawings.ColorPassive);
                            }
                            break;
                        case 3:
                            {
                                UBDrawings.DrawTimer(hero.Position.WorldToScreen(), time, MenuValue.Drawings.ColorPassive);
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        #region Menu Value
        protected internal static class MenuValue
        {
            internal static class General
            {
                public static int WHitChance { get { return Menu.GetSlotHitChance(SpellSlot.W); } }

                public static bool QStealth { get { return Menu.VChecked("UBAddons.Twitch.Recall.Q"); } }
            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ELogic { get { return ComboMenu.GetSlotComboBox(SpellSlot.E); } }

                public static int HeroCount { get { return ComboMenu.VSliderValue("UBAddons.Twitch.E.Slider"); } }

                public static int BuffCount { get { return ComboMenu.VSliderValue("UBAddons.Twitch.E.Slider2"); } }

                public static bool SmartE { get { return ComboMenu.VChecked("UBAddons.Twitch.E.CheckBox"); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static bool UseROut { get { return ComboMenu.VChecked("UBAddons.Twitch.R.OutRange"); } }

                public static int RHit { get { return ComboMenu.GetSlotHitSlider(SpellSlot.R); } }
            }

            internal static class Harass
            {
                public static bool UseE { get { return HarassMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return HarassMenu.GetManaLimit(); } }

                public static bool IsAuto { get { return HarassMenu.GetHarassKeyBind(); } }
            }

            internal static class LaneClear
            {
                public static bool EnableIfNoEnemies { get { return LaneClearMenu.GetNoEnemyOnly(); } }

                public static int ScanRange { get { return LaneClearMenu.GetDetectRange(); } }

                public static bool OnlyKillable { get { return LaneClearMenu.GetKillableOnly(); } }

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WHit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int EHit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.E); } }

                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode { get { return LastHitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LastHitMenu.PreventCombo(); } }

                public static bool UseW { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return LastHitMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

            }

            internal static class Drawings
            {

                public static bool EnableDraw { get { return DrawMenu.VChecked(Variables.AddonName + "." + player.Hero + ".EnableDraw"); } }

                public static int DrawPassive { get { return DrawMenu.VComboValue("UBAddons.Twitch.Passive.Draw"); } }

                public static Color ColorPassive { get { return DrawMenu["UBAddons.Twitch.Passive.ColorPicker"].Cast<ColorPicker>().CurrentValue; } }

                public static bool DrawW { get { return DrawMenu.GetDrawCheckValue(SpellSlot.W); } }

                public static bool ReadyW { get { return DrawMenu.GetOnlyReady(SpellSlot.W); } }

                public static SharpDX.Color ColorW { get { return DrawMenu.GetColorPicker(SpellSlot.W).ToSharpDX(); } }

                public static bool DrawE { get { return DrawMenu.GetDrawCheckValue(SpellSlot.E); } }

                public static bool ReadyE { get { return DrawMenu.GetOnlyReady(SpellSlot.E); } }

                public static SharpDX.Color ColorE { get { return DrawMenu.GetColorPicker(SpellSlot.E).ToSharpDX(); } }

                public static bool DrawR { get { return DrawMenu.GetDrawCheckValue(SpellSlot.R); } }

                public static bool ReadyR { get { return DrawMenu.GetOnlyReady(SpellSlot.R); } }

                public static SharpDX.Color ColorR { get { return DrawMenu.GetColorPicker(SpellSlot.R).ToSharpDX(); } }

                public static bool DrawDamageIndicator { get { return DrawMenu.GetDrawCheckValue(SpellSlot.Unknown); } }

                public static Color ColorDmg { get { return DrawMenu.GetColorPicker(SpellSlot.Unknown); } }

            }
        }
        #endregion
    }
}
