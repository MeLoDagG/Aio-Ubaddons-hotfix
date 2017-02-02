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

namespace UBAddons.Champions.TwistedFate
{
    internal class TwistedFate : ChampionPlugin
    {
        internal static AIHeroClient player = Player.Instance;
        //internal static CardType CardPicking { get; set; }
        //internal static int LastWTick { get; set; }
        //internal static bool IsPicking { get { return player.HasBuff("pickacard_tracker"); } }
        //internal static bool PickACard { get; set; }
        //internal static bool GoldCard { get { return W.Name.Equals("GoldCardLock"); } }
        //internal static bool BlueCard { get { return W.Name.Equals("BlueCardLock"); } }
        //internal static bool RedCard { get { return W.Name.Equals("RedCardLock"); } }
        //internal static bool HasGold { get { return Player.HasBuff("goldcardpreattack"); } }
        protected static Spell.Skillshot Q { get; set; }
        protected static Spell.Active W { get; set; }
        protected static Spell.Skillshot R { get; set; }

        public static CardType CardSeclecting;
        public static int LastWTick;
        public static SelectStatus Status { get; set; }


        internal static Menu Menu { get; set; }
        internal static Menu ComboMenu { get; set; }
        internal static Menu HarassMenu { get; set; }
        internal static Menu LaneClearMenu { get; set; }
        internal static Menu JungleClearMenu { get; set; }
        internal static Menu LastHitMenu { get; set; }
        internal static Menu MiscMenu { get; set; }
        internal static Menu DrawMenu { get; set; }
        
        static TwistedFate()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, DamageType.Magical)
            {
                AllowedCollisionCount = int.MaxValue,
            };

            W = new Spell.Active(SpellSlot.W, (uint)player.AttackRange + 250);

            R = new Spell.Skillshot(SpellSlot.R, 5500, SkillShotType.Circular, 1500, int.MaxValue, (int)player.BoundingRadius + 30, DamageType.Mixed);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;

