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


namespace UBAddons.Champions.Kayle
{
    internal class Kayle : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Targeted Q { get; set; }
        protected static Spell.Targeted W { get; set; }
        protected static Spell.Active E { get; set; }
        protected static Spell.Targeted R { get; set; }

        internal static Menu Menu { get; set; }
        internal static Menu RMenu { get; set; }
        internal static Menu ComboMenu { get; set; }
        internal static Menu HarassMenu { get; set; }
        internal static Menu LaneClearMenu { get; set; }
        internal static Menu JungleClearMenu { get; set; }
        internal static Menu LasthitMenu { get; set; }
        internal static Menu MiscMenu { get; set; }
        internal static Menu DrawMenu { get; set; }

        static Kayle()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 650, DamageType.Magical);

            W = new Spell.Targeted(SpellSlot.W, 900, DamageType.Magical);

            E = new Spell.Active(SpellSlot.E, 625, DamageType.Mixed);

            R = new Spell.Targeted(SpellSlot.R, 900);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            Obj_AI_Base.OnBasicAttack += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                var turret = sender as Obj_AI_Turret;
                if (turret == null || !MenuValue.Auto.Enable || !MenuValue.Auto.EnableWith(player)) return;
                if (args.Target.IsMe && MenuValue.Auto.TurretHit && player.HealthPercent <= 50)
                {
                    R.Cast(player);
                }
            };
            AIHeroClient.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender == null || !sender.IsEnemy || !sender.IsValid || args.Target == null || !R.IsReady() || !MenuValue.Auto.Enable) return;
                var caster = sender as AIHeroClient;
                var geter = args.Target as AIHeroClient;
                if (caster == null || geter == null) return;
                if (caster.IsEnemy && geter.IsAlly)
                {
                    if (MenuValue.Auto.EnableWith(geter) && R.IsInRange(geter))
                    {
                        if (caster.GetSpellDamage(geter, args.Slot) >= MenuValue.Auto.SpellDamage(geter))
                        {
                            R.Cast(geter);
                        }
                    }
                }
            };
        }

        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, Variables.AddonName + ".MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.Add(Variables.AddonName + ".Kayle.W.Enable", new CheckBox("Auto W"));
                Menu.Add(Variables.AddonName + ".Kayle.W.HP", new Slider("When HP", 50));
                Menu.Add(Variables.AddonName + ".Kayle.W.ManaLimit", new Slider("Stop when my MP below", 30));
                foreach (var champ in EntityManager.Heroes.Allies)
                {
                    Menu.Add(Variables.AddonName + ".Kayle.W.Enable." + champ.Hero, new CheckBox("Use W on " + champ.ChampionName, champ.IsMe));
                }
                #endregion

                #region R
                RMenu = Menu.AddSubMenu("R", "UBAddons.RMenu" + player.Hero, "Settings your R saver below");
                {
                    string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".R.";
                    RMenu.Add(BeginText + "Enable", new CheckBox("Use R"));
                    RMenu.AddSeparator();
                    foreach (var champ in EntityManager.Heroes.Allies)
                    {
                        RMenu.AddGroupLabel(champ.ChampionName);
                        RMenu.Add(BeginText + "Enable." + champ.Hero, new CheckBox("Use on " + champ.ChampionName));
                        if (champ.IsMe)
                        {
                            RMenu.Add(Variables.AddonName + ".Kayle.R.Turret", new CheckBox("R when turret hit me"));
                        }
                        RMenu.Add(BeginText + "HP." + champ.Hero, new Slider("Use R if " + champ.ChampionName + "'s HP below {0}%", 20, 0, 100));
                        RMenu.Add(BeginText + "Spell.Damage." + champ.Hero, new Slider("R if enemy spell damage more than", 500, 300, 3000));
                        RMenu.Add(BeginText + "Priority." + champ.Hero, new Slider(champ.ChampionName + "'s priority", CrazyTargetSelector.GetPriority(champ), 1, 5));
                        RMenu.AddSeparator();
                    }
                }
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", Variables.AddonName + ".ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
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
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", Variables.AddonName + ".JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E);
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
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "GapCloser");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser", false);
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
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
            if ((MenuValue.Misc.Idiot ? Ally.Distance(args.End) <= 350 : W.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.WGap)
            {
                if (W.IsReady())
                {
                    W.Cast(Ally);
                }
            }
            if (Q.IsReady() && (MenuValue.Misc.Idiot ? Ally.Distance(args.End) <= 350 : Q.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.QGap)
            {
                Q.Cast(sender);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            return;
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
        #endregion

        #region Damage

        #region DamageRaw
        public static float QDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.Q);
        }
        public static float WDamage(Obj_AI_Base target)
        {
            return 0;
        }
        public static float EDamage(Obj_AI_Base target)
        {
            return 0;
        }
        public static float RDamage(Obj_AI_Base target)
        {
            return 0;
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
                E.DrawRange(MenuValue.Drawings.ColorR);
            }
        }
        #endregion

        #region Menu Value
        protected internal static class MenuValue
        {
            internal static class General
            {
                static string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".W.";

                public static bool Enable { get { return Menu.VChecked(BeginText + "Enable") && player.ManaPercent >= Menu.VSliderValue(BeginText + "ManaLimit"); } }

                public static bool EnableWith(AIHeroClient champ) { return Menu.VChecked(BeginText + "Enable." + champ.Hero); }

                public static int HP { get { return Menu.VSliderValue(BeginText + "HP"); } }
            }

            internal static class Auto
            {
                static string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".R.";

                public static bool Enable { get { return RMenu.VChecked(BeginText + "Enable"); } }

                public static bool TurretHit { get { return RMenu.VChecked("UBAddons.Kayle.R.Turret"); } }

                public static bool EnableWith(AIHeroClient champ) { return RMenu.VChecked(BeginText + "Enable." + champ.Hero); }

                public static int SpellDamage(AIHeroClient champ) { return RMenu.VSliderValue(BeginText + "Spell.Damage." + champ.Hero); }

                public static int HP(AIHeroClient champ) { return RMenu.VSliderValue(BeginText + "HP." + champ.Hero); }

                public static int ChampPriority(AIHeroClient champ) { return CrazyTargetSelector.GetPriority(champ, RMenu); }
            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }
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

                public static int QHit { get { return JungleClearMenu.GetSlotHitSlider(SpellSlot.Q); } }

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

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool QGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.GapCloser.ToString()); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }
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
