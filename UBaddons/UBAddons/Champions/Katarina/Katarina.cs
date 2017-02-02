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
using UBAddons.Libs.ColorPicker;
using UBAddons.Log;
using Color = System.Drawing.Color;

namespace UBAddons.Champions.Katarina
{
    internal class Katarina : ChampionPlugin
    {
        protected static AIHeroClient player = Player.Instance;
        protected static Spell.Targeted Q { get; set; }
        protected static Spell.Active W { get; set; }
        protected static Spell.Skillshot E { get; set; }
        protected static Spell.Active R { get; set; }

        protected static Menu Menu { get; set; }
        protected static Menu ComboMenu { get; set; }
        protected static Menu HarassMenu { get; set; }
        protected static Menu LaneClearMenu { get; set; }
        protected static Menu JungleClearMenu { get; set; }
        protected static Menu LasthitMenu { get; set; }
        protected static Menu FleeMenu { get; set; }

        protected static Menu MiscMenu { get; set; }
        protected static Menu DrawMenu { get; set; }

        protected static Dictionary<Obj_GeneralParticleEmitter, Tuple<bool, float, bool>> Dagger = new Dictionary<Obj_GeneralParticleEmitter, Tuple<bool, float, bool>>();
        protected static bool KilledAnEnemy;
        protected static float LastUpdate;
        static Katarina()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 625, DamageType.Magical);
            W = new Spell.Active(SpellSlot.W, 370); //Real is 340
            E = new Spell.Skillshot(SpellSlot.E, 725, SkillShotType.Circular, 50, int.MaxValue, 100, DamageType.Magical);
            R = new Spell.Active(SpellSlot.R, 550, DamageType.Magical);

