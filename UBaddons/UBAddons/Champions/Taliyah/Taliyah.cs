using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using SharpDX;
using System;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Log;
using Color = System.Drawing.Color;

namespace UBAddons.Champions.Taliyah
{
    internal class Taliyah : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.SpellBase R { get; set; }
        protected static bool In_Q_Side { get; set; }
        protected static GameObject E_Object { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu LastHitMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Taliyah()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = 0,
            };

            W = new Spell.Skillshot(SpellSlot.W, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Cone, 250, 1000, 20, DamageType.Magical)
            {
                ConeAngleDegrees = 60,
                AllowedCollisionCount = int.MaxValue,
            };

            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 250, null, 150, DamageType.Mixed);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;

            Obj_GeneralParticleEmitter.OnCreate += delegate(GameObject sender, EventArgs args)
            {
                if (sender.Name.Equals("Taliyah_Base_E_MineRumbleSound.troy"))
                {
                    E_Object = sender;
                }
                if (sender.Name.Equals("Taliyah_Base_Q_aoe_bright.troy"))
                {
                    In_Q_Side = true;
                }
            };
            Obj_GeneralParticleEmitter.OnDelete += delegate(GameObject sender, EventArgs args)
            {
                if (sender.Name.Equals("Taliyah_Base_E_Timeout.troy"))
                {
                    E_Object = null;
                }
                if (sender.Name.Equals("Taliyah_Base_Q_aoe_bright.troy"))
                {
                    In_Q_Side = false;
                }
            };
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
                Menu.CreatSlotHitChance(SpellSlot.W);
                Menu.CreatSlotHitChance(SpellSlot.E);
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotComboBox(SpellSlot.W, 2, "Pull", "Push", "Smart");
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotComboBox(SpellSlot.W, 0, "Smart", "Push", "Pull");
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
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.E, 5, 1, 10);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region LastHit
                LastHitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LastHitMenu.CreatLasthitOpening();
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.W);
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.E);
                    LastHitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "KillSteal");
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
            if (W.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : W.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.WGap)
            {
                CastW(sender);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            if (W.IsReady() && MenuValue.Misc.WI && W.IsInRange(sender))
            {
                CastW(sender);
            }
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValidTarget()) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (player.ManaPercent < MenuValue.LastHit.ManaLimit) return;
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.Q) && MenuValue.LastHit.UseQ && Q.IsReady() && Q.IsInRange(target))
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth > 0)
                {
                    Q.Cast(target);
                }
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.W) && MenuValue.LastHit.UseW && W.IsReady() && W.IsInRange(target))
            {
                var predHealth = W.GetHealthPrediction(target);
                if (predHealth > 0)
                {
                    CastW(target);
                }
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.E) && MenuValue.LastHit.UseE && E.IsReady() && E.IsInRange(target))
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth > 0)
                {
                    E.Cast(target);
                }
            }
        }
        protected static void CastW(Obj_AI_Base target, int logic = 2)
        {
            if (target == null) return;
            var pred = W.GetPrediction(target);
            if (!pred.CanNext(W, MenuValue.General.WHitChance, true)) return;
            Vector3 StartPos;
            switch (logic)
            {
                case 0:
                    {
                        StartPos = pred.CastPosition.Extend(Player.Instance, pred.CastPosition.Distance(Player.Instance) + 200f).To3DWorld();
                        W.CastStartToEnd(StartPos, pred.CastPosition);
                    }
                    break;
                case 1:
                    {
                        StartPos = Player.Instance.Position.Extend(pred.CastPosition, pred.CastPosition.Distance(Player.Instance) + 200f).To3DWorld();
                        W.CastStartToEnd(StartPos, pred.CastPosition);
                    }
                    break;
                case 2:
                    {
                        if (E_Object != null)
                        {
                            StartPos = pred.CastPosition.Extend(E_Object.Position, pred.CastPosition.Distance(E_Object.Position) + 200f).To3DWorld();
                            W.CastStartToEnd(StartPos, pred.CastPosition);
                        }
                        else
                        {
                            if (player.Health > target.Health)
                            {
                                StartPos = pred.CastPosition.Extend(Player.Instance, pred.CastPosition.Distance(Player.Instance) + 200f).To3DWorld();
                                W.CastStartToEnd(StartPos, pred.CastPosition);
                            }
                            else
                            {
                                StartPos = Player.Instance.Position.Extend(pred.CastPosition, pred.CastPosition.Distance(Player.Instance) + 200f).To3DWorld();
                                W.CastStartToEnd(StartPos, pred.CastPosition);
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        protected static float QDamage(Obj_AI_Base target)
        {
            var Damage = new float[] { 0f, 70f, 95f, 120f, 145f, 170f }[Q.Level] + 0.45f * Player.Instance.TotalMagicalDamage;
            if (In_Q_Side)
            {
                return player.CalculateDamageOnUnit(target, DamageType.Magical, Damage);
            }
            else
            {
                return player.CalculateDamageOnUnit(target, DamageType.Magical, Damage + Damage * 2);
            }
        }

        protected static float WDamage(Obj_AI_Base target)
        {
            if (E_Object != null)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 80f, 100f, 120f, 140f, 160f }[W.Level] + (new[] { 0f, 35f, 45f, 55f, 65f, 75f }[E.Level] + 0.2f * player.TotalMagicalDamage) * 1.85f + 0.4f * player.TotalMagicalDamage);
            }
            else
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 80f, 100f, 120f, 140f, 160f }[W.Level] + 0.4f * player.TotalMagicalDamage);
            }
        }

        protected static float EDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 50f, 75f, 100f, 125f, 150f }[E.Level] + 0.3f * player.TotalMagicalDamage);
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

                public static int WHitChance { get { return Menu.GetSlotHitChance(SpellSlot.W); } }

                public static int EHitChance { get { return Menu.GetSlotHitChance(SpellSlot.E); } }
            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WLogic { get { return ComboMenu.GetSlotComboBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WLogic { get { return HarassMenu.GetSlotComboBox(SpellSlot.W); } }

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

                public static int QHit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.Q); } }

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

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
                public static bool OnlyFarmMode = LastHitMenu.OnlyFarmMode();


                public static bool PreventCombo = LastHitMenu.PreventCombo();


                public static bool UseQ = LastHitMenu.GetSlotCheckBox(SpellSlot.Q);


                public static bool UseW = LastHitMenu.GetSlotCheckBox(SpellSlot.W);


                public static bool UseE = LastHitMenu.GetSlotCheckBox(SpellSlot.E);


                public static int ManaLimit = LastHitMenu.GetManaLimit();

            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool WKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool QGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] dangerValue { get { return MiscMenu.GetDangerValue(); } }

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
