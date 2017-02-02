using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System;
using System.Drawing;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Log;

namespace UBAddons.Champions.Ekko
{
    internal class Ekko : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Skillshot Q { get; set; }
        protected static MissileClient QMissile { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Active R { get; set; }
        protected static Obj_GeneralParticleEmitter Ekko_Kage_Bunshin { get { return ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(x => x.Name.Equals("Ekko_Base_R_TrailEnd.troy")); } }//Because I like Shadow Clone


        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LastHitMenu { get; set; }

        protected static Menu FleeMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu AutoMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Ekko()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1075, SkillShotType.Linear, 250, 2200, 60, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            W = new Spell.Skillshot(SpellSlot.W, 1600, SkillShotType.Circular, 1500, 500, 650);

            E = new Spell.Skillshot(SpellSlot.E, 325, SkillShotType.Linear, 0, 1200, (int)player.BoundingRadius, DamageType.Mixed);

            R = new Spell.Active(SpellSlot.R, 375);

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

                #region Auto
                AutoMenu = Menu.AddSubMenu("R Settings", "UBAddons.R" + player.Hero, "Settings your R below");
                {
                    string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".R.";
                    AutoMenu.Add(BeginText + "Enable", new CheckBox("Use R"));
                    AutoMenu.Add(BeginText + "NotCombo", new CheckBox("Prevent R when I not in Combo"));
                    AutoMenu.Add(BeginText + "Hit", new Slider("R when hit {0} champ", 3, 1, 6));
                    AutoMenu.Add(BeginText + "MyHP", new Slider("Use R when my HP below", 15));
                    AutoMenu.Add(BeginText + "Prediction", new CheckBox("Use Prediction"));
                }
                #endregion 

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.Add(Variables.AddonName + "." + Player.Instance.Hero + "W.Prediction", new CheckBox("Use W Prediction", false));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotComboBox(SpellSlot.E, 0, "To Mouse", "To Side", "To Target");
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.CreatSlotComboBox(SpellSlot.E, 0, "To Mouse", "To Side", "To Target");
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LastHitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LastHitMenu.CreatLasthitOpening();
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LastHitMenu.CreatManaLimit();
                }
                #endregion