            DamageIndicator.DamageDelegate = HandleDamageIndicator;
            AIHeroClient.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe && args.Slot == SpellSlot.E)
                {
                    Orbwalker.ResetAutoAttack();
                }
            };
            Game.OnNotify += delegate(GameNotifyEventArgs args)
            {
                if (args.EventId.Equals(GameEventId.OnDeathAssist) || args.EventId.Equals(GameEventId.OnChampionKill))
                {
                    if (ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId) != null && ObjectManager.GetUnitByNetworkId<AIHeroClient>(args.NetworkId).IsMe)
                    {
                        KilledAnEnemy = true;
                        LastUpdate = Game.Time;
                    }
                }
            };
            Game.OnUpdate += delegate(EventArgs args)
            {
                if (KilledAnEnemy && LastUpdate + 0.7f > Game.Time)
                {
                    KilledAnEnemy = false;
                }

            };
            Obj_GeneralParticleEmitter.OnCreate += delegate(GameObject sender, EventArgs args)
            {
                var dagger = sender as Obj_GeneralParticleEmitter;
                if (dagger != null)
                {
                    if (dagger.Name.Equals("Katarina_Base_W_Indicator_Ally.troy"))
                    {
                        var time = dagger.Distance(player) < 350 ? 1.25f : 1;
                        Dagger.Add(dagger, new Tuple<bool, float, bool>(false, Game.Time + time, dagger.Distance(player) < 350));
                    }
                    if (dagger.Name.Equals("Katarina_Base_Q_Dagger_Land_Dirt.troy")
                        || dagger.Name.Equals("Katarina_Base_Q_Dagger_Land_Water.troy")
                        || dagger.Name.Equals("Katarina_Base_Q_Dagger_Land_Stone.troy"))
                    {
                        var currentdagger = BaseDagger(dagger);
                        if (Dagger.ContainsKey(currentdagger))
                        {
                            Dagger[currentdagger] = new Tuple<bool, float, bool>(true, Game.Time + 4.2f, false);
                        }
                    }
                }
            };
            Obj_GeneralParticleEmitter.OnDelete += delegate(GameObject sender, EventArgs args)
            {
                var dagger = sender as Obj_GeneralParticleEmitter;
                if (dagger != null)
                {
                    if (dagger.Name.Equals("Katarina_Base_W_Indicator_Ally.troy"))
                    {
                        Dagger.Remove(dagger);
                    }
                }
            };
        }
        internal static Obj_GeneralParticleEmitter BaseDagger(Obj_GeneralParticleEmitter sender)
        {
            return ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(x => x.Position.Distance(sender.Position) < 20 && x.IsValid && x != sender
                && x.Name.Equals("Katarina_Base_W_Indicator_Ally.troy")).FirstOrDefault();
        }
        internal static E_Status UpdateEStatus(Obj_GeneralParticleEmitter dagger ,out float timerefund)
        {
            float percentrefund = player.Level >= 16 ? 0.96f :                        
                player.Level >= 11 ? 0.9f :                         
                player.Level >= 6 ? 0.84f : 0.78f;
            float Ebasic = new float[] { 0f, 10f, 9.5f, 9f, 8.5f, 8f }[E.Level];
            float CurrentECooldown = Ebasic * (1 - Math.Abs(player.PercentCooldownMod / 100));
            if (!Dagger.ContainsKey(dagger))
            {
                timerefund = -1;
                return E_Status.NoDagger;
            }
            else
            {
                if (!Dagger[dagger].Item1)
                {
                    timerefund = -1;
                    return E_Status.StillAir;
                }
                if (E.IsReady())
                {
                    timerefund = CurrentECooldown * (1 - percentrefund);
                    return E_Status.EReady;
                }
                else
                {
                    if (!Dagger.ContainsKey(dagger))
                    {
                        Chat.Print("Has error with calculate E status. Report me");
                        timerefund = -1;
                        return E_Status.NoDagger;
                    }
                    if (player.Distance(dagger) - 150 < player.MoveSpeed * (Dagger[dagger].Item2 - Game.Time))
                    {
                        var time = (E.Handle.CooldownExpires - Game.Time) - CurrentECooldown * percentrefund;
                        timerefund = time > 0 ? time : 0;
                        return E_Status.CanWalk;
                    }
                    else
                    {
                        timerefund = -1;
                        return E_Status.CannotWalk;
                    }
                }
            }
        }
        protected override void CreateMenu()
        {
            try
            {
                #region Mainmenu
                Menu = MainMenu.AddMenu("UB" + player.Hero, "UBAddons.MainMenu" + player.Hero, "UB" + player.Hero + " - UBAddons - by U.Boruto");
                Menu.AddGroupLabel("General Setting");
                Menu.CreatSlotHitChance(SpellSlot.E);
                Menu.CreatSlotHitChance(SpellSlot.R);
                #endregion

                #region Combo
                ComboMenu = Menu.AddSubMenu("Combo", "UBAddons.ComboMenu" + player.Hero, "UB" + player.Hero + " - Settings your combo below");
                {
                    ComboMenu.CreatSlotCheckBox(SpellSlot.Q);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.W);
                    ComboMenu.CreatSlotComboBox(SpellSlot.W, 1, "Safe", "Damage");
                    ComboMenu.CreatSlotCheckBox(SpellSlot.E);
                    ComboMenu.CreatSlotCheckBox(SpellSlot.R);
                    ComboMenu.AddGroupLabel("E Setting");
                    ComboMenu.Add("UBAddons.Katarina.E.Turret.Disabble", new CheckBox("Prevent E to turret"));
                    ComboMenu.Add("UBAddons.Katarina.E.Killable.Only", new CheckBox("Only E if killable"));
                    ComboMenu.Add("UBAddons.Katarina.E.Killable.Anyway", new CheckBox("E anyway if killable"));
                    ComboMenu.Add("UBAddons.Katarina.E.To.Minion", new CheckBox("E To Minion", false));
                    ComboMenu.Add("UBAddons.Katarina.E.To.Champ.Enemy", new CheckBox("E To Enemy Champ"));
                    ComboMenu.Add("UBAddons.Katarina.E.To.Champ.Ally", new CheckBox("E To Ally Champ"));
                    ComboMenu.Add("UBAddons.Katarina.E.To.Dagger", new CheckBox("E To Dagger"));
                    ComboMenu.Add("UBAddons.Katarina.E.Kill", new ComboBox("When one enemy killed, I want E to", 0, "Smart", "Unit near Cursor", "Next Target"));
                }
                #endregion

                #region Harass
                HarassMenu = Menu.AddSubMenu("Harass", "UBAddons.HarassMenu" + player.Hero, "UB" + player.Hero + " - Settings your harass below");
                {
                    HarassMenu.CreatSlotCheckBox(SpellSlot.Q);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.W);
                    HarassMenu.CreatSlotCheckBox(SpellSlot.E);
                    HarassMenu.CreatHarassKeyBind();
                }
                #endregion

                #region LaneClear
                LaneClearMenu = Menu.AddSubMenu("LaneClear", "UBAddons.LaneClear" + player.Hero, "UB" + player.Hero + " - Settings your laneclear below");
                {
                    LaneClearMenu.CreatLaneClearOpening();
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.W, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.W, 5, 1, 10);
                    LaneClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                    LaneClearMenu.CreatSlotHitSlider(SpellSlot.E, 5, 1, 10);
                }
                #endregion

                #region JungleClear
                JungleClearMenu = Menu.AddSubMenu("JungleClear", "UBAddons.JungleClear" + player.Hero, "UB" + player.Hero + " - Settings your jungleclear below");
                {
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.Q, null, false);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.W);
                    JungleClearMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                }
                #endregion

                #region Lasthit
                LasthitMenu = Menu.AddSubMenu("Lasthit", "UBAddons.Lasthit" + player.Hero, "UB" + player.Hero + " - Settings your unkillable minion below");
                {
                    LasthitMenu.CreatLasthitOpening();
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.Q);
                    LasthitMenu.CreatSlotCheckBox(SpellSlot.E, null, false);
                }
                #endregion

                #region Flee
                FleeMenu = Menu.AddSubMenu("Flee", "UBAddons.Flee" + player.Hero, "Setting your flee below");
                {
                    string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".";
                    FleeMenu.Add(BeginText + "E", new CheckBox("Use E for flee"));
                    FleeMenu.Add(BeginText + "E.ToDagger", new CheckBox("E to dagger"));
                    FleeMenu.Add(BeginText + "E.ToMinion", new CheckBox("E to minion"));
                    FleeMenu.Add(BeginText + "E.ToMonster", new CheckBox("E to monster"));
                    FleeMenu.Add(BeginText + "E.ToChamp", new CheckBox("E to champ"));
                    FleeMenu.Add(BeginText + "E.HP", new Slider("Min {0}% HP for E champ & monster", 15));
                }
                #endregion

                #region Misc
                MiscMenu = Menu.AddSubMenu("Misc", "UBAddons.Misc" + player.Hero, "UB" + player.Hero + " - Settings your misc below");
                {
                    MiscMenu.AddGroupLabel("Anti Gapcloser settings");
                    MiscMenu.CreatMiscGapCloser();
                    MiscMenu.CreatSlotCheckBox(SpellSlot.W, "GapCloser");
                    MiscMenu.AddGroupLabel("Interrupter settings");
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
                    DrawMenu.Add("UBAddons.Katarina.Dagger.Draw", new ComboBox("Draw Time", 0, "Disable", "Circular", "Line", "Number"));
                    DrawMenu.Add("UBAddons.Katarina.Draw.Status", new CheckBox("Draw status"));
                    DrawMenu.Add("UBAddons.Katarina.Dagger.ColorPicker", new ColorPicker("Dagger Color", ColorReader.Load("UBAddons.Katarina.Dagger.ColorPicker", Color.GreenYellow)));
                    DrawMenu.CreatColorPicker(SpellSlot.Q);
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
            if (W.IsReady() && (MenuValue.Misc.Idiot ? player.Distance(args.End) <= 250 : W.IsInRange(args.End) || sender.IsAttackingPlayer) && MenuValue.Misc.WGap)
            {
                W.Cast();
            }
        }

        protected override void OnInterruptable(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            return;
        }

        protected override void OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsInvulnerable || !target.IsValidTarget()) return;
            if (MenuValue.LastHit.PreventCombo && Orbwalker.ActiveModes.Combo.IsOrb()) return;
            if (MenuValue.LastHit.OnlyFarmMode && !Variables.IsFarm) return;
            if (args.RemainingHealth <= DamageIndicator.DamageDelegate(target, SpellSlot.Q) && MenuValue.LastHit.UseQ && Q.IsReady())
            {
                var predHealth = Q.GetHealthPrediction(target);
                if (predHealth > float.Epsilon)
                {
                    Q.Cast(target);
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
        protected static float PassiveDamage(Obj_AI_Base target)
        {
            if (Dagger.Count <= float.Epsilon)
            {
                return 0;
            }
            else
            {
                if (Dagger.Keys.Any(x => x.Distance(target) <= W.Range))
                {
                    float raw = new float[] { 0, 75f, 80f, 87f, 94f, 102f, 111f, 120f, 131f, 143f, 155f, 168f, 183f, 198f, 214f, 231f, 248f, 267f, 287f }[player.Level];
                    int levelratio = player.Level >= 16 ? 3 :
                        player.Level >= 11 ? 2 : 
                        player.Level >= 6 ? 1 : 0;
                    var APratio = new float[] { 0f, 0.55f, 0.70f, 0.85f, 1f }[levelratio];
                    return player.CalculateDamageOnUnit(target, DamageType.Magical, raw + player.FlatPhysicalDamageMod + player.TotalMagicalDamage * APratio);
                }
                else
                {
                    return 0;
                }
            }
        }
        protected static float QDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.Default);
        }
        protected static float WDamage(Obj_AI_Base target)
        {
            return 0;
        }
        protected static float EDamage(Obj_AI_Base target)
        {
            return player.GetSpellDamage(target, SpellSlot.E, DamageLibrary.SpellStages.Default);
        }
        protected static float RDamage(Obj_AI_Base target, float basictime = 1)
        {
            var raw = new float[] { 0, 375, 562.5f, 750 }[R.Level];

            if (R.IsInRange(target))
            {
                var timeToOut = (R.Range - player.Distance(target)) / target.MoveSpeed + target.GetMovementBlockedDebuffDuration();
                var percentRHit = timeToOut / 2.5f >= 1 ? 1 : timeToOut / 2.5f;
                return player.CalculateDamageOnUnit(target, DamageType.Magical, (raw + 3.3f * player.FlatPhysicalDamageMod + 2.85f * player.TotalMagicalDamage) * percentRHit);
            }
            else
            {
                if (E.IsReady())
                {
                    basictime = Math.Abs(basictime) >= 2.5f ? 2.5f : Math.Abs(basictime);
                    return player.CalculateDamageOnUnit(target, DamageType.Magical, (raw + 3.3f * player.FlatPhysicalDamageMod + 2.85f * player.TotalMagicalDamage) * basictime);
                }
                else
                {
                    return 0;
                }
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
                        damage = damage + PassiveDamage(target);
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
            if (MenuValue.Drawings.DrawE && (!MenuValue.Drawings.ReadyE || E.IsReady()))
            {
                E.DrawRange(MenuValue.Drawings.ColorE);
            }
            if (MenuValue.Drawings.DrawR && (!MenuValue.Drawings.ReadyR || R.IsReady()))
            {
                R.DrawRange(MenuValue.Drawings.ColorR);
            }
            if (MenuValue.Drawings.DrawDagger != 0 && Dagger.Keys != null)
            {
                foreach (var dagger in Dagger.Keys)
                {
                    float thuong = Dagger[dagger].Item1 ? 4 : Dagger[dagger].Item3 ? 1.25f : 1f;
                    float percent = Dagger[dagger].Item2 - Game.Time > 0 ? -(Math.Abs(Dagger[dagger].Item2 - Game.Time) / thuong * 100) : 0;
                    switch (MenuValue.Drawings.DrawDagger)
                    {
                        case 1:
                            {
                                UBDrawings.DrawCircularTimer(dagger.Position, 150, MenuValue.Drawings.ColorDagger, 0, percent);
                            }
                            break;
                        case 2:
                            {
                                UBDrawings.DrawLinearTimer(dagger.Position.WorldToScreen(), percent, MenuValue.Drawings.ColorDagger);
                            }
                            break;
                        case 3:
                            {
                                var time = Dagger[dagger].Item2 - Game.Time > 0 ? Dagger[dagger].Item2 - Game.Time : 0;
                                UBDrawings.DrawTimer(dagger.Position.WorldToScreen(), time, MenuValue.Drawings.ColorDagger);
                            }
                            break;
                    }
                    if (MenuValue.Drawings.DrawStatus)
                    {
                        float time = new float();
                        switch (UpdateEStatus(dagger, out time))
                        {
                            case E_Status.CannotWalk:
                                {
                                    var pos = new Vector2(dagger.Position.WorldToScreen().X - 48, dagger.Position.WorldToScreen().Y);
                                    Drawing.DrawText(pos, Color.Red, "You can't pick it", 12);
                                }
                                break;
                            case E_Status.CanWalk:
                                {
                                    var pos = new Vector2(dagger.Position.WorldToScreen().X - 75, dagger.Position.WorldToScreen().Y);
                                    Drawing.DrawText(pos, Color.Yellow, "Your E will : " + time.ToString("N2") + " if pick", 12);
                                }
                                break;
                            case E_Status.EReady:
                                {
                                    var pos = new Vector2(dagger.Position.WorldToScreen().X - 75, dagger.Position.WorldToScreen().Y);
                                    Drawing.DrawText(pos, Color.Green, "Your E will : " + time.ToString("N2") + " if pick", 12);
                                }
                                break;
                            case E_Status.StillAir:
                                {
                                    var pos = new Vector2(dagger.Position.WorldToScreen().X - 28, dagger.Position.WorldToScreen().Y);
                                    Drawing.DrawText(pos, Color.Cyan, "It's airbone", 12);
                                }
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Menu Value
        protected internal static class MenuValue
        {
            internal static class Combo
            {
                public static bool UseQ { get { return ComboMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return ComboMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool Safe { get { return ComboMenu.GetSlotComboBox(SpellSlot.W) < float.Epsilon; } }

                public static bool UseE { get { return ComboMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static bool DontETurret { get { return ComboMenu.VChecked("UBAddons.Katarina.E.Turret.Disabble"); } }

                public static bool EKillAbleOnly { get { return ComboMenu.VChecked("UBAddons.Katarina.E.Killable.Only"); } }

                public static bool EAnyway { get { return ComboMenu.VChecked("UBAddons.Katarina.E.Killable.Anyway"); } }

                public static bool EToMinion { get { return ComboMenu.VChecked("UBAddons.Katarina.E.To.Minion"); } }

                public static bool EToAllyChamp { get { return ComboMenu.VChecked("UBAddons.Katarina.E.To.Champ.Ally"); } }

                public static bool EToEnemyChamp { get { return ComboMenu.VChecked("UBAddons.Katarina.E.To.Champ.Enemy"); } }

                public static bool EToDagger { get { return ComboMenu.VChecked("UBAddons.Katarina.E.To.Dagger"); } }

                public static int ENextLogic { get { return ComboMenu.VComboValue("UBAddons.Katarina.E.Kill"); } }

                public static bool UseR { get { return ComboMenu.GetSlotCheckBox(SpellSlot.R); } }
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

                public static bool UseW { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return LaneClearMenu.GetSlotCheckBox(SpellSlot.E); } }

                public static int Ehit { get { return LaneClearMenu.GetSlotHitSlider(SpellSlot.E); } }
            }

            internal static class JungleClear
            {
                public static bool UseQ { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseW { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.W); } }

                public static bool UseE { get { return JungleClearMenu.GetSlotCheckBox(SpellSlot.E); } }
            }

            internal static class LastHit
            {
                public static bool OnlyFarmMode { get { return LasthitMenu.OnlyFarmMode(); } }

                public static bool PreventCombo { get { return LasthitMenu.PreventCombo(); } }

                public static bool UseQ { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.Q); } }

                public static bool UseE { get { return LasthitMenu.GetSlotCheckBox(SpellSlot.E); } }
            }
            internal static class Flee
            {
                private static string BeginText = Variables.AddonName + "." + Player.Instance.Hero + ".";

                public static bool UseE { get { return FleeMenu.VChecked(BeginText + "E"); } }

                public static bool EDagger { get { return FleeMenu.VChecked(BeginText + "E.ToDagger"); } }

                public static bool EMinion { get { return FleeMenu.VChecked(BeginText + "E.ToMinion"); } }

                public static bool EMonster { get { return FleeMenu.VChecked(BeginText + "E.ToMonster"); } }

                public static bool EChamp { get { return FleeMenu.VChecked(BeginText + "E.ToChamp"); } }

                public static int HP { get { return FleeMenu.VSliderValue(BeginText + "E.HP"); } }
            }

            internal static class Misc
            {
                public static bool QKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.Q, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool EKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.E, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool RKS { get { return MiscMenu.GetSlotCheckBox(SpellSlot.R, Misc_Menu_Value.KillSteal.ToString()); } }

                public static bool Idiot { get { return MiscMenu.PreventIdiotAntiGap(); } }

                public static bool WGap { get { return MiscMenu.GetSlotCheckBox(SpellSlot.W, Misc_Menu_Value.GapCloser.ToString()); } }
            }

            internal static class Drawings
            {

                public static bool EnableDraw { get { return DrawMenu.VChecked(Variables.AddonName + "." + player.Hero + ".EnableDraw"); } }

                public static int DrawDagger { get { return DrawMenu.VComboValue("UBAddons.Katarina.Dagger.Draw"); } }

                public static bool DrawStatus { get { return DrawMenu.VChecked("UBAddons.Katarina.Draw.Status"); } }

                public static Color ColorDagger { get { return DrawMenu["UBAddons.Katarina.Dagger.ColorPicker"].Cast<ColorPicker>().CurrentValue; } }


                public static bool DrawQ { get { return DrawMenu.GetDrawCheckValue(SpellSlot.Q); } }

                public static bool ReadyQ { get { return DrawMenu.GetOnlyReady(SpellSlot.Q); } }

                public static SharpDX.Color ColorQ { get { return DrawMenu.GetColorPicker(SpellSlot.Q).ToSharpDX(); } }

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

        internal enum E_Status
        {
            EReady,
            CanWalk,
            CannotWalk,
            NoDagger,
            StillAir
        }
    }
}
