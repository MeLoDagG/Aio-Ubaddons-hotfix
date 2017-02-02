using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Drawing;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Log;

namespace UBAddons.Champions.Zilean
{
    internal class Zilean : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Active W { get; set; }
        protected static Spell.Targeted E { get; set; }
        protected static Spell.Targeted R { get; set; }


        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu AutoMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Zilean()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Circular, 350, 2000, 100, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            W = new Spell.Active(SpellSlot.W);

            E = new Spell.Targeted(SpellSlot.E, 750);

            R = new Spell.Targeted(SpellSlot.R, 900);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
        }

        #region Creat Menu
        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBAddons.MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.Q);                

                #endregion

                #region R
                AutoMenu = Menu.AddSubMenu("R", "UBAddons.RMenu" + player.Hero, "Settings your R saver below");
                {
                    string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".Saver.";
                    AutoMenu.Add(BeginText + "Enable", new CheckBox("Use R"));
                    AutoMenu.AddSeparator();
                    foreach (var champ in EntityManager.Heroes.Allies)
                    {
                        AutoMenu.Add(BeginText + "Enable." + champ.Hero, new CheckBox("Use on " + champ.ChampionName));
                        AutoMenu.Add(BeginText + "Prediction." + champ.Hero, new CheckBox("Use Prediction"));
                        AutoMenu.Add(BeginText + "HP." + champ.Hero, new Slider("Use R if " + champ.ChampionName + "'s HP below {0}%", 20, 0, 100));
                        AutoMenu.Add(BeginText + "Priority." + champ.Hero, new Slider(champ.ChampionName + "'s priority", CrazyTargetSelector.GetPriority(champ), 1, 5));
                        AutoMenu.AddSeparator();
                    }
                }
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotComboBox(SpellSlot.E, 0, "Smart", "Slow", "Speed");
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "Interrupter");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "KillSteal");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings", "UBAddons.Drawings" + player.Hero, "Settings your drawings below");
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
            if (sender == null || !sender.IsValid) return;
            switch (sender.IsEnemy)
            {
                case true:
                    {
                        var Ally = EntityManager.Heroes.Allies.Where(x => x.IsValid && !x.IsDead && ! x.IsZombie).OrderBy(x => x.Distance(args.End)).First();
                        if ((MenuValue.Misc.Idiot ? Ally.Distance(args.End) <= 250 : Q.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.QGap)
                        {
                            if (Q.IsReady())
                            {
                                Q.Cast(args.End);
                            }
                            else if (E.IsReady())
                            {
                                E.Cast(Ally);
                            }
                            else if (W.IsReady() && MenuValue.Misc.WGap)
                            {
                                W.Cast();
                            }
                        }
                    }
                    break;
                case false:
                    {
                        if (!args.Type.Equals(Gapcloser.GapcloserType.Targeted)) return;
                        if (MenuValue.Misc.EGap && E.IsReady())
                        {
                            E.Cast(sender);
                        }                       
                    }
                    break;
            }       
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            if (Q.IsReady() && MenuValue.Misc.QI && Q.IsInRange(sender))
            {
                Q.Cast(sender);
            }
            else if (W.IsReady() && MenuValue.Misc.WI)
            {
                W.Cast();
            }
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            return;
        }
        #endregion

        #region Damage

        #region DamageRaw
        protected static float QDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 75f, 115f, 165f, 230f, 300f }[Q.Level] + 0.9f * player.TotalMagicalDamage);
        }
        protected static float WDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 75f, 115f, 165f, 230f, 300f }[Q.Level] + 0.9f * player.TotalMagicalDamage);
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
                default:
                    {
                        float damage = 0f;

                        if (Q.IsReady() || target.HasQZileanBuff())
                        {
                            damage = damage + QDamage(target);
                        }
                        if (W.IsReady())
                        {
                            damage = damage + WDamage(target);
                        }
                        if (Orbwalker.CanAutoAttack)
                        {
                            damage = damage + player.GetAutoAttackDamage(target, true);
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
                public static int QHitChance { get { return Menu.GetSlotHitChance(SpellSlot.Q); } }
            }

            internal static class Auto
            {
                static string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".Saver.";

                public static bool Enable { get { return AutoMenu.VChecked(BeginText + "Enable"); } }

                public static bool EnableWith(AIHeroClient champ) { return AutoMenu.VChecked(BeginText + "Enable." + champ.Hero); }

                public static bool EnablePred(AIHeroClient champ) { return AutoMenu.VChecked(BeginText + "Prediction." + champ.Hero); }

                public static int HP(AIHeroClient champ) { return AutoMenu.VSliderValue(BeginText + "HP." + champ.Hero); }

                public static int ChampPriority(AIHeroClient champ) { return CrazyTargetSelector.GetPriority(champ, AutoMenu); }
            }
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int LogicE { get { return ComboMenu.GetSlotComboBox(SpellSlot.E); } }
            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return HarassMenu.GetSlotCheckBox(SpellSlot.E); } }

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

                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool WKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool QGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] dangerValue { get { return MiscMenu.GetDangerValue(); } }

                public static bool QI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.Interrupter.ToString()); } }

                public static bool WI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.Interrupter.ToString()); } }
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
