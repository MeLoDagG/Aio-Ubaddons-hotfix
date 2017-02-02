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

namespace UBAddons.Champions.Ziggs
{
    internal class Ziggs : ChampionPlugin
    {
        //WBuff name is moveawaycollision
        //Ziggs_Base_W_Countdown.troy
        protected static AIHeroClient player = Player.Instance;
        protected const float QBounceRatio2 = 850f / 1125f;
        protected const float QBounceRatio3 = 850f / 1400f;
        //protected static int LastWTick { get; set; }
        //protected static Obj_GeneralParticleEmitter W_Bomb { get; set; }
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Skillshot Q2 { get; set; }
        protected static Spell.Skillshot Q3 { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Skillshot R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LasthitMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Ziggs()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            Q2 = new Spell.Skillshot(SpellSlot.Q, 1125, SkillShotType.Linear, 50 + Q.CastDelay, Q.Speed, Q.Width, DamageType.Magical);
            Q3 = new Spell.Skillshot(SpellSlot.Q, 1400, SkillShotType.Linear, 100 + Q.CastDelay, Q.Speed, Q.Width, DamageType.Magical);
            W = new Spell.Skillshot(SpellSlot.W, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            E = new Spell.Skillshot(SpellSlot.E, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };
            R = new Spell.Skillshot(SpellSlot.R, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            //Obj_GeneralParticleEmitter.OnCreate += delegate (GameObject sender, EventArgs args)
            //{
            //    var Wobj = sender as Obj_GeneralParticleEmitter;
            //    if (Wobj != null && Wobj.Name.Equals("Ziggs_Base_W_Countdown.troy"))
            //    {
            //        W_Bomb = Wobj;
            //    }
            //};
            //Obj_GeneralParticleEmitter.OnDelete += delegate (GameObject sender, EventArgs args)
            //{
            //    var Wobj = sender as Obj_GeneralParticleEmitter;
            //    if (Wobj != null && Wobj.Name.Equals("Ziggs_Base_W_Countdown.troy"))
            //    {
            //        W_Bomb = null;
            //    }
            //};
            //AIHeroClient.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            //{
            //    if (!sender.IsMe) return;
            //    if (args.Slot.Equals(SpellSlot.W))
            //    {
            //        LastWTick = Core.GameTickCount;
            //    }
            //};
        }

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
                Menu.CreatSlotHitChance(SpellSlot.R);
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "UB" + player.Hero + " - Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.CreatSlotHitSlider(SpellSlot.R, 5, 1, 6);
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "UB" + player.Hero + " - Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
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
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.W);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.E);
                    LasthitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "UB" + player.Hero + " - Settings your misc below");
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
        internal static bool CastQ1(Obj_AI_Base target)
        {
            if (target != null && Q.IsReady())
            {
                var pred = Q.GetPrediction(target);
                if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                {
                    return Q.Cast(pred.CastPosition);
                }
            }
            return false;
        }
        internal static bool CastQ2(Obj_AI_Base target)
        {
            if (target != null && Q.IsReady())
            {
                var pred = Q2.GetPrediction(target);
                if (Q.IsInRange(pred.CastPosition))
                {
                    CastQ1(target);
                }
                if (pred.CanNext(Q2, MenuValue.General.QHitChance, false))
                {
                    var castPosition = player.Position.Extend(pred.CastPosition, player.Distance(pred.CastPosition) * QBounceRatio2).To3DWorld();
                    return Q.Cast(castPosition);
                }
            }
            return false;
        }
        internal static bool CastQ3(Obj_AI_Base target)
        {
            if (target != null && Q.IsReady())
            {
                var pred = Q3.GetPrediction(target);
                if (Q.IsInRange(pred.CastPosition))
                {
                    return CastQ1(target);
                }
                if (Q2.IsInRange(pred.CastPosition))
                {
                    return CastQ2(target);
                }
                if (pred.CanNext(Q3, MenuValue.General.QHitChance, false))
                {
                    var castPosition = player.Position.Extend(pred.CastPosition, player.Distance(pred.CastPosition) * QBounceRatio3).To3DWorld();
                    return Q.Cast(castPosition);
                }
            }
            return false;
        }
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid) return;
            if (W.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : W.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.WGap)
            {
                if (W.Cast(player.Position.Extend(args.End, 150).To3DWorld()))
                {
                    Core.DelayAction(() => Player.CastSpell(SpellSlot.W), 250);
                }
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.DangerValue.Contains(args.DangerLevel)) return;
            if (W.IsReady() && MenuValue.Misc.WI && W.IsInRange(sender))
            {
                if (W.Cast(sender))
                {
                    Core.DelayAction(() => Player.CastSpell(SpellSlot.W), W.CastDelay + (int)player.Distance(sender.Position) / W.Speed);
                }
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
                if (predHealth > float.Epsilon)
                {
                    CastQ3(target);
                }
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.W) && MenuValue.LastHit.UseW && W.IsReady())
            {
                var predHealth = W.GetHealthPrediction(target);
                if (predHealth > float.Epsilon && player.IsInAutoAttackRange(target))
                {
                    if (W.Cast(target))
                    {
                        Core.DelayAction(() => Player.CastSpell(SpellSlot.W), W.CastDelay + (int)player.Distance(target.Position) / W.Speed);
                    }
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
        #endregion

        #region Damage

        #region DamageRaw
        protected static float QDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 70f, 120f, 165f, 210f, 255f }[Q.Level] + 0.65f * player.TotalMagicalDamage);
        }
        protected static float WDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 70f, 105f, 140f, 175f, 210f }[W.Level] + 0.35f * player.TotalMagicalDamage);
        }
        protected static float EDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 72f, 117f, 162f, 207f, 252f }[E.Level] + 0.54f * player.TotalMagicalDamage);
        }
        protected static float RDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 200f, 300f, 400f }[R.Level] + 0.733f * player.TotalMagicalDamage);
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

                public static int RHitChance { get { return Menu.GetSlotHitChance(SpellSlot.R); } }

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
                public static bool OnlyFarmMode { get { return LasthitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LasthitMenu.PreventCombo(); } }

                public static bool UseQ { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return LasthitMenu.GetManaLimit(); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool WKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] DangerValue { get { return MiscMenu.GetDangerValue(); } }

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
