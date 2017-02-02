using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Drawing;
using System.Linq;
using UBMiddle.General;
using UBMiddle.Libs;
using UBMiddle.Log;

namespace UBMiddle.Champions.Camille
{
    internal class Camille : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Active Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Skillshot EOnWall { get; set; }
        protected static Spell.SimpleSkillshot EToWall { get; set; }
        protected static Spell.Targeted R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LasthitMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Camille()
        {
            Q = new Spell.Active(SpellSlot.Q, 325, DamageType.Physical);
            W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Cone, 500, int.MaxValue, 20, DamageType.Physical)
            {
                AllowedCollisionCount = int.MaxValue,
                ConeAngleDegrees = 45,
            };
            E = new Spell.Skillshot(SpellSlot.E, 1400, SkillShotType.Linear, 250, int.MaxValue, (int)player.BoundingRadius + 1, DamageType.Physical)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            EToWall = new Spell.SimpleSkillshot(SpellSlot.E, 1400, DamageType.Physical);
            R = new Spell.Targeted(SpellSlot.R, 475, DamageType.Mixed);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;

            Orbwalker.OnPostAttack += delegate(AttackableUnit target, EventArgs args)
            {
                if (target == null || !Q.IsReady()) return;
                switch (Orbwalker.ActiveModesFlags)
                {
                    case Orbwalker.ActiveModes.Combo:
                        {
                            if (MenuValue.Combo.UseQ && MenuValue.Combo.QAfter)
                            {
                                Q.Cast();
                                Orbwalker.ResetAutoAttack();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.Harass:
                        {
                            if (MenuValue.Harass.UseQ && MenuValue.Harass.QAfter)
                            {
                                Q.Cast();
                                Orbwalker.ResetAutoAttack();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.JungleClear:
                        {
                            if (MenuValue.JungleClear.UseQ && Q.GetJungleMobs().Any())
                            {
                                Q.Cast();
                                Orbwalker.ResetAutoAttack();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.LaneClear:
                        {
                            if (MenuValue.LaneClear.UseQ && Q.GetLaneMinions().Any())
                            {
                                Q.Cast();
                                Orbwalker.ResetAutoAttack();
                            }
                        }
                        break;
                    default:
                        break;
                }
            };
            Orbwalker.OnPreAttack += delegate(AttackableUnit target, Orbwalker.PreAttackArgs args)
            {
                var Target = target as AIHeroClient;
                if (Target == null) return;
                switch (Orbwalker.ActiveModesFlags)
                {
                    case Orbwalker.ActiveModes.Combo:
                        {
                            if (MenuValue.Combo.UseW && !MenuValue.Combo.QAfter)
                            {
                                Q.Cast();
                            }
                        }
                        break;
                    case Orbwalker.ActiveModes.Harass:
                        {
                            if (MenuValue.Harass.UseW && !MenuValue.Harass.QAfter)
                            {
                                Q.Cast();
                            }
                        }
                        break;
                    default:
                        break;
                }
            };
            AIHeroClient.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (!sender.IsMe || args.Slot != SpellSlot.Q) return;
                Orbwalker.ResetAutoAttack();
            };
        }

        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBMiddle.MainMenu" + player.Hero, "UB" + player.Hero + " - UBMiddle - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.E);
                Menu.CreatSlotHitChance(SpellSlot.R);
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBMiddle.ComboMenu" + player.Hero, "UB" + player.Hero + " - Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotComboBox(SpellSlot.W, 0, "After AA", "Before AA");
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.Add("UBMiddle.Kassadin.R.HP.Enemy", new Slider("Enemy HP {0}% for R", 60));
                    ComboMenu.Add("UBMiddle.Kassadin.R.HP.My", new Slider("My HP {0}% for R", 30));
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBMiddle.HarassMenu" + player.Hero, "UB" + player.Hero + " - Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotComboBox(SpellSlot.W, 0, "After AA", "Before AA");
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.CreatManaLimit(true);
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBMiddle.LaneClear" + player.Hero, "UB" + player.Hero + " - Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.E, 5, 1, 10);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.R);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.R, 5, 1, 10);
                    LaneClearMenu.Add("UBMiddle.Kassadin.R.Stack", new Slider("R stack limit", 1, 1, 4));
                    LaneClearMenu.CreatManaLimit(true);
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBMiddle.JungleClear" + player.Hero, "UB" + player.Hero + " - Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.R, null, false);
                    JungleClearMenu.Add("UBMiddle.Kassadin.R.Stack", new Slider("R stack limit", 1, 1, 4));
                    JungleClearMenu.CreatManaLimit(true);
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", "UBMiddle.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.W);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.E);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.R, null, false);
                    LasthitMenu.Add("UBMiddle.Kassadin.R.Stack", new Slider("R stack limit", 1, 1, 4));
                    LasthitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBMiddle.Misc" + player.Hero, "UB" + player.Hero + " - Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "KillSteal");
                    MiscMenu.AddGroupLabel("Deny Recall settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.DenyRecall.ToString());
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, Misc_Menu_Value.DenyRecall.ToString());
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, Misc_Menu_Value.DenyRecall.ToString());

                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings");
                {
                    DrawMenu.CreatDrawingOpening();
                    DrawMenu.CreatColorPicker(SpellSlot.Q);
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

        #region Misc
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid) return;
            if (E.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : E.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.EGap)
            {
                E.Cast(sender);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.DangerValue.Contains(args.DangerLevel)) return;
            if (Q.IsReady() && MenuValue.Misc.QI)
            {
                Q.Cast(sender);
            }
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValidTarget()) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.Q) && MenuValue.LastHit.UseQ && Q.IsReady())
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth > float.Epsilon)
                {
                    Q.Cast(target);
                }
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.W) && MenuValue.LastHit.UseW && W.IsReady())
            {
                var predHealth = W.GetHealthPrediction(target);
                if (predHealth > float.Epsilon && player.IsInAutoAttackRange(target))
                {
                    W.Cast();
                }
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.E) && MenuValue.LastHit.UseE && E.IsReady())
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth > float.Epsilon)
                {
                    E.Cast(target);
                }
            }
        }
        protected override void OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || args.Type != TeleportType.Recall) return;
            if (MenuValue.Misc.QRecall && Q.IsReady() && Q.IsInRange(sender))
            {
                Q.Cast(sender);
            }
            if (MenuValue.Misc.ERecall && E.IsReady() && E.IsInRange(sender))
            {
                E.Cast(sender);
            }
            if (R.IsReady() && (E.IsReady() || Q.IsReady()))
            {
                if (sender.IsInRange(player, Q.Range + E.Range))
                {
                    var pos = player.Position.Extend(sender, R.Range).To3DWorld();
                    if (pos.CountEnemiesInRange(600) <= 2 && RStack <= 1)
                    {
                        R.Cast(pos);
                    }
                }
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        protected static float QDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.Default);
        }
        protected static float WDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 40f, 65f, 90f, 115f, 140f }[W.Level] + 0.7f * player.TotalMagicalDamage);
        }
        protected static float EDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.E, DamageLibrary.SpellStages.Default);
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            var raw = player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 80f, 100f, 120f }[R.Level] + 0.2f * player.TotalMagicalDamage + 0.02f * player.MaxMana);
            var dmgperstack = player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 40f, 50f, 60f }[R.Level] + 0.1f * player.TotalMagicalDamage + 0.01f * player.MaxMana);
            return raw + dmgperstack + RStack;
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
                        float damage = 0f;

                        if (Q.IsReady())
                        {
                            damage = damage + QDamage(target);
                        }
                        if (W.IsReady())
                        {
                            damage = damage + WDamage(target);
                        }
                        if (E.IsReady())
                        {
                            damage = damage + EDamage(target);
                        }
                        if (R.IsReady())
                        {
                            damage = damage + RDamage(target);
                        }
                        if (Orbwalker.CanAutoAttack)
                        {
                            damage = damage + player.GetAutoAttackDamage(target, true);
                        }
                        return damage;
                    }
            }
        }

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
            if (MenuValue.Drawings.DrawQ && (!MenuValue.Drawings.ReadyQ || Q.IsReady()))
            {
                Q.DrawRange(MenuValue.Drawings.ColorQ);
            }
            if (MenuValue.Drawings.DrawE && (!MenuValue.Drawings.ReadyE || E.IsReady()))
            {
                E.DrawRange(MenuValue.Drawings.ColorE);
            }
            if (MenuValue.Drawings.DrawR && (!MenuValue.Drawings.ReadyR || R.IsReady()))
            {
                R.DrawRange(MenuValue.Drawings.ColorR);
            }
        }
        #endregion

        #region Menu Value
        protected internal static class MenuValue
        {
            internal static class General
            {
                public static int EHitChance { get { return Menu.GetSlotHitChance(SpellSlot.E); } }
                public static int RHitChance { get { return Menu.GetSlotHitChance(SpellSlot.R); } }

            }
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool WAfter { get { return ComboMenu.GetSlotComboBox(SpellSlot.W) < float.Epsilon; } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int MyHP { get { return ComboMenu.VSliderValue("UBMiddle.Kassadin.R.HP.My"); } }

                public static int EnemyHP { get { return ComboMenu.VSliderValue("UBMiddle.Kassadin.R.HP.Enemy"); } }

            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool WAfter { get { return HarassMenu.GetSlotComboBox(SpellSlot.W) < float.Epsilon; } }

                public static int ManaLimit { get { return HarassMenu.GetManaLimit(); } }

                public static bool IsAuto { get { return HarassMenu.GetHarassKeyBind(); } }
            }

            internal static class LaneClear
            {
                public static bool EnableIfNoEnemies { get { return LaneClearMenu.GetNoEnemyOnly(); } }

                public static int ScanRange { get { return LaneClearMenu.GetDetectRange(); } }

                public static bool OnlyKillable { get { return LaneClearMenu.GetKillableOnly(); } }

                public static bool UseQ { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int Ehit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.E); } }

                public static bool UseR { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int Rhit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.R); } }

                public static int RStack { get { return LaneClearMenu.VSliderValue("UBMiddle.Kassadin.R.Stack"); } }


                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int RStack { get { return JungleClearMenu.VSliderValue("UBMiddle.Kassadin.R.Stack"); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode { get { return LasthitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LasthitMenu.PreventCombo(); } }

                public static bool UseQ { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int ManaLimit { get { return LasthitMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool WKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] DangerValue { get { return MiscMenu.GetDangerValue(); } }

                public static bool QI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.Interrupter.ToString()); } }

                public static bool QRecall { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.DenyRecall.ToString()); } }

                public static bool ERecall { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.DenyRecall.ToString()); } }

                public static bool RRecall { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.DenyRecall.ToString()); } }


            }

            internal static class Drawings
            {

                public static bool EnableDraw { get { return DrawMenu.VChecked(Variables.AddonName + "." + player.Hero + ".EnableDraw"); } }

                public static bool DrawQ { get { return DrawMenu.GetDrawCheckValue(SpellSlot.Q); } }

                public static bool ReadyQ { get { return DrawMenu.GetOnlyReady(SpellSlot.Q); } }

                public static SharpDX.Color ColorQ { get { return DrawMenu.GetColorPicker(SpellSlot.Q).ToSharpDX(); } }

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