            Game.OnUpdate += delegate(EventArgs args)
            {
                //Get W name
                var wName = player.Spellbook.GetSpell(SpellSlot.W).Name;
                var wState = player.Spellbook.CanUseSpell(SpellSlot.W);

                //It's Ready
                if ((wState.Equals(SpellState.Ready) && wName.Equals("PickACard") &&
                     (!Status.Equals(SelectStatus.Selecting) || Core.GameTickCount - LastWTick > 500)) || player.IsDead)
                {
                    Status = SelectStatus.Ready;
                }
                //It's cooldown
                else if ((wState.Equals(SpellState.Cooldown) || wState.Equals(SpellState.NoMana) || wState.Equals(SpellState.NotLearned)) && wName.Equals("PickACard"))
                {
                    CardSeclecting = CardType.None;
                    Status = SelectStatus.CannotUse;
                }
                //As E teemo 
                else if (wState.Equals(SpellState.Surpressed) && !player.IsDead)
                {
                    Status = SelectStatus.Selected;
                }
                if (CardSeclecting.Equals(CardType.Blue) && wName.Equals("BlueCardLock") && Core.GameTickCount > LastWTick + 120)
                {
                    player.Spellbook.CastSpell(SpellSlot.W, false);
                }
                else if (CardSeclecting.Equals(CardType.Red) && wName.Equals("RedCardLock") && Core.GameTickCount > LastWTick + 120)
                {
                    player.Spellbook.CastSpell(SpellSlot.W, false);
                }
                else if (CardSeclecting.Equals(CardType.Yellow) && wName.Equals("GoldCardLock") && Core.GameTickCount > LastWTick + 120)
                {
                    player.Spellbook.CastSpell(SpellSlot.W, false);
                }
            };
            AIHeroClient.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (!sender.IsMe) return;
                if (args.SData.Name.Equals("PickACard"))
                {
                    Status = SelectStatus.Selecting;
                }
                if (args.SData.Name.Equals("BlueCardLock") || args.SData.Name.Equals("RedCardLock") || args.SData.Name.Equals("GoldCardLock"))
                {
                    Status = SelectStatus.Selected;
                    CardSeclecting = CardType.None;
                }
                //if (args.SData.Name.Equals("Destiny") && MenuValue.General.RW)
                //{
                //   Core.DelayAction(() => Pick(CardType.Yellow), 1300);
                //}
                if (args.SData.Name.Equals("Gate") && MenuValue.General.RW)
                {
                    Pick(CardType.Yellow);
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
                Menu.AddGroupLabel("Card Selector Key");
                var Bluekey = Menu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".Key.Blue", new KeyBind("Pick Blue", false, KeyBind.BindTypes.HoldActive, 'S'));
                Bluekey.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    if (args.NewValue && W.IsReady())
                    {
                        Pick(CardType.Blue);
                    }
                };
                var Redkey = Menu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".Key.Red", new KeyBind("Pick Red", false, KeyBind.BindTypes.HoldActive, 'E'));
                Redkey.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    if (args.NewValue && W.IsReady())
                    {
                        Pick(CardType.Red);
                    }
                };
                var Goldkey = Menu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".Key.Gold", new KeyBind("Pick Gold", false, KeyBind.BindTypes.HoldActive, 'W'));
                Goldkey.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    if (args.NewValue && W.IsReady())
                    {
                        Pick(CardType.Yellow);
                    }
                };
                Menu.AddGroupLabel("W Smart Setting (For all mode)");
                Menu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".W.Blue.Mana", new Slider("Pick Blue when Mana < {0}%", 20));
                Menu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".W.Red", new Slider("Pick Red when can hit {0} unit", 4, 1, 8));
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".Q.Immobilize", new CheckBox("Only immobilize", false));
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotComboBox(SpellSlot.W, 0, "Smart", "Gold", "Red", "Blue");
                    ComboMenu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".R.AutoPick", new CheckBox("Pick Yellow Card when R"));
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.Add(Variables.AddonName + "." + Player.Instance.Hero + ".Q.Immobilize", new CheckBox("Only immobilize", false));
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotComboBox(SpellSlot.W, 0, "Smart", "Gold", "Red", "Blue");
                    HarassMenu.CreatManaLimit();
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W);
                    LaneClearMenu.CreatSlotComboBox(SpellSlot.W, 0, "Smart", "Gold", "Red", "Blue");
                    LaneClearMenu.CreatManaLimit();
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    JungleClearMenu.CreatSlotComboBox(SpellSlot.W, 0, "Smart", "Gold", "Red", "Blue");
                    JungleClearMenu.CreatManaLimit();
                }
                #endregion

                #region LastHit
                LastHitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LastHitMenu.CreatLasthitOpening();
                    LastHitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LastHitMenu.CreatManaLimit();
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Interrupter settings");
                    MiscMenu.CreatDangerValueBox();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "Interrupter");
                    MiscMenu.AddGroupLabel("Killsteal settings");
                    MiscMenu.CreatSlotCheckBox(SpellSlot.Q, "KillSteal");
                }
                #endregion

                #region Drawings
                DrawMenu = Menu.AddSubMenu("Drawings", "UBAddons.Drawings" + player.Hero, "Settings your drawings below");
                {
                    DrawMenu.CreatDrawingOpening();
                    DrawMenu.CreatColorPicker(SpellSlot.Q);
                    DrawMenu.CreatColorPicker(SpellSlot.R);
                    DrawMenu.CreatColorPicker(SpellSlot.Unknown);
                }
                #endregion
            }
            catch (Exception exception)
            {
                Debug.Print(exception.ToString(), Console_Message.Error);
            }
            DamageIndicator.Initalize(MenuValue.Drawings.ColorDmg);
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
            return;
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsValid || !MenuValue.Misc.dangerValue.Contains(args.DangerLevel)) return;
            Pick(CardType.Yellow);
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
        }
        #endregion

        #region Damage

        #region DamageRaw
        public static float QDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.Default);
        }
        public static float WDamage(Obj_AI_Base target)
        {
            return player.CalculateDamageOnUnit(target, DamageType.Mixed, new float[] { 0, 15, 22.5f, 30, 37.5f, 45f }[W.Level] + player.TotalAttackDamage + 0.5f * player.TotalMagicalDamage);
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
                default:
                    {
                        float damage = 0f;

                        if (Q.IsReady())
                        {
                            damage = damage + QDamage(target);
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
            if (MenuValue.Drawings.DrawR && (!MenuValue.Drawings.ReadyR || R.IsReady()))
            {
                R.DrawRange(MenuValue.Drawings.ColorR);
            }
        }
        #endregion

        internal static void Pick(CardType type)
        {
            if (player.Spellbook.GetSpell(SpellSlot.W).Name.Equals("PickACard") && Status.Equals(SelectStatus.Ready))
            {
                CardSeclecting = type;
                if (Core.GameTickCount - LastWTick > 120 + Game.Ping / 2)
                {
                    W.Cast();
                    LastWTick = Core.GameTickCount;
                }
            }
            if (Status.Equals(SelectStatus.Selecting))
            {
                CardSeclecting = type;
            }
        }
        internal static void LogicPickedCard(bool use, int Logic)
        {
            if (!W.IsReady() || !use) return;
            var WTar = W.GetTarget();
            if (WTar != null) return;
            {
                switch (Logic)
                {
                    case 0:
                        {
                            if (Orbwalker.ActiveModes.Combo.IsOrb() || Orbwalker.ActiveModes.Harass.IsOrb())
                            {
                                if (MenuValue.General.ShouldBlue)
                                {
                                    Pick(CardType.Blue);
                                }
                                else if (WTar.CountEnemyHeroesInRangeWithPrediction(180, 450) >= MenuValue.General.RedHit)
                                {
                                    Pick(CardType.Red);
                                }
                                else
                                {
                                    Pick(CardType.Yellow);
                                }
                            }
                            if (Orbwalker.ActiveModes.LaneClear.IsOrb() || Orbwalker.ActiveModes.JungleClear.IsOrb())
                            {
                                if (MenuValue.General.ShouldBlue)
                                {
                                    Pick(CardType.Blue);
                                }
                                else if (WTar != null && WTar.CountEnemyMinionsInRangeWithPrediction(200, 450) >= MenuValue.General.RedHit)
                                {
                                    Pick(CardType.Red);
                                }
                                else if (EntityManager.MinionsAndMonsters.Monsters.Count(x => x.IsValid && x.IsInRange(player, player.GetAutoAttackRange() + 200)) >= MenuValue.General.RedHit)
                                {
                                    Pick(CardType.Red);
                                }
                                else if (W.GetJungleMobs().Any())
                                {
                                    Pick(CardType.Yellow);
                                }
                            }
                        }
                        break;
                    case 1:
                        {
                            Pick(CardType.Yellow);
                        }
                        break;
                    case 2:
                        {
                            Pick(CardType.Red);
                        }
                        break;
                    case 3:
                        {
                            Pick(CardType.Blue);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #region Menu Value
        protected internal static class MenuValue
        {
            internal static class General
            {
                public static int QHitChance { get { return Menu.GetSlotHitChance(SpellSlot.Q); } }

                public static bool RW { get { return ComboMenu.VChecked(Variables.AddonName + "." + Player.Instance.Hero + ".R.AutoPick"); } }

                public static bool ShouldBlue { get { return Menu.VSliderValue(Variables.AddonName + "." + Player.Instance.Hero + ".W.Blue.Mana") > player.ManaPercent; } }

                public static int RedHit { get { return Menu.VSliderValue(Variables.AddonName + "." + Player.Instance.Hero + ".W.Red"); } }
            }

            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WLogic { get { return ComboMenu.GetSlotComboBox(SpellSlot.W); } }

                public static bool OnlyStun { get { return ComboMenu.VChecked(Variables.AddonName + "." + Player.Instance.Hero + ".Q.Immobilize"); } }
            }

            internal static class Harass
            {
                public static bool UseQ { get { return HarassMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return HarassMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WLogic { get { return HarassMenu.GetSlotComboBox(SpellSlot.W); } }

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

                public static int WLogic { get { return HarassMenu.GetSlotComboBox(SpellSlot.W); } }

                public static int ManaLimit { get { return LaneClearMenu.GetManaLimit(); } }
            }

            internal static class JungleClear
            {

                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static int WLogic { get { return JungleClearMenu.GetSlotComboBox(SpellSlot.W); } }

                public static int ManaLimit { get { return JungleClearMenu.GetManaLimit(); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode = LastHitMenu.OnlyFarmMode();

                public static bool PreventCombo = LastHitMenu.PreventCombo();

                public static bool UseQ = LastHitMenu.GetSlotCheckBox(SpellSlot.Q);

                public static int ManaLimit = LastHitMenu.GetManaLimit();

            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static DangerLevel[] dangerValue { get { return MiscMenu.GetDangerValue(); } }

                public static bool WI { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.Interrupter.ToString()); } }
            }

            internal static class Drawings
            {

                public static bool EnableDraw { get { return DrawMenu.VChecked(Variables.AddonName + "." + player.Hero + ".EnableDraw"); } }

                public static bool DrawQ { get { return DrawMenu.GetDrawCheckValue(SpellSlot.Q); } }

                public static bool ReadyQ { get { return DrawMenu.GetOnlyReady(SpellSlot.Q); } }

                public static SharpDX.Color ColorQ { get { return DrawMenu.GetColorPicker(SpellSlot.Q).ToSharpDX(); } }

                public static bool DrawR { get { return DrawMenu.GetDrawCheckValue(SpellSlot.R); } }

                public static bool ReadyR { get { return DrawMenu.GetOnlyReady(SpellSlot.R); } }

                public static SharpDX.Color ColorR { get { return DrawMenu.GetColorPicker(SpellSlot.R).ToSharpDX(); } }

                public static bool DrawDamageIndicator { get { return DrawMenu.GetDrawCheckValue(SpellSlot.Unknown); } }

                public static Color ColorDmg { get { return DrawMenu.GetColorPicker(SpellSlot.Unknown); } }

            }
        }
        #endregion
        internal enum CardType
        {
            Red,
            Yellow,
            Blue,
            None,
        }
        //Idea from Karma Panda
        internal enum SelectStatus
        {
            Ready,
            Selecting,
            Selected,
            CannotUse,
        }
    }    
}
