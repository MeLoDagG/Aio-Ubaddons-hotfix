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

namespace UBMiddle.Champions.AurelionSol
{
    internal class AurelionSol : ChampionPlugin
    {
        internal static AIHeroClient player = Player.Instance;
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Active W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Skillshot R { get; set; }

        internal static Menu Menu { get; set; }
        internal static Menu ComboMenu { get; set; }
        internal static Menu HarassMenu { get; set; }
        internal static Menu LaneClearMenu { get; set; }
        internal static Menu JungleClearMenu { get; set; }
        internal static Menu LastHitMenu { get; set; }
        internal static Menu MiscMenu { get; set; }
        internal static Menu DrawMenu { get; set; }

        static AurelionSol()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical);
            W = new Spell.Active(SpellSlot.W, 600, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E);
            R = new Spell.Skillshot(SpellSlot.R, DamageType.Magical);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            
        }

        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBMiddle.MainMenu" + player.Hero, "UB" + player.Hero + " - UBMiddle - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.Q);
                Menu.CreatSlotHitChance(SpellSlot.R);
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBMiddle.ComboMenu" + player.Hero, "UB" + player.Hero + " - Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.CreatSlotHitSlider(SpellSlot.R, 2, 1, 5);
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBMiddle.HarassMenu" + player.Hero, "UB" + player.Hero + " - Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotHitSlider(SpellSlot.W, 1, 1, 5);
                    HarassMenu.CreatManaLimit(true);
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBMiddle.LaneClear" + player.Hero, "UB" + player.Hero + " - Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.Add("UBMiddle.Annie.Passive.Off", new CheckBox("Stop if I has stun"));
                    LaneClearMenu.Add("UBMiddle.Annie.Passive.Count.Buff", new Slider("Stop if my stacks", 3, 1, 4));
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.W, 5, 1, 10);
                    LaneClearMenu.CreatManaLimit(true);
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBMiddle.JungleClear" + player.Hero, "UB" + player.Hero + " - Settings your jungleclear below");
                {
                    JungleClearMenu.Add("UBMiddle.Annie.Passive.Off", new CheckBox("Stop if I has stun", false));
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    JungleClearMenu.CreatSlotHitSlider(SpellSlot.W, 1, 1, 6);
                    JungleClearMenu.CreatManaLimit(true);
                }
                #endregion

                #region Lasthit
                LastHitMenu = Menu.AddSubMenu("Lasthit", "UBMiddle.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LastHitMenu.CreatLasthitOpening();
                    LastHitMenu.Add("UBMiddle.Annie.Passive.Off", new CheckBox("Stop if I has stun"));
                    LastHitMenu.Add("UBMiddle.Annie.Passive.Count.Buff", new Slider("Stop if my stacks", 4, 1, 4));
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.W);
                    LastHitMenu.CreatManaLimit(true);
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBMiddle.Misc" + player.Hero, "UB" + player.Hero + " - Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.AddLabel("Will Check if only stun");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.AddLabel("[repeat] Will Check if only stun");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "Interrupter");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "Interrupter");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "KillSteal");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings");
                {
                    DrawMenu.CreatDrawingOpening();
                    DrawMenu.CreatColorPicker(SpellSlot.Q);
                    DrawMenu.CreatColorPicker(SpellSlot.W);
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
            if (E.IsReady() && MenuValue.Misc.EGap && (E.IsInRange(args.End) || sender.IsAttackingPlayer))
            {
                E.Cast();
            }
            if (Has_Stun)
            {
                if (Q.IsReady() && MenuValue.Misc.QGap && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : Q.IsInRange(args.End) || sender.IsAttackingPlayer))
                {
                    Q.Cast(sender);
                }
                if (W.IsReady() && MenuValue.Misc.WGap && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : W.IsInRange(args.End) || sender.IsAttackingPlayer))
                {
                    W.Cast(sender);
                }
                if (R.IsReady() && MenuValue.Misc.RGap && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : R.IsInRange(args.End) || sender.IsAttackingPlayer))
                {
                    R.Cast(sender);
                }
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            if (Has_Stun)
            {
                if (Q.IsReady() && MenuValue.Misc.QI && Q.IsInRange(sender))
                {
                    Q.Cast(sender);
                }
                if (W.IsReady() && MenuValue.Misc.WI && W.IsInRange(sender))
                {
                    W.Cast(sender);
                }
                if (R.IsReady() && MenuValue.Misc.RI && R.IsInRange(sender))
                {
                    R.Cast(sender);
                }
            }
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValid) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if ((Has_Stun && MenuValue.LastHit.Stop) || (Passive_Count <= MenuValue.LastHit.Stopifhas)) return;
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.Q) && MenuValue.LastHit.UseQ)
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                Q.Cast(target);
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.W) && MenuValue.LastHit.UseW && target.HasBuff("kennenmarkofstorm"))
            {
                var predHealth = W.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                W.Cast();
            }
        }
        protected override void OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            return;
        }
        #endregion

        #region Damage

        #region DamageRaw
        public static float QDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.Q);
        }
        public static float WDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.W);
        }
        public static float EDamage(Obj_AI_Base target)
        {
            return 0;
        }
        public static float RDamage(Obj_AI_Base target)
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

        protected override bool IsAutoHarass
        {
            get { return MenuValue.Harass.IsAuto; }
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
                public static int WHitChance { get { return Menu.GetSlotHitChance(SpellSlot.W); } }

                public static int RHitChance { get { return Menu.GetSlotHitChance(SpellSlot.R); } }

                public static bool EOnAttack { get { return Menu.VChecked("UBMiddle.Annie.E.Attack.Enable"); } }

                public static bool EOnSpell { get { return Menu.VChecked("UBMiddle.Annie.E.Spell.Enable"); } }
            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int RHit { get { return ComboMenu.GetSlotHitSlider(SpellSlot.R); } }

            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WHit { get { return HarassMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static int ManaLimit { get { return HarassMenu.GetManaLimit(); } }

                public static bool IsAuto { get { return HarassMenu.GetHarassKeyBind(); } }
            }

            internal static class LaneClear
            {
                public static bool EnableIfNoEnemies { get { return LaneClearMenu.GetNoEnemyOnly(); } }

                public static int ScanRange { get { return LaneClearMenu.GetDetectRange(); } }

                public static bool OnlyKillable { get { return LaneClearMenu.GetKillableOnly(); } }

                public static bool Stop { get { return LaneClearMenu.VChecked("UBMiddle.Annie.Passive.Off"); } }

                public static int Stopifhas { get { return LaneClearMenu.VSliderValue("UBMiddle.Annie.Passive.Count.Buff"); } }

                public static bool UseQ { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int Whit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {
                public static bool Stop { get { return JungleClearMenu.VChecked("UBMiddle.Annie.Passive.Off"); } }

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode { get { return LastHitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LastHitMenu.PreventCombo(); } }

                public static bool Stop { get { return LastHitMenu.VChecked("UBMiddle.Annie.Passive.Off"); } }

                public static int Stopifhas { get { return LastHitMenu.VSliderValue("UBMiddle.Annie.Passive.Count.Buff"); } }

                public static bool UseQ { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int ManaLimit { get { return LastHitMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool WKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool QGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool RGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] dangerValue { get { return MiscMenu.GetDangerValue(); } }

                public static bool QI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.Interrupter.ToString()); } }

                public static bool WI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.Interrupter.ToString()); } }

                public static bool EI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.Interrupter.ToString()); } }

                public static bool RI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.Interrupter.ToString()); } }
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
