using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Libs.Dictionary;
using UBAddons.Log;

namespace UBAddons.Champions.Viktor
{
    internal class Viktor : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static bool QUpgrade { get; set; }
        protected static bool WUpgrade { get; set; }
        protected static bool EUpgrade { get; set; }

        protected static Spell.Targeted Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.SimpleSkillshot E1 { get; set; }

        protected static Spell.Skillshot R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LasthitMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Viktor()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 650, DamageType.Magical);
            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular, 250, int.MaxValue, 300)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            E = new Spell.Skillshot(SpellSlot.E, 1225, SkillShotType.Linear, 50, 1050, 100)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            E1 = new Spell.SimpleSkillshot(SpellSlot.E, 525, DamageType.Magical)
            {
                CastDelay = 50,
            };
            R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 250, int.MaxValue, 450)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            UtilityPlugin.AddPlugin(EUtility.ImmobileTracker);
            DamageIndicator.DamageDelegate = HandleDamageIndicator;
        }

        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBAddons.MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.W);
                Menu.Add("UBAddons.Viktor.E.Direction", new CheckBox("Allow use E direction"));
                Menu.AddLabel("Tips: Turn on direction will make your E look like human");
                Menu.CreatSlotHitChance(SpellSlot.E);
                Menu.CreatSlotHitChance(SpellSlot.R);
                Menu.AddGroupLabel("Immobile Setting");
                Menu.Add("UBAddons.Viktor.W.CC", new CheckBox("W CC stacks"));
                Menu.Add("UBAddons.Viktor.W.Teleport", new CheckBox("W Teleport"));
                Menu.Add("UBAddons.Viktor.W.Revive", new CheckBox("W Revive"));

                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "UB" + player.Hero + " - Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    ComboMenu.CreatSlotHitSlider(SpellSlot.W, 1, 1, 5);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.CreatSlotHitSlider(SpellSlot.R, 2, 1, 5);
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "UB" + player.Hero + " - Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    HarassMenu.CreatSlotHitSlider(SpellSlot.W, 1, 1, 5);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.CreatManaLimit();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "UB" + player.Hero + " - Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.AddLabel("W only work if you upgraded");
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.W, 5, 1, 10);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.E, 5, 1, 10);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "UB" + player.Hero + " - Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    //JungleClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    //JungleClearMenu.CreatSlotHitSlider(SpellSlot.W, 1, 1, 6);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LasthitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "UB" + player.Hero + " - Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "Interrupter", false);
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "KillSteal");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings");
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

        #region Misc
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid) return;
            if (W.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 350 : W.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.WGap)
            {
                W.Cast(sender);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.GetdangerValue().Contains(args.DangerLevel)) return;
            if (W.IsReady() && MenuValue.Misc.WI)
            {
                W.Cast(sender);
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
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.E) && MenuValue.LastHit.UseE && E.IsReady())
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                CastE(target);
            }
        }
        internal static bool CastE(Obj_AI_Base target)
        {
            if (target == null || !E.IsReady())
            {
                E.SourcePosition = null;
                return false;
            }
            else
            {
                if (E1.IsInRange(target))
                {
                    E.SourcePosition = target.Position;
                    var otherchamp = EntityManager.Heroes.Enemies.Where(x => x != target);
                    var othertarget = E.GetTarget(otherchamp);
                    var pred = E.GetPrediction(target);
                    if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                    {
                        if (othertarget == null)
                        {
                            Vector3[] location = target.Position.Symmetry(pred.UnitPosition, 335);
                            Vector3 start = E1.IsInRange(location[0]) ? location[0] : location[1];
                            Vector3 end = E1.IsInRange(location[0]) ? location[1] : location[0];
                            if (E.CastStartToEnd(end, start))
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            var pred2 = E.GetPrediction(othertarget);
                            if (pred2.CastPosition.Distance(target) <= E.Range - E1.Range)
                            {
                                if (E.CastStartToEnd(pred2.CastPosition, target.Position))
                                {
                                    return true;
                                }
                                return false;
                            }
                            else
                            {
                                Vector3[] location = target.Position.Symmetry(pred.UnitPosition, 335);
                                Vector3 start = E1.IsInRange(location[0]) ? location[0] : location[1];
                                Vector3 end = E1.IsInRange(location[0]) ? location[1] : location[0];
                                if (E.CastStartToEnd(end, start))
                                {
                                    return true;
                                }
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    E.SourcePosition = player.Position.Extend(target, E1.Range).To3DWorld();
                    var pred = E.GetPrediction(target);
                    Vector2 intersection1;
                    Vector2 intersection2;
                    int count = Geometry.LineCircleIntersection(player.Position.X, player.Position.Y, E1.Range, pred.CastPosition.To2D(), pred.CastPosition.Extend(target.Position, 600), out intersection1, out intersection2);
                    Vector3[] intersection = new[] { intersection1.To3DWorld(), intersection2.To3DWorld() };
                    if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                    {
                        if (E1.IsInRange(pred.CastPosition))
                        {
                            if (E.CastStartToEnd(pred.CastPosition, player.Position.Extend(pred.CastPosition, E1.Range - 250).To3DWorld()))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (count != 0 && E.Range - player.Distance(pred.CastPosition) > 100 && MenuValue.General.EDirection)
                            {
                                if (E.CastStartToEnd(pred.CastPosition, intersection.OrderBy(x => x.Distance(pred.CastPosition)).FirstOrDefault()))
                                {
                                    return true;
                                }
                            }
                            else if (E.CastStartToEnd(pred.CastPosition, player.Position.Extend(pred.CastPosition, E1.Range - 5).To3DWorld()))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
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
        protected static float EDamage(Obj_AI_Base target)
        {
            var firstdamage = player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 70f, 110f, 150f, 190f, 230f }[E.Level] + 0.5f * player.TotalMagicalDamage);
            var seconddamage = player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 20f, 60f, 100f, 140f, 180f }[E.Level] + 0.7f * player.TotalMagicalDamage);
            if (!EUpgrade)
            {
                return firstdamage;
            }
            else
            {
                if (!(target is AIHeroClient))
                {
                    return firstdamage + seconddamage;
                }
                else
                {
                    if (ImmobileTracker.WillBeImmobile(target, 1))
                    {
                        return firstdamage + seconddamage;
                    }
                    else
                        return firstdamage;
                }
            }
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            var champ = target as AIHeroClient;
            var raw = player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 100f, 175f, 250f }[R.Level] + 0.5f * player.TotalMagicalDamage);
            if (champ != null)
            {
                if (champ.BrainIsCharged())
                {
                    return raw;
                }
                else
                    return raw + player.CalculateDamageOnUnit(target, DamageType.Magical, new float[] { 0f, 150f, 250f, 350f }[R.Level] + 0.6f * player.TotalMagicalDamage);
            }
            else
            {
                return raw;
            }
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
                W.DrawRange(MenuValue.Drawings.ColorW);
            }
            if (MenuValue.Drawings.DrawE && (!MenuValue.Drawings.ReadyE || E.IsReady()))
            {
                E.DrawRange(MenuValue.Drawings.ColorE);
                E1.DrawRange(MenuValue.Drawings.ColorE);
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

                public static bool EDirection { get { return Menu.VChecked("UBAddons.Viktor.E.Direction"); } }

                public static int EHitChance { get { return Menu.GetSlotHitChance(SpellSlot.E); } }

                public static int RHitChance { get { return Menu.GetSlotHitChance(SpellSlot.R); } }

                public static bool WCCStacks { get { return Menu.VChecked("UBAddons.Viktor.W.CC"); } }

                public static bool WTeleport { get { return Menu.VChecked("UBAddons.Viktor.W.Teleport"); } }

                public static bool WRevive { get { return Menu.VChecked("UBAddons.Viktor.W.Revive"); } }

            }
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int Whit { get { return ComboMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int Rhit { get { return ComboMenu.GetSlotHitSlider(SpellSlot.R); } }

            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int Whit { get { return HarassMenu.GetSlotHitSlider(SpellSlot.W); } }

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

                public static int Whit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int Ehit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.E); } }

                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static int Whit { get { return JungleClearMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static bool UseE { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode { get { return LasthitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LasthitMenu.PreventCombo(); } }

                public static bool UseQ { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseE { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return LasthitMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] GetdangerValue()
                {
                    return MiscMenu.GetDangerValue();
                }

                public static bool WI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.Interrupter.ToString()); } }

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
    }
}
