using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Log;

namespace UBAddons.Champions.Azir
{
    internal class Azir : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Vector3 PositionSelected = new Vector3();
        protected static float LastSetTime = new float();
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Skillshot R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu InsecMenu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LasthitMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Azir()
        {
            Q = new Spell.Skillshot(SpellSlot.Q/*, 875, SkillShotType.Linear, 250, 1200, 70*/, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            Q.Range = Q.Range / 2;
            W = new Spell.Skillshot(SpellSlot.W, 450, SkillShotType.Circular, 250, int.MaxValue, 375, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Linear, 250, 2000, 150, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            R = new Spell.Skillshot(SpellSlot.R , 450, SkillShotType.Linear, 500, 1000, 220, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue
            };
            Chat.Print("Azir not added Insec yet. I'll be waiting for Azir Update");
            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowMessages.LeftButtonDown)
            {
                PositionSelected = new Vector3();
                LastSetTime = Game.Time;
            }
            if (args.Msg == (uint)WindowMessages.LeftButtonDoubleClick)
            {
                PositionSelected = Game.CursorPos;
                LastSetTime = Game.Time;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (R.IsLearned && R.IsReady())
            {
                R.Width = 107 * (R.Level - 1) < 200 ? 220 : (107 * (R.Level - 1)) + 5;
                R.SourcePosition = player.Position.Extend(player.Direction, -325).To3DWorld();
            }
            if (LastSetTime + 10f < Game.Time)
            {
                PositionSelected = new Vector3();
            }
        }

        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBAddons.MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.Q);
                Menu.CreatSlotHitChance(SpellSlot.R);
                #endregion

                #region Insec
                InsecMenu = Menu.AddSubMenu("Insec", "UBAddons.Insec" + player.Hero, "UB" + player.Hero + " - Settings your insec below");
                {
                    InsecMenu.Add("UBAddons.Azir.Insec.Key", new KeyBind("Insec Key", true, KeyBind.BindTypes.HoldActive));
                    InsecMenu.Add("UBAddons.Azir.Insec.To", new ComboBox("Insec Target to", 0, "My Cursor", "My Turret", "My Ally", "Select Position"));
                }
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "UB" + player.Hero + " - Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.Add("UBAddons.Azir.W.Soldier", new Slider("Max soldier", 2, 1, 3));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.Add("UBAddons.Azir.E.OnlyCanKill", new CheckBox("E to Killable Target only"));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.CreatSlotComboBox(SpellSlot.R, 0, "Smart", "Ally", "Turret", "Push");
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "UB" + player.Hero + " - Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.Add("UBAddons.Azir.W.Soldier", new Slider("Max soldier", 2, 1, 3));
                    HarassMenu.CreatManaLimit();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "UB" + player.Hero + " - Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.Q, 5, 1, 10);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.Add("UBAddons.Azir.W.Soldier", new Slider("Max soldier", 2, 1, 3));
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "UB" + player.Hero + " - Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W);
                    JungleClearMenu.Add("UBAddons.Azir.W.Soldier", new Slider("Max soldier", 2, 1, 3));
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LasthitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "UB" + player.Hero + " - Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "KillSteal");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings");
                {
                    DrawMenu.CreatDrawingOpening();
                    DrawMenu.Add("UBAddons.Insec.Position", new CheckBox("Draw Selected Position"));
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

        #region Misc
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid) return;
            if (R.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : R.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.RGap)
            {
                R.Cast(sender);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid || !MenuValue.Misc.DangerValue.Contains(args.DangerLevel)) return;
            if (R.IsReady() && MenuValue.Misc.RI)
            {
                R.Cast(sender);
            }
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValid) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (player.ManaPercent < MenuValue.LastHit.ManaLimit) return;
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.Q) && MenuValue.LastHit.UseQ)
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth > float.Epsilon)
                {
                    Q.Cast(target);
                }
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        public static float QDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.Default);
        }
        public static float WDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.W, DamageLibrary.SpellStages.Default);
        }
        public static float EDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.E, DamageLibrary.SpellStages.Default);
        }
        public static float RDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.R, DamageLibrary.SpellStages.Default);
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
            if (MenuValue.Drawings.DrawW && (!MenuValue.Drawings.ReadyW || W.IsReady()))
            {
                W.DrawRange(MenuValue.Drawings.ColorQ);
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

                public static int RHitChance { get { return Menu.GetSlotHitChance(SpellSlot.R); } }

            }
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int MaxSoldier { get { return ComboMenu.VSliderValue("UBAddons.Azir.W.Soldier"); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseEKillable { get { return ComboMenu.VChecked("UBAddons.Azir.E.OnlyCanKill"); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int RWhere { get { return ComboMenu.GetSlotComboBox(SpellSlot.R); } }
            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int MaxSoldier { get { return HarassMenu.VSliderValue("UBAddons.Azir.W.Soldier"); } }

                public static int ManaLimit { get { return HarassMenu.GetManaLimit(); } }

                public static bool IsAuto { get { return HarassMenu.GetHarassKeyBind(); } }
            }

            internal static class LaneClear
            {
                public static bool EnableIfNoEnemies { get { return LaneClearMenu.GetNoEnemyOnly(); } }

                public static int ScanRange { get { return LaneClearMenu.GetDetectRange(); } }

                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }

                public static bool OnlyKillable { get { return LaneClearMenu.GetKillableOnly(); } }

                public static int MaxSoldier { get { return LaneClearMenu.VSliderValue("UBAddons.Azir.W.Soldier"); } }

                public static bool UseQ { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }
            }

            internal static class JungleClear
            {

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int MaxSoldier { get { return JungleClearMenu.VSliderValue("UBAddons.Azir.W.Soldier"); } }

                public static bool UseE { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode { get { return LasthitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LasthitMenu.PreventCombo(); } }

                public static bool UseQ { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static int ManaLimit { get { return LasthitMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool WKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool RGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] DangerValue { get { return MiscMenu.GetDangerValue(); } }

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

                public static bool DrawE { get { return DrawMenu.GetDrawCheckValue(SpellSlot.E); } }

                public static bool ReadyE { get { return DrawMenu.GetOnlyReady(SpellSlot.E); } }

                public static SharpDX.Color ColorE { get { return DrawMenu.GetColorPicker(SpellSlot.E).ToSharpDX(); } }

                public static bool DrawR { get { return DrawMenu.GetDrawCheckValue(SpellSlot.R); } }

                public static bool ReadyR { get { return DrawMenu.GetOnlyReady(SpellSlot.R); } }

                public static SharpDX.Color ColorR { get { return DrawMenu.GetColorPicker(SpellSlot.R).ToSharpDX(); } }

                public static bool DrawDamageIndicator { get { return DrawMenu.GetDrawCheckValue(SpellSlot.Unknown); } }

                public static System.Drawing.Color ColorDmg { get { return DrawMenu.GetColorPicker(SpellSlot.Unknown); } }

            }
        }
        #endregion

        #region R Method
        internal enum I_To
        {
            Turret, Ally, Cursor, Smart, Push, Pull, Position,
        }
        internal static void CastR(I_To tO, Obj_AI_Base target = null)
        {
            if (target == null)
            {
                target = R.GetTarget();
            }
            switch (tO)
            {
                case I_To.Ally:
                    {
                        if (target != null)
                        {
                            var Ally = EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(1200)).FirstOrDefault();
                            if (Ally != null)
                            {
                                R.Cast(player.Position.Extend(Ally, R.Range).To3DWorld());
                            }
                            else
                            {
                                UBNotification.ShowNotif("UBAddons Warning", "Ally is null", "warn");
                            }
                        }
                        else
                        {
                            UBNotification.ShowNotif("UBAddons Warning", "Target is null", "warn");
                        }
                    }
                    break;
                case I_To.Turret:
                    {
                        if (target != null)
                        {
                            var Turret = EntityManager.Turrets.Allies.Where(x => x.IsValidTarget(900, false, target.Position)).FirstOrDefault();
                            if (Turret != null)
                            {
                                R.Cast(player.Position.Extend(Turret, R.Range).To3DWorld());
                            }
                            else
                            {
                                UBNotification.ShowNotif("UBAddons Warning", "Ally is null", "warn");
                            }
                        }
                        else
                        {
                            UBNotification.ShowNotif("UBAddons Warning", "Target is null", "warn");
                        }
                    }

                    break;
                case I_To.Smart:
                    if (target != null)
                    {
                        var Turret = EntityManager.Turrets.Allies.Where(x => x.IsValidTarget(900, false, target.Position)).FirstOrDefault();
                        if (Turret != null)
                        {
                            R.Cast(player.Position.Extend(Turret, R.Range).To3DWorld());
                        }
                        else
                        {
                            var Ally = EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(1200)).FirstOrDefault();
                            if (Ally != null)
                            {
                                R.Cast(player.Position.Extend(Ally, R.Range).To3DWorld());
                            }
                        }
                    }
                    else
                    {
                        UBNotification.ShowNotif("UBAddons Warning", "Target is null", "warn");
                    }
                    break;
                case I_To.Cursor:
                    {
                        if (target != null)
                        {
                            R.Cast(player.Position.Extend(Game.CursorPos, R.Range).To3DWorld());
                        }
                    }
                    break;
                case I_To.Push:
                    {
                        if (target != null)
                        {
                            var pred = R.GetPrediction(target);
                            if (pred.CanNext(R, MenuValue.General.RHitChance, true))
                            {
                                R.Cast(pred.CastPosition);
                            }
                        }
                    }
                    break;
                case I_To.Pull:
                    {
                        if (target != null)
                        {
                            var pred = R.GetPrediction(target);
                            if (pred.CanNext(R, MenuValue.General.RHitChance, true))
                            {
                                R.Cast(pred.CastPosition.Extend(player, player.Distance(pred.CastPosition) + R.Range).To3DWorld());
                            }
                        }
                    }
                    break;
                case I_To.Position:
                    {

                    }
                    break;
            }
        }
        #endregion
    }
}
