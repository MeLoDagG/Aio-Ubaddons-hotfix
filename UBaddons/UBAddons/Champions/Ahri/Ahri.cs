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


namespace UBAddons.Champions.Ahri
{
    internal class Ahri : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Obj_AI_Base LastTurretTarget;
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Active W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Skillshot R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu DashMenu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LasthitMenu { get; set; }
        protected static Menu MiscMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        static Ahri()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W, 750, DamageType.Physical);
            E = new Spell.Skillshot(SpellSlot.E, DamageType.Magical);
            R = new Spell.Skillshot(SpellSlot.R, 450, SkillShotType.Circular, 0, 1400, 600, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue
            };

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                var turret = sender as Obj_AI_Turret;
                if (turret == null) return;
                if (turret.Distance(player) > 1000)
                {
                    LastTurretTarget = null;
                }
                else
                {
                    LastTurretTarget = (Obj_AI_Base)args.Target;
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
                Menu.CreatSlotHitChance(SpellSlot.Q);
                Menu.CreatSlotHitChance(SpellSlot.E, 85);
                #endregion

                #region DashMenu
                DashMenu = Menu.AddSubMenu("R", Variables.AddonName + "RMenu" + player.Hero, "Settings your R Logic below");
                {
                    var xxx = Variables.AddonName + "." + player.Hero;
                    DashMenu.CreatSlotComboBox(SpellSlot.R, 0, "To Mouse", "To Side", "To Target");
                    DashMenu.Add(xxx + ".Dash.Dangerous", new CheckBox("Dangerous Check"));
                    DashMenu.Add(xxx + ".Dash.Wall", new CheckBox("Check Wall"));
                    DashMenu.Add(xxx + ".Dash.Correct", new CheckBox("Allow Correct the Direction"));
                    DashMenu.Add(xxx + ".Dash.TryE", new CheckBox("Try R for E"));
                    DashMenu.AddLabel("Try R for E only avaiable for To Side");
                    DashMenu.AddGroupLabel("Dive Turret Calculator");
                    DashMenu.Add(xxx + ".Dash.DiveTurret", new ComboBox("Dive turret logic", 1, "Not dive turret", "Logic dive turret", "Always allow dive turret"));
                    DashMenu.Add(xxx + ".Dash.DiveTurret.MyHP", new Slider("My min HP for dive turret", 50));
                    DashMenu.Add(xxx + ".Dash.DiveTurret.Flash", new CheckBox("Don't dive if target has Flash/ Flee spell"));
                }
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", Variables.AddonName + ".ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", Variables.AddonName + ".HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", Variables.AddonName + ".LaneClear" + player.Hero, "Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.Q, 5, 1, 10);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", Variables.AddonName + ".JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotHitSlider(SpellSlot.Q, 1, 1, 6);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", Variables.AddonName + ".Lasthit" + player.Hero, "Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.W);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
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
                    MiscMenu.CreatSlotCheckBox(SpellSlot.E, "KillSteal");
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

        #region Dash Logic
        protected static class DashLogic
        {
            public static Vector3 DashPos(AIHeroClient target)
            {
                Vector3 pos = new Vector3();
                switch (MenuValue.DashLogic.RLogicValue)
                {
                    case 0:
                        {
                            pos = player.Position.Extend(Game.CursorPos, R.Range).To3DWorld();
                        }
                        break;
                    case 1:
                        {
                            var Path = player.Position.CirclesIntersection(R.Range, target.Position, R.Radius - 100);
                            if (MenuValue.DashLogic.TryE)
                            {
                                var location = Path.OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault(x => VectorHelper.Linear_Collision_Point(x.To3DWorld(), target.Position, E.Range, E.Width, E.Speed, E.CastDelay) == Vector2.Zero);
                                pos = location.To3DWorld();
                            }
                            else
                            {
                                pos = Path.OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault().To3DWorld();
                            }
                        }
                        break;
                    case 2:
                        {
                            pos = player.Position.Extend(target.Position, R.Range).To3DWorld();
                        }
                        break;
                }
                return CheckDangerous(pos, target.BrainIsCharged(), target.IsKillable(SpellSlot.R));
            }
            private static Vector3 CheckDangerous(Vector3 input, bool HasFlash, bool CanKillWithCombo)
            {
                var pos = new Vector3();
                if (MenuValue.DashLogic.CheckDangerous)
                {
                    if (MenuValue.DashLogic.CheckWall)
                    {
                        var Fragment = R.Range / 9;
                        for (var i = 1; i <= 9; i++)
                        {
                            if (player.Position.Extend(input, i * Fragment).IsWall())
                            {
                                if (MenuValue.DashLogic.CorrectVector)
                                {
                                    pos = player.Position.Extend(input, (i - 1) * Fragment).To3DWorld();
                                }
                                else
                                {
                                    pos = new Vector3();
                                }
                                break;
                            }                                
                        }
                    }
                    if (input.IsUnderEnemyTurret())
                    {
                        switch (MenuValue.DashLogic.TurretLogics)
                        {
                            case 0:
                                {
                                    pos = input;
                                }
                                break;
                            case 1:
                                {
                                    if (player.HealthPercent < MenuValue.DashLogic.HPlimit || HasFlash)
                                    {
                                        pos = new Vector3();
                                    }
                                    else
                                    {
                                        if (CanKillWithCombo)
                                        {
                                            pos = input;
                                        }
                                        else
                                        {
                                            pos = new Vector3();
                                        }
                                    }
                                }
                                break;
                            case 2:
                                {
                                    pos = new Vector3();
                                }
                                break;
                        }
                        
                    }
                    if (input.CountEnemyChampionsInRange(1000) > input.CountAllyChampionsInRange(1000) && !CanKillWithCombo)
                    {
                        pos = new Vector3();
                    }
                    else
                    {
                        pos = input;
                    }                   
                }
                else
                {
                    pos = input;
                }
                return pos;
            }
        }
        #endregion

        #region Misc
        protected override void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid) return;
            if (E.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : E.IsInRange(args.End) || sender.IsAttackingPlayer))
            {
                E.Cast(sender);
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            if (E.IsReady() && MenuValue.Misc.EI)
            {
                E.Cast(sender);
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
            else if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.W) && MenuValue.LastHit.UseW && W.IsReady() && W.IsInRange(target))
            {
                var predHealth = W.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                W.Cast();
            }
            else if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.E) && MenuValue.LastHit.UseE && E.IsReady())
            {
                var predHealth = E.GetHealthPrediction(target);
                if (predHealth < float.Epsilon) return;
                E.Cast(target);
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
            return player.GetSpellDamage(target, SpellSlot.E);
        }
        public static float RDamage(Obj_AI_Base target)
        {
            var champ = target as AIHeroClient;
            var raw = player.CalculateDamageOnUnit(target, DamageType.Magical, new[] { 0f, 70f, 110f, 150f }[R.Level] + 0.3f * player.TotalMagicalDamage);
            if (champ != null)
            {
                if (champ.BrainIsCharged())
                {
                    return raw;
                }
                else
                {
                    if (R.ToggleState == 2)
                    {
                        return raw * player.GetBuffCount("AhriTumble");
                    }
                    else
                    {
                        return raw * 3;
                    }
                }
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
        } 
        #endregion

        #region Menu Value
        protected internal static class MenuValue
        {
            internal static class General
            {
                public static int QHitChance { get { return Menu.GetSlotHitChance(SpellSlot.Q); } }

                public static int EHitChance { get { return Menu.GetSlotHitChance(SpellSlot.E); } }
            }

            internal static class DashLogic
            {
                public static string conststring { get { return Variables.AddonName + "." + player.Hero; } }

                public static int RLogicValue { get { return DashMenu.GetSlotComboBox(SpellSlot.R); } }

                public static bool CheckDangerous { get { return DashMenu.VChecked(conststring + ".Dash.Dangerous"); } }

                public static bool CheckWall { get { return DashMenu.VChecked(conststring + ".Dash.Wall"); } }

                public static bool CorrectVector { get { return DashMenu.VChecked(conststring + ".Dash.Correct"); } }

                public static bool TryE { get { return DashMenu.VChecked(conststring + ".Dash.TryE"); } }

                public static int TurretLogics { get { return DashMenu.VComboValue(conststring + ".Dash.DiveTurret"); } }

                public static int HPlimit { get { return DashMenu.VSliderValue(conststring + ".Dash.DiveTurret.MyHP"); } }

                public static bool DenyFlash { get { return DashMenu.VChecked(conststring + "Dash.DiveTurret.Flash"); } }
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

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

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

                public static bool EGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.GapCloser.ToString()); } }

                public static DangerLevel[] dangerValue { get { return MiscMenu.GetDangerValue(); } }

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
