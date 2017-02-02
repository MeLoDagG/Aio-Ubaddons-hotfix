using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Log;
using Color = System.Drawing.Color;

namespace UBAddons.Champions.Brand
{
    internal class Brand : ChampionPlugin
    {
        internal static AIHeroClient player = Player.Instance;
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Targeted E { get; set; }
        protected static Spell.Targeted R { get; set; }

        protected const string BrandPassive = "BrandAblaze";
        protected const string BrandDetonate = "BrandAblazeDetonateMarker";
        protected static bool HasPassiveBuff(Obj_AI_Base target)
        {
            return target.HasBuff(BrandPassive) || target.HasBuff(BrandDetonate);
        }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LastHitMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Brand()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = 0,
            };

            W = new Spell.Skillshot(SpellSlot.W, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            E = new Spell.Targeted(SpellSlot.E, 625, DamageType.Magical);

            R = new Spell.Targeted(SpellSlot.R, 750, DamageType.Magical);           

            DamageIndicator.DamageDelegate = HandleDamageIndicator;

            MissileClient.OnCreate += delegate(GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (missile != null && E.IsReady())
                {
                    var target = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget() && x.IsInMissileLine(missile));
                    E.GetTarget(target);
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
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.Add("UBAddons.Brand.Q.Only.Stun", new CheckBox("Only Stun"));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.Add("UBAddons.Brand.W.Only.Extra", new CheckBox("Only Extra damage", false));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.Add("UBAddons.Brand.E.Only.Spread", new CheckBox("Only Spread", false));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.Add("UBAddons.Brand.R.Count.Passive", new Slider("Only has {0} champ passived around", 1, 1, 5));
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.Add("UBAddons.Brand.Q.Only.Stun", new CheckBox("Only Stun", false));
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.Add("UBAddons.Brand.W.Only.Extra", new CheckBox("Only Extra damage", false));
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.Add("UBAddons.Brand.E.Only.Spread", new CheckBox("Only Spread", false));
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.Add("UBAddons.Brand.Q.LaneClear.Around", new Slider("Only Q if around", 3, 1, 10));
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
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LastHitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LastHitMenu.CreatLasthitOpening();
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.W);
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LastHitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "Interrupter");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
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
            if (sender.HasBuff(BrandPassive))
            {
                if (MenuValue.Misc.QGap && Q.IsReady()
                    && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : Q.IsInRange(args.End) || sender.IsAttackingPlayer))
                {
                    Q.Cast(sender);
                }
            }
            else
            {
                if (MenuValue.Misc.WGap && W.IsReady()
                    && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : W.IsInRange(args.End) || sender.IsAttackingPlayer))
                {
                    W.Cast(sender);
                }
                if (MenuValue.Misc.EGap && E.IsReady()
                    && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : E.IsInRange(args.End) || sender.IsAttackingPlayer))
                {
                    E.Cast(sender);
                }
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValidTarget() || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            if (sender.HasBuff(BrandPassive))
            {
                if (Q.IsReady() && MenuValue.Misc.QI && Q.IsInRange(sender))
                {
                    Q.Cast(sender);
                }
            }
            else
            {
                if (W.IsReady() && MenuValue.Misc.WI && W.IsInRange(sender))
                {
                    W.Cast(sender);
                }
                if (E.IsReady() && MenuValue.Misc.EI && E.IsInRange(sender))
                {
                    E.Cast(sender);
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
                if (predHealth < float.Epsilon) return;
                Q.Cast(target);
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.W) && MenuValue.LastHit.UseW && W.IsReady() && W.IsInRange(target))
            {
                var predHealth = W.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                W.Cast(target);
            }
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.E) && MenuValue.LastHit.UseE && E.IsReady() && E.IsInRange(target))
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                E.Cast(target);
            }
        }
        #endregion

        #region Damage

        #region DamageRaw
        public static float PassiveDamage(Obj_AI_Base target)
        {
            if (target.HasBuff(BrandPassive))
            {
                return player.CalculateDamageOnUnit(target, DamageType.Magical, target.MaxHealth * 0.02f);
            }
            if (target.HasBuff(BrandDetonate))
            {
                var percentdetonate = player.Level > 9 ? 16 : player.Level * 0.5f + 11.5f;
                return player.CalculateDamageOnUnit(target, DamageType.Magical, target.MaxHealth * (percentdetonate / 100 + player.TotalMagicalDamage * 0.15f));
            }
            return 0;
        }
        public static float QDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 80f, 110f, 140f, 170f, 200f }[Q.Level] + 0.55f * player.TotalMagicalDamage);
        }
        public static float WDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 75f, 120f, 165f, 210f, 255f }[W.Level] + 0.6f * player.TotalMagicalDamage * (target.HasBuff(BrandPassive) || target.HasBuff(BrandDetonate) ? 1.25f : 1));
        }
        public static float EDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 70f, 90f, 110f, 130f, 150f }[E.Level] + 0.35f * player.TotalMagicalDamage);
        }
        public static float RDamage(Obj_AI_Base target)
        {                
            var raw = new[] { 0f, 100, 200, 300 }[R.Level] + player.TotalMagicalDamage * 0.25f;
            if (target is AIHeroClient)
            {
                var count = ObjectManager.Get<Obj_AI_Base>().Count(x => x.IsValidTarget() && x.Distance(target) < 500 && (x.HasBuff(BrandPassive) || x.HasBuff(BrandDetonate)));
                var count2 = ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsValidTarget() && x.Distance(target) < 500 && !x.HasBuff(BrandPassive) && !x.HasBuff(BrandDetonate));
                switch (count)
                {
                    case 0:
                        {
                            switch (count2.Count())
                            {
                                case 0:
                                    {
                                        return player.CalculateDamageOnUnit(target, DamageType.Magical, raw);
                                    }
                                case 1:
                                    {
                                        var bonus = (int)count2.First().Health / raw;
                                        var bonus2 = bonus > 3 ? 3 : bonus + 1;
                                        return player.CalculateDamageOnUnit(target, DamageType.Magical, raw * bonus2);
                                    }
                                default:
                                    {
                                        return player.CalculateDamageOnUnit(target, DamageType.Magical, raw * 3);
                                    }
                            }
                        }
                    case 1:
                        {
                            switch (count2.Count())
                            {
                                case 0:
                                    {
                                        return player.CalculateDamageOnUnit(target, DamageType.Magical, raw);
                                    }
                                default:
                                    {
                                        return player.CalculateDamageOnUnit(target, DamageType.Magical, raw * 3);
                                    }
                            }
                        }
                    default:
                        {
                            return player.CalculateDamageOnUnit(target, DamageType.Magical, raw * 2);
                        }
                }
            }
            else
            {
                return player.CalculateDamageOnUnit(target, DamageType.Magical, raw);
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
                case SpellSlot.W:
                    return WDamage(target);
                case SpellSlot.E:
                    return EDamage(target);
                case SpellSlot.R:
                    return RDamage(target);
                default:
                    {
                        float damage = PassiveDamage(target);
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

            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool OnlyStun { get { return ComboMenu.VChecked("UBAddons.Brand.Q.Only.Stun"); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool OnlyExtra { get { return ComboMenu.VChecked("UBAddons.Brand.Q.Only.Extra"); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool OnlySpread { get { return ComboMenu.VChecked("UBAddons.Brand.Q.Only.Spread"); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }

                public static int RCount { get { return ComboMenu.VSliderValue("UBAddons.Brand.R.Count.Passive"); } }
            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool OnlyStun { get { return ComboMenu.VChecked("UBAddons.Brand.Q.Only.Stun"); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool OnlyExtra { get { return ComboMenu.VChecked("UBAddons.Brand.Q.Only.Extra"); } }

                public static bool UseE { get { return HarassMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool OnlySpread { get { return ComboMenu.VChecked("UBAddons.Brand.Q.Only.Spread"); } }

                public static int ManaLimit { get { return HarassMenu.GetManaLimit(); } }

                public static bool IsAuto { get { return HarassMenu.GetHarassKeyBind(); } }
            }

            internal static class LaneClear
            {
                public static bool EnableIfNoEnemies { get { return LaneClearMenu.GetNoEnemyOnly(); } }

                public static int ScanRange { get { return LaneClearMenu.GetDetectRange(); } }

                public static bool OnlyKillable { get { return LaneClearMenu.GetKillableOnly(); } }

                public static bool UseQ { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static int Qhit { get { return LaneClearMenu.VSliderValue("UBAddons.Brand.Q.LaneClear.Around"); } }

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int Whit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int Ehit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.E); } }

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

                public static bool QGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] dangerValue { get { return MiscMenu.GetDangerValue(); } }
                public static bool QI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.Interrupter.ToString()); } }

                public static bool WI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.Interrupter.ToString()); } }

                public static bool EI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.Interrupter.ToString()); } }
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