                #region Flee
                FleeMenu = Menu.AddSubMenu("Flee", "UBAddons.Flee" + player.Hero, "Setting your flee below");
                {
                    string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".";
                    FleeMenu.Add(BeginText + "E", new CheckBox("Use E for flee"));
                    FleeMenu.Add(BeginText + "E.Disable", new CheckBox("Disable E when jump spot near"));
                    FleeMenu.Add(BeginText + "E.Distance", new Slider("Range for disable", 600, 0, 1000));
                    FleeMenu.Add(BeginText + "AA", new CheckBox("AA for flee"));
                    FleeMenu.Add(BeginText + "AA.ToMonster", new CheckBox("AA to monster"));
                    FleeMenu.Add(BeginText + "AA.ToChamp", new CheckBox("AA to champ"));
                    FleeMenu.Add(BeginText + "AA.HP", new Slider("Min {0}% HP for AA champ & monster", 15));
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "KillSteal");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings", "UBAddons.Drawings" + player.Hero, "Settings your drawings below");
                {
                    DrawMenu.CreatDrawingOpening();
                    DrawMenu.CreatColorPicker(SpellSlot.Q);
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
            if (MenuValue.Misc.EGap && args.End.Distance(player) <= 325)
            {
                E.Cast(args.Start.Extend(args.End, E.Range).To3DWorld());
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            if (Q.IsReady() && MenuValue.Misc.QI)
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
            if (target == null || target.IsInvulnerable || !target.IsValidTarget()) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (player.ManaPercent < MenuValue.LastHit.ManaLimit) return;
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.Q) && MenuValue.LastHit.UseQ && Q.IsReady())
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                Q.Cast(target);
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        protected static float PassiveDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 30, 40, 50, 60, 70, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140 }[player.Level] + 0.8f * player.TotalMagicalDamage);
        }
        protected static float QDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 60f, 75f, 90f, 105f, 120f }[Q.Level] + 0.3f * player.TotalMagicalDamage);
        }
        protected static float WDamage(Obj_AI_Base target)
        {
            if (100 - target.HealthPercent > 30f)
            {
                return 0;
            }
            else
            {
                return player.CalculateDamageOnUnit(target, DamageType.Magical, (3 + 0.0303f * player.TotalMagicalDamage) * (target.MaxHealth - target.Health));
            }
        }
        protected static float EDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 40f, 65f, 90f, 115f, 140f }[E.Level] + 0.4f * player.TotalMagicalDamage);
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.R);
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

                        if (target.HasBuff("ekkostacks") && !target.HasBuff("ekkostunmarker"))
                        {
                            damage = damage + PassiveDamage(target);
                        }
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
                if (Ekko_Kage_Bunshin != null)
                {
                    Drawing.DrawText(player.HPBarPosition.X + 0, player.HPBarPosition.Y - 20, MenuValue.Drawings.ColorR.ToSystem(), "R Will Hit: " + Ekko_Kage_Bunshin.CountEnemyHeroesInRangeWithPrediction((int)R.Range, 500), 12);
                    Drawing.DrawText(Ekko_Kage_Bunshin.Position.WorldToScreen().X - 40, Ekko_Kage_Bunshin.Position.WorldToScreen().Y + 20, MenuValue.Drawings.ColorR.ToSystem(), "R Will Hit: " + Ekko_Kage_Bunshin.CountEnemyHeroesInRangeWithPrediction((int)R.Range, 500), 12);
                    Circle.Draw(MenuValue.Drawings.ColorR, R.Range, Ekko_Kage_Bunshin);
                }
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
                static string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".R.";

                public static bool Enable { get { return AutoMenu.VChecked(BeginText + "Enable"); } }

                public static bool NotCombo { get { return AutoMenu.VChecked(BeginText + "NotCombo"); } }

                public static bool EnablePred { get { return AutoMenu.VChecked(BeginText + "Prediction"); } }

                public static int HP { get { return AutoMenu.VSliderValue(BeginText + "MyHP"); } }

                public static int ChampHit { get { return AutoMenu.VSliderValue(BeginText + "Hit"); } }
            }
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool Prediction { get { return ComboMenu.VChecked(Variables.AddonName + "." + Player.Instance.Hero + "W.Prediction"); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int LogicE { get { return ComboMenu.GetSlotComboBox(SpellSlot.E); } }
            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return HarassMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return HarassMenu.GetManaLimit(); } }

                public static int LogicE { get { return HarassMenu.GetSlotComboBox(SpellSlot.E); } }

                public static bool IsAuto { get { return HarassMenu.GetHarassKeyBind(); } }
            }

            internal static class LaneClear
            {
                public static bool EnableIfNoEnemies { get { return LaneClearMenu.GetNoEnemyOnly(); } }

                public static int ScanRange { get { return LaneClearMenu.GetDetectRange(); } }

                public static bool OnlyKillable { get { return LaneClearMenu.GetKillableOnly(); } }

                public static bool UseQ { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

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

                public static bool UseQ { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static int ManaLimit { get { return LastHitMenu.GetManaLimit(); } }
            }


            internal static class Flee
            {
                private static string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".";

                public static bool UseE { get {  return FleeMenu.VChecked(BeginText + "E"); } }

                public static bool DisableWall { get { return FleeMenu.VChecked(BeginText + "E.Disable"); } }

                public static int Distance { get { return FleeMenu.VSliderValue(BeginText + "E.Distance"); } }

                public static bool UseAA { get {  return FleeMenu.VChecked(BeginText + "AA"); } }

                public static bool AAMonster { get { return FleeMenu.VChecked(BeginText + "AA.ToMonster"); } }

                public static bool AAChamp { get { return FleeMenu.VChecked(BeginText + "AA.ToChamp"); } }

                public static int HP { get { return FleeMenu.VSliderValue(BeginText + "AA.HP"); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }


                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

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
