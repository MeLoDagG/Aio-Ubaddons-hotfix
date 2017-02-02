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
using UBAddons.Libs.Dictionary;
using Color = System.Drawing.Color;


namespace UBAddons.Champions.Veigar
{
    internal class Veigar : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Skillshot W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Targeted R { get; set; }

        internal static Menu Menu { get; set; }
        internal static Menu ComboMenu { get; set; }
        internal static Menu HarassMenu { get; set; }
        internal static Menu LaneClearMenu { get; set; }
        internal static Menu JungleClearMenu { get; set; }
        internal static Menu LasthitMenu { get; set; }
        internal static Menu MiscMenu { get; set; }
        internal static Menu DrawMenu { get; set; }

        static Veigar()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = 1,
            };

            W = new Spell.Skillshot(SpellSlot.W, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            E = new Spell.Skillshot(SpellSlot.E, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            R = new Spell.Targeted(SpellSlot.R, 650);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
        }

        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, Variables.AddonName + ".MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.Q);
                Menu.CreatSlotHitChance(SpellSlot.W);
                Menu.CreatSlotHitChance(SpellSlot.E);
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", Variables.AddonName + ".ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotComboBox(SpellSlot.W, 1, "Usually", "Only stun");
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotHitSlider(SpellSlot.E, 3, 1, 5);
                    ComboMenu.AddLabel("Normal will try to stun target");
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", Variables.AddonName + ".HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", Variables.AddonName + ".LaneClear" + player.Hero, "Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.Add("UBAddons.Veigar.Farm.Enable",new CheckBox("Enable AP Farm"));
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.W, 5, 1, 10);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", Variables.AddonName + ".JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", Variables.AddonName + ".Lasthit" + player.Hero, "Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LasthitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", Variables.AddonName + ".Misc" + player.Hero, "Settings your misc below");
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
                    MiscMenu.CreatSlotCheckBox(SpellSlot.R, "KillSteal");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings", Variables.AddonName + ".Drawing" + player.Hero, "Setting your Drawings below");
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
                Log.Debug.Print(exception.ToString(), Console_Message.Error);
            }
        }

        #region Misc
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid) return;
            var Ally = EntityManager.Heroes.Allies.Where(x => x.IsValid && !x.IsDead && !x.IsZombie).OrderBy(x => x.Distance(args.End)).First();
            if ((MenuValue.Misc.Idiot ? Ally.Distance(args.End) <= 350 : E.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.EGap)
            {
                if (E.IsReady())
                {
                    CastE(sender);
                }
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            if (E.IsReady() && MenuValue.Misc.EI && E.IsInRange(sender))
            {
                CastE(sender);
            }
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValid) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (player.ManaPercent < MenuValue.LastHit.ManaLimit) return;
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.Q) && MenuValue.LastHit.UseQ && Q.IsReady() && Q.IsInRange(target))
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                Q.Cast(target);
            }
        }
        protected static bool CastE(Obj_AI_Base target)
        {
            if (target == null)
            {
                return false;
            }
            else
            {
                var pred = E.GetPrediction(target);
                if (pred.CanNext(E, MenuValue.General.EHitChance, true) && pred.UnitPosition.IsInRange(Player.Instance, E.Range + E.Radius) && E.IsReady())
                {
                    var Location = Player.Instance.Position.Extend(pred.UnitPosition, Player.Instance.Distance(pred.UnitPosition) - E.Radius + 50).To3DWorld();
                    if (E.Cast(Location))
                    {
                        return true;
                    }
                }
                return false;
            }
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
                            damage = damage += player.GetAutoAttackDamage(target, true);
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
            if (MenuValue.Drawings.DrawW && (MenuValue.Drawings.ReadyW) || W.IsReady())
            {
                W.DrawRange(MenuValue.Drawings.ColorW);
            }
            if (MenuValue.Drawings.DrawE && (MenuValue.Drawings.ReadyE) || E.IsReady())
            {
                E.DrawRange(MenuValue.Drawings.ColorE);
            }
            if (MenuValue.Drawings.DrawR && (MenuValue.Drawings.ReadyR) || R.IsReady())
            {
                R.DrawRange(MenuValue.Drawings.ColorR);
            }
            if (Orbwalker.DrawLastHitMarker)
            {
                foreach (var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.VisibleOnScreen && x.IsValidTarget() && x.IsKillable(SpellSlot.Q)))
                {
                    minion.DrawCircle(50, MenuValue.Drawings.ColorQ, 4);
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

                public static int WHitChance { get { return Menu.GetSlotHitChance(SpellSlot.W); } }

                public static int EHitChance { get { return Menu.GetSlotHitChance(SpellSlot.E); } }

            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WLogic { get { return ComboMenu.GetSlotComboBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int EHit { get { return ComboMenu.GetSlotHitSlider(SpellSlot.E); } }

            }

            internal static class Harass
            {
                public static bool IsAuto { get { return HarassMenu.GetHarassKeyBind(); } }

                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return HarassMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int ManaLimit { get { return HarassMenu.GetManaLimit(); } }
            }

            internal static class LaneClear
            {
                public static bool APEnable { get { return LaneClearMenu.VChecked("UBAddons.Veigar.Farm.Enable"); } }

                public static bool EnableIfNoEnemies { get { return LaneClearMenu.GetNoEnemyOnly(); } }

                public static int ScanRange { get { return LaneClearMenu.GetDetectRange(); } }

                public static bool OnlyKillable { get { return LaneClearMenu.GetKillableOnly(); } }

                public static bool UseQ { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WHit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.W); } }

                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {
                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

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

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] dangerValue { get { return MiscMenu.GetDangerValue(); } }

                public static bool EI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.Interrupter.ToString()); } }
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
