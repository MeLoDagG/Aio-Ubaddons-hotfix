using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Linq;
using System.Drawing;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Log;

namespace UBAddons.Champions.Orianna
{
    internal class Orianna : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Active W { get; set; }
        protected static Spell.Targeted E { get; set; }
        protected static Spell.Active R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu LastHitMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Orianna()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
                Range = 800,
            };

            W = new Spell.Active(SpellSlot.W, 250)
            {
                CastDelay = 250,
            };

            E = new Spell.Targeted(SpellSlot.E, 1100);

            R = new Spell.Active(SpellSlot.R, 400)
            {
                CastDelay = 450,
            };

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            Obj_AI_Base.OnBasicAttack += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsAlly || !!args.Target.IsAlly || !(args.Target is AIHeroClient) || !E.IsReady()) return;
                {
                    var target = args.Target as AIHeroClient;
                    if ((sender is AIHeroClient || sender is Obj_AI_Turret) && target != null)
                    {
                        E.Cast(target);
                    }
                }
            };
            Game.OnUpdate += delegate (EventArgs args)
            {
                var ball = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.BaseSkinName.Equals("OriannaBall") && x.IsAlly && x.IsValid && !x.IsDead).FirstOrDefault();
                if (ball != null)
                {
                    Q.SourcePosition = ball.Position;
                    W.RangeCheckSource = ball.Position;
                    R.RangeCheckSource = ball.Position;
                }
                else
                {
                    var Qmissile = ObjectManager.Get<MissileClient>().Where(x => x.SpellCaster.IsMe && x.IsValid && !x.IsDead && x.SData.Name.Equals("OrianasQ")).FirstOrDefault();
                    if (Qmissile != null)
                    {
                        Q.SourcePosition = Qmissile.EndPosition;
                        W.RangeCheckSource = Qmissile.EndPosition;
                        R.RangeCheckSource = Qmissile.EndPosition;
                    }
                    else
                    {
                        if (player.HasBuff("orianaghostself"))
                        {
                            Q.SourcePosition = player.Position;
                            W.RangeCheckSource = player.Position;
                            R.RangeCheckSource = player.Position;
                        }
                        else
                        {
                            var champ = EntityManager.Heroes.Allies.Where(x =>  x.IsValidTarget() && x.GetBuff("orianaghost") != null && x.GetBuff("orianaghost").Caster.IsMe).FirstOrDefault();
                            if (champ != null)
                            {
                                Q.SourcePosition = champ.Position;
                                W.RangeCheckSource = champ.Position;
                                R.RangeCheckSource = champ.Position;
                            }
                        }
                    }
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
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.CreatSlotHitSlider(SpellSlot.R, 3, 1, 6);
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
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "Interrupter");
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
        protected static bool CastE(Obj_AI_Base target)
        {
            if (target != null && E.IsReady())
            {
                var CurrentBallPos = Q.SourcePosition;
                foreach (var ally in EntityManager.Heroes.Allies.Where(x => x.IsValidTarget(E.Range) && x.Distance(CurrentBallPos.Value) > 30))
                {
                    Geometry.Polygon.Rectangle rectangle = new Geometry.Polygon.Rectangle(CurrentBallPos.Value, ally.Position, Q.Radius / 4);
                    if (rectangle.IsInside(Prediction.Position.PredictUnitPosition(target, 350)))
                    {
                        if (E.Cast(ally))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsValidTarget() || !sender.IsEnemy) return;
            if (E.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : E.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.EGap)
            {
                E.Cast(player);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.DangerValue.Contains(args.DangerLevel)) return;
            if (R.IsReady() && MenuValue.Misc.RI)
            {
                if (R.IsInRange(sender))
                {
                    R.Cast();
                }
                else
                {
                    if (Q.IsInRange(sender) && Q.IsReady())
                    {
                        Q.Cast(sender);
                    }
                }
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
                    W.Cast();
                }
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.E) && MenuValue.LastHit.UseE && E.IsReady() && E.IsInRange(target))
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth > 0)
                {
                    CastE(target);
                }
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        protected static float QDamage(Obj_AI_Base target)
        {
            var raw = new[] { 0f, 60f, 90f, 120f, 150f, 180f }[Q.Level] + 0.5f * Player.Instance.TotalMagicalDamage;
            var pred = Q.GetPrediction(target);
            int collisionCount = pred.CollisionObjects.Length > 6 ? 6 : pred.CollisionObjects.Length;
            return player.CalculateDamageOnUnit(target, DamageType.Magical, raw * (1f - collisionCount * 0.1f)); 
        }

        protected static float WDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 70f, 115f, 160f, 205f, 250f }[W.Level] + 0.7f * Player.Instance.TotalMagicalDamage);
        }

        protected static float EDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 60f, 90f, 120f, 150f, 180f }[E.Level] + 0.3f * Player.Instance.TotalMagicalDamage);
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 150f, 225f, 350f }[R.Level] + 0.7f * Player.Instance.TotalMagicalDamage);
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
            }
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int RHit { get { return ComboMenu.GetSlotHitSlider(SpellSlot.R); } }

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
                public static bool OnlyFarmMode { get { return LastHitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LastHitMenu.PreventCombo(); } }

                public static bool UseQ { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return LastHitMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return LastHitMenu.GetManaLimit(); } }
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

                public static Color ColorDmg { get { return DrawMenu.GetColorPicker(SpellSlot.Unknown); } }

            }
        }
        #endregion
    }
}
