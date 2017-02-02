using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System;
using System.Linq;
using System.Collections.Generic;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Log;
using Color = System.Drawing.Color;

namespace UBAddons.Champions.Syndra
{
    internal class Syndra : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static int LastWCast { get; set; }
        protected static bool IsCombo { get; set; }
        protected static bool IsWHolding { get; set; }
        protected static bool IsHoldingPet { get; set; }
        protected static IEnumerable<Obj_AI_Minion> Balls
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValid && !x.IsDead && x.IsAlly && x.Name.Equals("seed"));
            }
        }
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Skillshot EQ { get; set; }
        protected static Spell.SpellBase R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu LastHitMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Syndra()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
                CastDelay = 600,
            };

            W = new Spell.Skillshot(SpellSlot.W, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            E = new Spell.Skillshot(SpellSlot.E, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            EQ = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Linear, 600, 2500, 65, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            R = new Spell.Targeted(SpellSlot.R, 675, DamageType.Magical);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            Game.OnUpdate += delegate (EventArgs args)
            {
                IsWHolding = player.HasBuff("syndrawtooltip");
                var EnemyPet = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsPet() && x.IsValidTarget(W.Range) && !x.IsAlly);
                if (EnemyPet.Any())
                {
                    if ((!IsWHolding || W.ToggleState != 2) && LastWCast < Core.GameTickCount - 400)
                    {
                        if (W.Cast(EnemyPet.FirstOrDefault()))
                        {
                            LastWCast = Core.GameTickCount;
                            IsHoldingPet = true;
                        }
                    }
                }
                if (!IsWHolding)
                {
                    IsHoldingPet = IsWHolding;
                }
                else
                {
                    IsHoldingPet = ObjectManager.Get<Obj_AI_Minion>().Any(x => x.IsPet() && x.IsValid && !x.IsDead && x.HasBuff("syndrawbuff") && !x.IsAlly);
                }
                if (W.ToggleState.Equals(2))
                {
                    W.Range = 950;
                }
                else
                {
                    W.Range = 925;
                }
            };
            AIHeroClient.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (!sender.IsMe) return;
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                        {
                            if (E.IsInRange(args.End) && IsCombo && E.IsReady())
                            {
                                E.Cast(args.End);
                                IsCombo = false;
                            }
                            else
                            {
                                IsCombo = false;
                            }
                        }
                        break;
                    case SpellSlot.W:
                        {
                            if (E.IsInRange(args.End) && IsCombo && E.IsReady() && args.IsToggle)
                            {
                                E.Cast(args.End);
                                IsCombo = false;
                            }
                            else
                            {
                                IsCombo = false;
                            }
                        }
                        break;
                    case SpellSlot.E:
                        {
                            IsCombo = false;
                        }
                        break;
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
                Menu.Add(Variables.AddonName + "Syndra.QE.HitChance", new Slider("EQ hitchance prediction", 80));
                Menu.Add(Variables.AddonName + "Syndra.QE.Auto.Hit", new Slider("Auto E if stun {0} champ", 3, 1, 6));
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.Add(Variables.AddonName + ".QE.Enable", new CheckBox("Use Q-E"));
                    ComboMenu.Add(Variables.AddonName + ".WE.Enable", new CheckBox("Use W-E"));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
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
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.Q, 5, 1, 10);
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
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "KillSteal");
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

        #region Combo
        protected static bool CastE(Obj_AI_Base target, bool useQE, bool useWE)
        {
            if (target != null && E.IsReady())
            {
                switch (E.IsInRange(target))
                {
                    case true:
                        {
                            var pred = E.GetPrediction(target);
                            if (pred.CanNext(E, MenuValue.General.EHitChance, true))
                            {
                                return E.Cast(pred.CastPosition);
                            }
                            return false;
                        }
                    case false:
                        {                           
                            if (Q.IsReady() && useQE)
                            {
                                var pred = EQ.GetPrediction(target);
                                if (pred.CanNext(EQ, MenuValue.General.EQHitChance, false))
                                {
                                    IsCombo = true;
                                    return Q.Cast(player.Position.Extend(pred.CastPosition, E.Range - 20).To3DWorld());
                                }
                            }
                            else if (W.IsReady() && W.ToggleState == 2 && useWE && IsWHolding)
                            {
                                var pred = EQ.GetPrediction(target);
                                if (pred.CanNext(EQ, MenuValue.General.EQHitChance, false))
                                {
                                    IsCombo = true;
                                    if (W.Cast(player.Position.Extend(pred.CastPosition, E.Range - 20).To3DWorld()))
                                    {
                                        LastWCast = Core.GameTickCount;
                                        return true;
                                    }
                                }
                                return false;
                            }
                            return false;
                        }
                }
            }
            return false;
        }
        protected static bool TakeShit_and_Cast(Obj_AI_Base target)
        {
            if (W.IsReady())
            {
                if (W.ToggleState != 2 && !IsWHolding && LastWCast < Core.GameTickCount - 400)
                { 
                    if (Balls.Any(x => W.IsInRange(x)))
                    {
                        if (W.Cast(Balls.Where(x => W.IsInRange(x)).FirstOrDefault()))
                        {
                            LastWCast = Core.GameTickCount;
                            return true;
                        }
                    }
                    else
                    {
                        var minion = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(W.Range) && x.IsEnemy && x != target).FirstOrDefault();
                        if (minion != null)
                        {
                            if (W.Cast(minion))
                            {
                                LastWCast = Core.GameTickCount;
                                return true;
                            }
                        }
                    }
                }
                if (W.ToggleState == 2 && IsWHolding && !IsHoldingPet && LastWCast < Core.GameTickCount - 400)
                {
                    var pred = W.GetPrediction(target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, true))
                    {
                        if (W.Cast(pred.CastPosition))
                        {
                            LastWCast = Core.GameTickCount;
                            return true;
                        }                        
                    }
                }
            }
            return false;
        }
        protected static bool TakeShit_and_Cast(Vector3 position)
        {
            if (W.IsReady())
            {
                if (W.ToggleState != 2 && !IsWHolding)
                {
                    if (Balls.Any())
                    {
                        return W.Cast(Balls.FirstOrDefault());
                    }
                    else
                    {
                        var minion = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(W.Range) && x.IsEnemy).FirstOrDefault();
                        if (minion != null)
                        {
                            return W.Cast(minion);
                        }
                    }
                }
                if (W.ToggleState == 2 && IsWHolding && !IsHoldingPet)
                {
                    W.Cast(position);
                }
            }
            return false;
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
            if (E.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : E.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.EGap)
            {
                CastE(sender, true, true);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.DangerValue.Contains(args.DangerLevel)) return;
            if (E.IsReady() && MenuValue.Misc.EI && E.IsInRange(sender))
            {
                CastE(sender, true, true);
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
                    TakeShit_and_Cast(target);
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
        #endregion

        #region Damage

        #region DamageRaw
        protected static float QDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 50f, 95f, 140f, 185f, 230f }[Q.Level] + 0.75f * player.TotalMagicalDamage);
        }

        protected static float WDamage(Obj_AI_Base target)
        {
            var raw = new[] { 0f, 70f, 110f, 140f, 190f, 230f }[W.Level] + 0.7f * player.TotalMagicalDamage;
            return player.CalculateDamageOnUnit(target, DamageType.Magical, raw)
                + (W.Level.Equals(5) ? raw * 0.2f : 0);
        }

        protected static float EDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 70f, 115f, 160f, 205f, 250f }[E.Level] + 0.5f * player.TotalMagicalDamage);
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            int Ball = Player.GetSpell(SpellSlot.R).Ammo;
            return player.CalculateDamageOnUnit(target, DamageType.Magical, (new[] { 0f, 90f, 135f, 180f }[R.Level] + 0.2f * player.TotalMagicalDamage) * Ball);
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

                public static int EQHitChance { get { return Menu.VSliderValue(Variables.AddonName + "Syndra.QE.HitChance"); } }

                public static int EQHit { get { return Menu.VSliderValue(Variables.AddonName + "Syndra.QE.Auto.Hit"); } }

            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseQE { get { return ComboMenu.VChecked(Variables.AddonName + ".QE.Enable"); } }

                public static bool UseWE { get { return ComboMenu.VChecked(Variables.AddonName + ".WE.Enable"); } }

                public static bool UseEQ { get { return ComboMenu.VChecked(Variables.AddonName + ".EQ.Enable"); } }

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

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool QGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] DangerValue { get { return MiscMenu.GetDangerValue(); } }

                public static bool EI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.Interrupter.ToString()); } }
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
