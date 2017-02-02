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

namespace UBAddons.Champions.Anivia
{
    internal class Anivia : ChampionPlugin
    {
        protected static AIHeroClient player { get { return Player.Instance; } }
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Targeted E { get; set; }
        protected static Spell.Skillshot R { get; set; }
        protected static Spell.Active QActive { get; set; }
        protected static Spell.Active ROff { get; set; }
        protected static MissileClient QMissile { get; set; }
        protected static Obj_GeneralParticleEmitter Storm { get; set; }
        protected const string IcedName = "aniviaiced";
        protected static int LastQTick { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LasthitMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu AutoMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Anivia()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular, 600, int.MaxValue, 100, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            E = new Spell.Targeted(SpellSlot.E, 650);

            R = new Spell.Skillshot(SpellSlot.R, 750, SkillShotType.Circular, 50, int.MaxValue, 400, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            QActive = new Spell.Active(SpellSlot.Q, 230, DamageType.Magical);

            ROff = new Spell.Active(SpellSlot.R, 400, DamageType.Magical);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;

            #region Event
            Obj_GeneralParticleEmitter.OnCreate += delegate(GameObject sender, EventArgs args)
            {
                var missile = sender as Obj_GeneralParticleEmitter;
                if (sender != null)
                {
                    if (missile.Name.Equals("cryo_storm_green_team.troy"))
                    {
                        Storm = missile;
                    }
                }
            };
            Obj_GeneralParticleEmitter.OnDelete += delegate(GameObject sender, EventArgs args)
            {
                var missile = sender as Obj_GeneralParticleEmitter;
                if (sender != null)
                {
                    if (missile.Name.Equals("cryo_storm_green_team.troy"))
                    {
                        Storm = null;
                    }
                }
            };
            MissileClient.OnCreate += delegate(GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (sender != null && missile.SpellCaster.IsMe)
                {
                    if (missile.SData.Name.Equals("FlashFrostSpell"))
                    {
                        QMissile = missile;
                    }
                }
            };
            MissileClient.OnDelete += delegate(GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (sender != null && missile.SpellCaster.IsMe && missile.SData.Name.Equals("FlashFrostSpell"))
                {
                    QMissile = null;
                }
            };
            AIHeroClient.OnBuffGain += delegate(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
            {
                var hero = sender as AIHeroClient;
                if (hero != null && hero.IsEnemy)
                {
                    if (args.Buff.Name.Equals("chilled") && QActive.IsReady() && QMissile.IsInRange(hero, QActive.Range))
                    {
                        QActive.Cast();
                    }
                    if (args.Buff.Name.Equals(IcedName) && MenuValue.General.AutoE && E.IsReady() && E.IsInRange(hero))
                    {
                        E.Cast(hero);
                    }
                }
            };
            AIHeroClient.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (!sender.IsMe) return;
                if (args.Slot.Equals(SpellSlot.Q))
                {
                    LastQTick = Core.GameTickCount;
                }
            };
            #endregion
        }

        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBAddons.MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.Q);
                Menu.Add("UBAddons.Anivia.E.Auto", new CheckBox("Auto E if x2"));
                Menu.Add("UBAddons.Anivia.R.Auto", new CheckBox("Auto Turn off R"));
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "UB" + player.Hero + " - Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.Add("UBAddons.Anivia.E.Only.Iced", new CheckBox("Only E if x2"));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "UB" + player.Hero + " - Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.Add("UBAddons.Anivia.E.Only.Iced", new CheckBox("Only E if x2"));
                    HarassMenu.CreatSlotCheckBox(SpellSlot.R);
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "UB" + player.Hero + " - Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.Q, 5, 1, 10);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.R, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.R, 5, 1, 10);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "UB" + player.Hero + " - Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.R);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.E);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.R);
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
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "Interrupter");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
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
            if (Q.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : Q.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.QGap)
            {
                Q.Cast(sender);
            }
            else if (W.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : W.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.WGap)
            {
                W.Cast(player.Position.Extend(args.End, player.Distance(args.End) - 20f).To3DWorld());
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid || !MenuValue.Misc.DangerValue.Contains(args.DangerLevel)) return;
            if (Q.IsReady() && MenuValue.Misc.QI)
            {
                Q.Cast(sender);
            }
            else if (W.IsReady() && MenuValue.Misc.WI)
            {
                W.Cast(sender);
            }
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValid) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (player.ManaPercent < MenuValue.LastHit.ManaLimit) return;
            if (args.RemainingHealth <= QDamage(target) && MenuValue.LastHit.UseQ)
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                Q.Cast(target);
            }
            if (args.RemainingHealth <= EDamage(target) && MenuValue.LastHit.UseE)
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                E.Cast(target);
            }
            if (args.RemainingHealth <= RDamage(target) && MenuValue.LastHit.UseR)
            {
                var predHealth = W.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                R.Cast(target);
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        protected static float QDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 60f, 85f, 110f, 135f, 160f }[Q.Level] + 0.4f * player.TotalMagicalDamage);
        }
        protected static float WDamage(Obj_AI_Base target)
        {
            return 0;
        }
        protected static float EDamage(Obj_AI_Base target)
        {
            var raw = new[] { 0f, 50f, 75f, 100f, 125f, 150f }[E.Level] + player.TotalMagicalDamage * 0.5f;
            if (target.HasBuff(IcedName))
            {
                return player.CalculateDamageOnUnit(target, DamageType.Magical, raw * 2);
            }
            else
            {
                return player.CalculateDamageOnUnit(target, DamageType.Magical, raw);
            }
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 40f, 60f, 80f }[R.Level] + 0.125f * player.TotalMagicalDamage);
        }
        #endregion

        protected static float HandleDamageIndicator(Obj_AI_Base target, SpellSlot? slot = null)
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

                public static bool AutoE { get { return Menu.VChecked("UBAddons.Anivia.E.Auto"); } }

                public static bool AutoR { get { return Menu.VChecked("UBAddons.Anivia.R.Auto"); } }

            }

            internal static class Auto
            {
                public static bool RECombo { get { return ComboMenu.VChecked(Variables.AddonName + "." + Player.Instance.Hero + ".RE.Enable"); } }

                public static bool ERCombo { get { return ComboMenu.VChecked(Variables.AddonName + "." + Player.Instance.Hero + ".ER.Enable"); } }

            }
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool OnlyIced { get { return ComboMenu.VChecked("UBAddons.Anivia.E.Only.Iced"); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return HarassMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool OnlyIced { get { return HarassMenu.VChecked("UBAddons.Anivia.E.Only.Iced"); } }

                public static bool UseR { get { return HarassMenu.GetSlotCheckBox(SpellSlot.R); } }


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

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int RHit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.R); } }


                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseE { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode { get { return LasthitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LasthitMenu.PreventCombo(); } }

                public static bool UseQ { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseE { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int ManaLimit { get { return LasthitMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }


                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool QGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] DangerValue { get { return MiscMenu.GetDangerValue(); } }

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
