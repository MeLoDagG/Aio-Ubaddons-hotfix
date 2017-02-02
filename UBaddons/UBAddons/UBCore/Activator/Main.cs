using EloBuddy;
using EloBuddy.Sandbox;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Spells;
using System;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Libs.Base;
using UBAddons.Libs.ColorPicker;
using UBAddons.Libs.Dictionary;

namespace UBAddons.UBCore.Activator
{
    class Main : IModuleBase
    {
        public static bool Initialized { get; private set; }
        public static Menu Activator, Potions, AttackMenu, DefensiveMenu, List, UtilityMenu, SpellsMenu, Clean;
        static Main()
        {
            //if (AddonManager.IsAddonLoaded("UBActivator"))
            if (MainMenu.MenuInstances.FirstOrDefault(x => x.Key.Contains("UBActivator")).Value == null)
            {
                try
                {
                    Activator = MainMenu.AddMenu("UBActivator", "UBActivator", "Activator for UBAddons");
                    Activator.AddLabel("Wait to ignoredev fix the event, so now this Activator is useless");
                    if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Heal) || SummonerSpells.PlayerHas(SummonerSpellsEnum.Ignite) 
                        || SummonerSpells.PlayerHas(SummonerSpellsEnum.Exhaust) || SummonerSpells.PlayerHas(SummonerSpellsEnum.Barrier) ||
                          SummonerSpells.PlayerHas(SummonerSpellsEnum.Mark) || SummonerSpells.PlayerHas(SummonerSpellsEnum.Smite))
                    {
                        SpellsMenu = Activator.AddSubMenu("Spells", "UBActivator.Spells", "Setting your Spells here");
                        {
                            if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Heal))
                            {
                                SpellsMenu.Add("Heal", new GroupLabel("Summoner Heal"));
                                SpellsMenu.Add("Heal.Enabled", new CheckBox("Use Heal"));
                                SpellsMenu.Add("Heal.Combo", new CheckBox("Only on combo", false));
                                SpellsMenu.Add("Heal.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                SpellsMenu.Add("Heal.Ally.Enabled", new CheckBox("Heal on Ally"));
                                SpellsMenu.Add("Heal.Smart", new CheckBox("Smart Heal"));
                                SpellsMenu.Add("Heal.MyHP", new Slider("Use if my HP below {0}%", 20, 0, 100));
                                SpellsMenu.Add("Heal.AllyHP", new Slider("Use if ally's HP below {0}%", 15, 0, 100));
                            }
                            if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Ignite))
                            {
                                SpellsMenu.Add("Ignite", new GroupLabel("Summoner Ignite"));
                                SpellsMenu.Add("Ignite.Enabled", new CheckBox("Use Ignite"));
                                SpellsMenu.Add("Ignite.Combo", new CheckBox("Only on combo", false));
                                //SpellsMenu.Add("Ignite.Save", new CheckBox("Save Ignite"));
                                //SpellsMenu.AddLabel("Tips: Ignite won't waste if can kill target by another spell, or can kill by your AA");
                            }
                            if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Exhaust))
                            {
                                SpellsMenu.Add("Exhaust", new GroupLabel("Summoner Exhaust"));
                                SpellsMenu.Add("Exhaust.Enabled", new CheckBox("Use Exhaust"));
                                SpellsMenu.Add("Exhaust.Combo", new CheckBox("Only on combo", false));
                                SpellsMenu.Add("Exhaust.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                SpellsMenu.Add("Exhaust.AllyHP", new Slider("Use if ally's HP below {0}%", 35, 0, 100));
                                SpellsMenu.Add("Exhaust.EnemyHP", new Slider("Use if enemy HP below {0}%", 35, 0, 100));
                                SpellsMenu.Add("Exhaust.Target", new ComboBox("What target should Exhauts", 0, "Auto", "Least HP", "Most AA dmg", "Most AP dmg", "Most SpeedMove", "Most Attack speed"));
                            }
                            if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Barrier))
                            {
                                SpellsMenu.Add("Barrier", new GroupLabel("Summoner Barrier"));
                                SpellsMenu.Add("Barrier.Enabled", new CheckBox("Use Barrier"));
                                SpellsMenu.Add("Barrier.Combo", new CheckBox("Only on combo", false));
                                SpellsMenu.Add("Barrier.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                SpellsMenu.Add("Barrier.Smart", new CheckBox("Smart Barrier"));
                                SpellsMenu.Add("Barrier.MyHP", new Slider("Use if my HP below {0}%", 20, 0, 100));
                                SpellsMenu.Add("Barrier.Spells", new CheckBox("Shield on Enemy's Spells"));
                                var ValueDmg = SpellsMenu.Add("Barrier.Value", new Slider("Only block if enemy damage more than", 50, 1, 200));
                                ValueDmg.DisplayName = "Only block if enemy damage more than " + ValueDmg.CurrentValue * 50;
                                ValueDmg.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                {
                                    ValueDmg.DisplayName = "Only block if enemy damage more than " + valueargs.NewValue * 50;
                                };
                            }
                            if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Mark))
                            {
                                SpellsMenu.Add("Mark", new GroupLabel("Summoner Mark"));
                                SpellsMenu.Add("Mark.Enabled", new CheckBox("Use Mark"));
                                SpellsMenu.Add("Mark.Percent", new Slider("Cast if percent more than {0}%", 60));
                            }
                            if (SummonerSpells.PlayerHas(SummonerSpellsEnum.Smite))
                            {
                                SpellsMenu.Add("Smite", new GroupLabel("Summoner Smite"));
                                SpellsMenu.Add("Smite.Enabled", new CheckBox("Use Smite"));
                                SpellsMenu.Add("Smite.Enabled.OnMonster", new CheckBox("Use On Important Monster"));
                                SpellsMenu.Add("Smite.Enabled.OnChamps", new CheckBox("Use On Champ", false));
                                SpellsMenu.Add("Smite.KillSteal", new CheckBox("Use KillSteal"));
                            }
                        }
                    }

                    Potions = Activator.AddSubMenu("Potions", "UBActivator.Potion", "Setting your potion here");
                    {
                        Potions.Add("Label", new GroupLabel("Please buy a Potions to inject"));
                    }

                    AttackMenu = Activator.AddSubMenu("Offensive", "UBActivator.Attack", "Setting your offensive item below");
                    {
                        AttackMenu.Add("Label", new GroupLabel("Please buy an Attack Item to inject"));
                    }

                    DefensiveMenu = Activator.AddSubMenu("Defensive", "UBActivator.Defensive", "Setting your defensive below");
                    {
                        DefensiveMenu.Add("Label", new GroupLabel("Please buy a Defensive Item to inject"));
                    }
                    //List = Activator.AddSubMenu("SpellList", "UBActivator.SpellList", "Setting your detais defense below");
                    UtilityMenu = Activator.AddSubMenu("Utility", "UBActivator.Utility", "Setting your Utility below");
                    {
                        UtilityMenu.Add("Trinket" + Player.Instance.ChampionName, new ComboBox("Trinket Change", CrazyTargetSelector.GetPriority(Player.Instance) <= 2 ? 2 : 1, "None", "Farsight Alteration", "Oracle Alteration"));
                        UtilityMenu.Add("Label", new GroupLabel("Please buy a Utility Item to inject"));
                    }

                    Clean = Activator.AddSubMenu("Clean", "UBActivator.Clean", "Setting your Utility below");
                    {
                        if (!SummonerSpells.PlayerHas(SummonerSpellsEnum.Cleanse))
                        {
                            Clean.Add("Label", new GroupLabel("You don't have summoner cleanse. Please buy a Clean Item to inject"));
                        }
                        else
                        {
                            Clean.Add("Cleanse", new GroupLabel("Summoner Cleanse"));
                            Clean.Add("Cleanse.Enabled", new GroupLabel("Use Cleanse"));
                            Clean.Add("Cleanse.BuffType", new Label("List of BuffType"));
                            Clean.Add("Cleanse." + BuffType.Blind, new CheckBox("Use On Blind", false));
                            Clean.Add("Cleanse." + BuffType.Charm, new CheckBox("Use On Charm"));
                            Clean.Add("Cleanse." + BuffType.Disarm, new CheckBox("Use On Disarm", false));
                            Clean.Add("Cleanse." + BuffType.Fear, new CheckBox("Use On Fear"));
                            Clean.Add("Cleanse." + BuffType.Flee, new CheckBox("Use On Flee"));
                         //   Clean.Add("Cleanse." + BuffType.Frenzy, new CheckBox("Use On Frenzy", false));
                            Clean.Add("Cleanse." + BuffType.Knockback, new CheckBox("Use On Knockback"));
                            Clean.Add("Cleanse." + BuffType.Knockup, new CheckBox("Use On Knockup"));
                            Clean.Add("Cleanse." + BuffType.NearSight, new CheckBox("Use On NearSight", false));
                            Clean.Add("Cleanse." + BuffType.Poison, new CheckBox("Use On Poison", false));
                            Clean.Add("Cleanse." + BuffType.Polymorph, new CheckBox("Use On Polymorph"));
                            Clean.Add("Cleanse." + BuffType.Silence, new CheckBox("Use On Silence", false));
                            Clean.Add("Cleanse." + BuffType.Slow, new CheckBox("Use On Slow", false));
                            Clean.Add("Cleanse." + BuffType.Snare, new CheckBox("Use On Snare"));
                            Clean.Add("Cleanse." + BuffType.Stun, new CheckBox("Use On Stun"));
                            Clean.Add("Cleanse." + BuffType.Suppression, new CheckBox("Use On Suppression"));
                            Clean.Add("Cleanse." + BuffType.Taunt, new CheckBox("Use On Taunt"));
                            var dur = Clean.Add("Cleanse.Duration", new Slider("Use only if duration more than", 15));
                            dur.DisplayName = "Use only if duration more than " + dur.CurrentValue * 50;
                            dur.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                            {
                                dur.DisplayName = "Use only if duration more than " + valueargs.NewValue * 50;
                            };
                            var min = Clean.Add("Cleanse.Delay.Min", new Slider("Min delay {0}", 300, 0, 1000));
                            var max = Clean.Add("Cleanse.Delay.Max", new Slider("Max delay {0}", 500, 0, 2500));
                            min.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                            {
                                if (valueargs.NewValue > max.CurrentValue)
                                {
                                    send.CurrentValue = valueargs.OldValue;
                                    Log.UBNotification.ShowNotif("UBAddons Warning", "Min value cann't be greater than max value", "warn");
                                }
                            };
                            max.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                            {
                                if (valueargs.NewValue < min.CurrentValue)
                                {
                                    send.CurrentValue = valueargs.OldValue;
                                    Log.UBNotification.ShowNotif("UBAddons Warning", "Max value cann't be less than min value", "warn");
                                }
                            };
                            Clean.AddSeparator();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Debug.Print(e.ToString(), General.Console_Message.Error);
                }
            }
            else
            {
                Core.DelayAction(() =>
                    {
                        Log.UBNotification.ShowNotif("UBAdddons Notification", "You're using my old Activator please untick it and reload.", "warn");
                        Chat.Print("You're using my old Activator, please untick it and reload.");
                    }, 800);
            }
            if (Activator != null)
            { 
                Shop.OnBuyItem += delegate (AIHeroClient sender, ShopActionEventArgs args)
                {
                    if (!sender.IsMe) return;
                    var supporteditem = true;
                    switch (args.Id)
                    {
                        #region Add Potions Method:
                        case (int)ItemId.Health_Potion:
                            {
                                if (Potions != null)
                                {
                                    try
                                    {
                                        if (Potions["Label"].Cast<GroupLabel>() != null)
                                        {
                                            Potions.Remove("Label");
                                        }
                                        Potions.Add(ItemId.Health_Potion.ToString(), new CheckBox("Health Potion"));
                                        Potions.Add(ItemId.Health_Potion + ".HP", new Slider("HP below {0}% use Potion", 65, 0, 100));
                                        Potions.AddSeparator();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;

                        case (int)ItemId.Total_Biscuit_of_Rejuvenation:
                            {
                                if (Potions != null)
                                {
                                    try
                                    {
                                        if (Potions["Label"].Cast<GroupLabel>() != null)
                                        {
                                            Potions.Remove("Label");
                                        }
                                        Potions.Add(ItemId.Total_Biscuit_of_Rejuvenation.ToString(), new CheckBox("Biscuit"));
                                        Potions.Add(ItemId.Total_Biscuit_of_Rejuvenation + ".HP", new Slider("HP below {0}% use Potion", 60, 0, 100));
                                        Potions.AddSeparator();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Refillable_Potion:
                            {
                                if (Potions != null)
                                {
                                    try
                                    {
                                        if (Potions["Label"].Cast<GroupLabel>() != null)
                                        {
                                            Potions.Remove("Label");
                                        }
                                        Potions.Add(ItemId.Refillable_Potion.ToString(), new CheckBox("Refillable Potion"));
                                        Potions.Add(ItemId.Refillable_Potion + ".HP", new Slider("HP below {0}% use Potion", 65, 0, 100));
                                        Potions.AddSeparator();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Hunters_Potion:
                            {
                                if (Potions != null)
                                {
                                    try
                                    {
                                        if (Potions["Label"].Cast<GroupLabel>() != null)
                                        {
                                            Potions.Remove("Label");
                                        }
                                        Potions.Add(ItemId.Hunters_Potion.ToString(), new CheckBox("Health Potion"));
                                        Potions.Add(ItemId.Hunters_Potion + ".MP", new CheckBox("Prevent use Potion if the restore MP more than max MP"));
                                        Potions.Add(ItemId.Hunters_Potion + ".HP", new Slider("HP below {0}% use Potion", 65, 0, 100));
                                        Potions.AddSeparator();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Corrupting_Potion:
                            {
                                if (Potions != null)
                                {
                                    try
                                    {
                                        if (Potions["Label"].Cast<GroupLabel>() != null)
                                        {
                                            Potions.Remove("Label");
                                        }
                                        Potions.Add(ItemId.Corrupting_Potion.ToString(), new CheckBox("Corrupting Potion"));
                                        Potions.Add(ItemId.Corrupting_Potion + ".MP", new CheckBox("Prevent use Potion if the restore MP more than max MP"));
                                        Potions.Add(ItemId.Corrupting_Potion + ".HP", new Slider("HP below {0}% use Potion", 65, 0, 100));
                                        Potions.AddSeparator();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Add Attack Item Method:

                        #region Tiamat / Ravenous Hydra / Titanic Hydra
                        case (int)ItemId.Tiamat:
                        case (int)ItemId.Ravenous_Hydra:
                            {
                                if (AttackMenu != null)
                                {
                                    try
                                    {
                                        if (AttackMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            AttackMenu.Remove("Label");
                                        }
                                        if (AttackMenu["Tiamat"].Cast<GroupLabel>() == null)
                                        {
                                            AttackMenu.Add("Tiamat", new GroupLabel("Tiamat / Ravenous Hydra"));
                                            AttackMenu.Add("Tiamat.Enabled", new CheckBox("Use Tiamat/ Ravenous Hydra"));
                                            AttackMenu.Add("Tiamat.Style", new ComboBox("How to use", 0, "Canceling AA Amination", "On Collision"));
                                            AttackMenu.Add("Tiamat.Combo", new CheckBox("Only on combo"));
                                            AttackMenu.Add("Tiamat.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            AttackMenu.Add("Tiamat.Killsteal", new CheckBox("Killsteal"));
                                            AttackMenu.Add("Tiamat.Clear", new CheckBox("Use on Clear"));
                                            AttackMenu.Add("Tiamat.Clear.Hit", new Slider("Use only hit {0} minions / monster", 5, 0, 10));
                                            AttackMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Titanic_Hydra:
                            {
                                if (AttackMenu != null)
                                {
                                    try
                                    {
                                        if (AttackMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            AttackMenu.Remove("Label");
                                        }
                                        if (AttackMenu["Tiamat"].Cast<GroupLabel>() != null)
                                        {
                                            AttackMenu.Remove("Tiamat");
                                            AttackMenu.Remove("Tiamat.Enabled");
                                            AttackMenu.Remove("Tiamat.Combo");
                                            AttackMenu.Remove("Tiamat.Stealth");
                                            AttackMenu.Remove("Tiamat.Killsteal");
                                            AttackMenu.Remove("Tiamat.Clear");
                                            AttackMenu.Remove("Tiamat.Clear.Hit");
                                        }
                                        if (AttackMenu["Titanic_Hydra"].Cast<GroupLabel>() == null)
                                        {
                                            AttackMenu.Add("Titanic_Hydra", new GroupLabel("Titanic Hydra"));
                                            AttackMenu.Add("Titanic_Hydra.Enabled", new CheckBox("Use Titanic_Hydra"));
                                            AttackMenu.Add("Titanic_Hydra.Style", new ComboBox("How to use", 0, "After AA", "Before AA"));
                                            AttackMenu.Add("Titanic_Hydra.Combo", new CheckBox("Only on combo"));
                                            AttackMenu.Add("Titanic_Hydra.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            AttackMenu.Add("Titanic_Hydra.Clear", new CheckBox("Use on Clear"));
                                            AttackMenu.Add("Titanic_Hydra.Clear.Hit", new Slider("Use only hit {0} minions / monster", 5, 0, 10));
                                            AttackMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Blade of Ruined King / Bilgewater Cutlass
                        case (int)ItemId.Blade_of_the_Ruined_King:
                        case (int)ItemId.Bilgewater_Cutlass:
                            {
                                if (AttackMenu != null)
                                {
                                    try
                                    {
                                        if (AttackMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            AttackMenu.Remove("Label");
                                        }
                                        if (AttackMenu["Bork"].Cast<GroupLabel>() == null)
                                        {
                                            AttackMenu.Add("Bork", new GroupLabel("Bork / Cutlass"));
                                            AttackMenu.Add("Bork.Enabled", new CheckBox("Use Bork / Cutlass"));
                                            AttackMenu.Add("Bork.Combo", new CheckBox("Only on combo"));
                                            AttackMenu.Add("Bork.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            AttackMenu.Add("Bork.Killsteal", new CheckBox("Killsteal"));
                                            AttackMenu.Add("Bork.MyHP", new Slider("Use if My HP below {0}%", 70));
                                            AttackMenu.Add("Bork.EnemyHP", new Slider("Use if Enemy's HP below {0}", 70));
                                            AttackMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Hextech Item
                        case (int)ItemId.Hextech_GLP_800:
                        case (int)ItemId.Hextech_Protobelt_01:
                        case (int)ItemId.Hextech_Gunblade:
                            {
                                if (AttackMenu != null)
                                {
                                    try
                                    {
                                        if (AttackMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            AttackMenu.Remove("Label");
                                        }
                                        if (AttackMenu["Hextech"].Cast<GroupLabel>() == null)
                                        {
                                            AttackMenu.Add("Hextech", new GroupLabel("Hextech"));
                                            AttackMenu.Add("Hextech.Enabled", new CheckBox("Use Hextech"));
                                            AttackMenu.Add("Hextech.Combo", new CheckBox("Only on combo"));
                                            AttackMenu.Add("Hextech.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            AttackMenu.Add("Hextech.Killsteal", new CheckBox("Killsteal"));
                                            AttackMenu.Add("Hextech.AntiGapcloser", new CheckBox("Anti Gapcloser"));
                                            AttackMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #endregion

                        #region Defensive
                        case (int)ItemId.Face_of_the_Mountain:
                            {
                                if (DefensiveMenu != null)
                                {
                                    try
                                    {
                                        if (DefensiveMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            DefensiveMenu.Remove("Label");
                                        }
                                        if (DefensiveMenu["Face_of_the_Mountain"].Cast<GroupLabel>() == null)
                                        {
                                            DefensiveMenu.Add("Face_of_the_Mountain", new GroupLabel("Face of the Mountain (FOTM)"));
                                            DefensiveMenu.Add("Face_of_the_Mountain.Enabled", new CheckBox("Use FOTM"));
                                            DefensiveMenu.Add("Face_of_the_Mountain.Combo", new CheckBox("Only on combo", false));
                                            DefensiveMenu.Add("Face_of_the_Mountain.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            DefensiveMenu.Add("Face_of_the_Mountain.Turret", new CheckBox("Shield on Turret"));
                                            DefensiveMenu.Add("Face_of_the_Mountain.EnemyAA", new CheckBox("Shield on Enemy's AA"));
                                            DefensiveMenu.Add("Face_of_the_Mountain.AllyHP", new Slider("Shield on Ally if Hp below", 30));
                                            DefensiveMenu.Add("Face_of_the_Mountain.Spells", new CheckBox("Shield on Enemy's Spells"));
                                            var ValueDmg = DefensiveMenu.Add("Face_of_the_Mountain.Value", new Slider("Only block if enemy damage more than" , 50, 1, 200));
                                            ValueDmg.DisplayName = "Only block if enemy damage more than " + ValueDmg.CurrentValue * 50;
                                            ValueDmg.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                ValueDmg.DisplayName = "Only block if enemy damage more than " + valueargs.NewValue * 50;
                                            };
                                            DefensiveMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Locket_of_the_Iron_Solari:
                            {
                                if (DefensiveMenu != null)
                                {
                                    try
                                    {
                                        if (DefensiveMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            DefensiveMenu.Remove("Label");
                                        }
                                        if (DefensiveMenu["Locket_of_the_Iron_Solari"].Cast<GroupLabel>() == null)
                                        {
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari", new GroupLabel("Locket of the Iron Solari (Solari)"));
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari.Enabled", new CheckBox("Use Solari"));
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari.Combo", new CheckBox("Only on combo", false));
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari.Turret", new CheckBox("Shield on Turret"));
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari.EnemyAA", new CheckBox("Shield on Enemy's AA"));
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari.AllyHP", new Slider("Shield on Ally if Hp below", 30));
                                            DefensiveMenu.Add("Locket_of_the_Iron_Solari.Spells", new CheckBox("Shield on Enemy's Spells"));
                                            var ValueDmg = DefensiveMenu.Add("Locket_of_the_Iron_Solari.Value", new Slider("Only block if enemy damage more than", 50, 1, 200));
                                            ValueDmg.DisplayName = "Only block if enemy damage more than " + ValueDmg.CurrentValue * 50;
                                            ValueDmg.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                ValueDmg.DisplayName = "Only block if enemy damage more than " + valueargs.NewValue * 50;
                                            };
                                            DefensiveMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Randuins_Omen:
                            {
                                if (DefensiveMenu != null)
                                {
                                    try
                                    {
                                        if (DefensiveMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            DefensiveMenu.Remove("Label");
                                        }
                                        if (DefensiveMenu["Randuins_Omen"].Cast<GroupLabel>() == null)
                                        {
                                            DefensiveMenu.Add("Randuins_Omen", new GroupLabel("Randuins Omen (Randuins)"));
                                            DefensiveMenu.Add("Randuins_Omen.Enabled", new CheckBox("Use Randuins"));
                                            DefensiveMenu.Add("Randuins_Omen.Combo", new CheckBox("Only on combo", false));
                                            DefensiveMenu.Add("Randuins_Omen.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            DefensiveMenu.Add("Randuins_Omen.Hit", new Slider("Use if hit {0} champ(s)", 2, 1, 5));
                                            DefensiveMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Archangels_Staff:
                        case (int)ItemId.Archangels_Staff_Quick_Charge:
                        case (int)ItemId.Seraphs_Embrace:
                            {
                                if (DefensiveMenu != null)
                                {
                                    try
                                    {
                                        if (DefensiveMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            DefensiveMenu.Remove("Label");
                                        }
                                        if (DefensiveMenu["Seraphs_Embrace"].Cast<GroupLabel>() == null)
                                        {
                                            DefensiveMenu.Add("Seraphs_Embrace", new GroupLabel("Seraphs Embrace (Seraphs)"));
                                            DefensiveMenu.Add("Seraphs_Embrace.Enabled", new CheckBox("Use Seraphs"));
                                            DefensiveMenu.Add("Seraphs_Embrace.Combo", new CheckBox("Only on combo", false));
                                            DefensiveMenu.Add("Seraphs_Embrace.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            DefensiveMenu.Add("Seraphs_Embrace.Turret", new CheckBox("Shield on Turret", false));
                                            DefensiveMenu.Add("Seraphs_Embrace.EnemyAA", new CheckBox("Shield on Enemy's AA"));
                                            DefensiveMenu.Add("Seraphs_Embrace.AllyHP", new Slider("Shield on Ally if Hp below", 30));
                                            DefensiveMenu.Add("Seraphs_Embrace.Spells", new CheckBox("Shield on Enemy's Spells"));
                                            var ValueDmg = DefensiveMenu.Add("Seraphs_Embrace.Value", new Slider("Only block if enemy damage more than", 50, 1, 200));
                                            ValueDmg.DisplayName = "Only block if enemy damage more than " + ValueDmg.CurrentValue * 50;
                                            ValueDmg.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                ValueDmg.DisplayName = "Only block if enemy damage more than " + valueargs.NewValue * 50;
                                            };
                                            DefensiveMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                                {
                                    if (UtilityMenu != null)
                                    {
                                        try
                                        {
                                            if (UtilityMenu["Label"].Cast<GroupLabel>() != null)
                                            {
                                                UtilityMenu.Remove("Label");
                                            }
                                            if (UtilityMenu["Stack"].Cast<CheckBox>() == null)
                                            {
                                                UtilityMenu.Add("Stack", new CheckBox("Auto Stack"));
                                                UtilityMenu.AddSeparator();
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Debug.Print(e.ToString(), Console_Message.Error);
                                        }
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Zhonyas_Hourglass:
                        case (int)ItemId.Wooglets_Witchcap:
                            {
                                if (DefensiveMenu != null)
                                {
                                    try
                                    {
                                        if (DefensiveMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            DefensiveMenu.Remove("Label");
                                        }
                                        if (DefensiveMenu["Zhonyas"].Cast<GroupLabel>() == null)
                                        {
                                            DefensiveMenu.Add("Zhonyas", new GroupLabel("Zhonyas Hourglass (Zhonyas) / Wooglets Witchcap (Cap)"));
                                            DefensiveMenu.Add("Zhonyas.Enabled", new CheckBox("Use Zhonya / Cap"));
                                            DefensiveMenu.Add("Zhonyas.Combo", new CheckBox("Only on combo", false));
                                            DefensiveMenu.Add("Zhonyas.Stealth", new CheckBox("Prevent use if I'm stealth"));
                                            DefensiveMenu.Add("Zhonyas.Turret", new CheckBox("Shield on Turret", false));
                                            DefensiveMenu.Add("Zhonyas.EnemyAA", new CheckBox("Shield on Enemy's AA"));
                                            DefensiveMenu.Add("Zhonyas.AllyHP", new Slider("Shield on Ally if Hp below", 30));
                                            DefensiveMenu.Add("Zhonyas.Spells", new CheckBox("Shield on Enemy's Spells"));
                                            var ValueDmg = DefensiveMenu.Add("Zhonyas.Value", new Slider("Only block if enemy damage more than", 50, 1, 200));
                                            ValueDmg.DisplayName = "Only block if enemy damage more than " + ValueDmg.CurrentValue * 50;
                                            ValueDmg.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                ValueDmg.DisplayName = "Only block if enemy damage more than " + valueargs.NewValue * 50;
                                            };
                                            DefensiveMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Clean
                        case (int)ItemId.Quicksilver_Sash:
                        case (int)ItemId.Mercurial_Scimitar:
                        case (int)ItemId.Dervish_Blade:
                            {
                                if (Clean != null)
                                {
                                    try
                                    {
                                        if (Clean["Label"].Cast<GroupLabel>() != null)
                                        {
                                            Clean.Remove("Label");
                                        }
                                        if (Clean["QSS"].Cast<GroupLabel>() == null)
                                        {
                                            Clean.Add("QSS", new GroupLabel("Quicksilver Sash / Mercurial Scimitar / Dervish Blade (QSS)"));
                                            Clean.Add("QSS.Enabled", new GroupLabel("Use QSS"));
                                            Clean.Add("QSS.BuffType", new Label("List of BuffType"));
                                            Clean.Add("QSS." + BuffType.Blind, new CheckBox("Use On Blind", false));
                                            Clean.Add("QSS." + BuffType.Charm, new CheckBox("Use On Charm"));
                                            Clean.Add("QSS." + BuffType.Disarm, new CheckBox("Use On Disarm", false));
                                            Clean.Add("QSS." + BuffType.Fear, new CheckBox("Use On Fear"));
                                            Clean.Add("QSS." + BuffType.Flee, new CheckBox("Use On Flee"));
                                        //    Clean.Add("QSS." + BuffType.Frenzy, new CheckBox("Use On Frenzy", false));
                                            Clean.Add("QSS." + BuffType.Knockback, new CheckBox("Use On Knockback"));
                                            Clean.Add("QSS." + BuffType.Knockup, new CheckBox("Use On Knockup"));
                                            Clean.Add("QSS." + BuffType.NearSight, new CheckBox("Use On NearSight", false));
                                            Clean.Add("QSS." + BuffType.Poison, new CheckBox("Use On Poison", false));
                                            Clean.Add("QSS." + BuffType.Polymorph, new CheckBox("Use On Polymorph"));
                                            Clean.Add("QSS." + BuffType.Silence, new CheckBox("Use On Silence", false));
                                            Clean.Add("QSS." + BuffType.Slow, new CheckBox("Use On Slow", false));
                                            Clean.Add("QSS." + BuffType.Snare, new CheckBox("Use On Snare"));
                                            Clean.Add("QSS." + BuffType.Stun, new CheckBox("Use On Stun"));
                                            Clean.Add("QSS." + BuffType.Suppression, new CheckBox("Use On Suppression"));
                                            Clean.Add("QSS." + BuffType.Taunt, new CheckBox("Use On Taunt"));
                                            var dur = Clean.Add("QSS.Duration", new Slider("Use only if duration more than", 15));
                                            dur.DisplayName = "Use only if duration more than " + dur.CurrentValue * 50;
                                            dur.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                dur.DisplayName = "Use only if duration more than " + valueargs.NewValue * 50;
                                            };
                                            var min = Clean.Add("QSS.Delay.Min", new Slider("Min delay {0}", 300, 0, 1000));
                                            var max = Clean.Add("QSS.Delay.Max", new Slider("Max delay {0}", 500, 0, 2500));
                                            min.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                if (valueargs.NewValue > max.CurrentValue)
                                                {
                                                    send.CurrentValue = valueargs.OldValue;
                                                    Log.UBNotification.ShowNotif("UBAddons Warning", "Min value cann't be greater than max value", "warn");
                                                }
                                            };
                                            max.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                if (valueargs.NewValue < min.CurrentValue)
                                                {
                                                    send.CurrentValue = valueargs.OldValue;
                                                    Log.UBNotification.ShowNotif("UBAddons Warning", "Max value cann't be less than min value", "warn");
                                                }
                                            };
                                            Clean.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Mikaels_Crucible:
                            {
                                if (Clean != null)
                                {
                                    try
                                    {
                                        if (Clean["Label"].Cast<GroupLabel>() != null)
                                        {
                                            Clean.Remove("Label");
                                        }
                                        if (Clean["Mikaels_Crucible"].Cast<GroupLabel>() == null)
                                        {
                                            Clean.Add("Mikaels_Crucible", new GroupLabel("Mikaels Crucible (Mikael)"));
                                            Clean.Add("Mikaels_Crucible.Enabled", new GroupLabel("Use Mikael"));
                                            foreach (var ally in EntityManager.Heroes.Allies)
                                            {
                                                Clean.Add("Mikaels_Crucible.Enabled." + ally.ChampionName, new GroupLabel("Use Mikael on " + ally.ChampionName));
                                            }
                                            Clean.Add("Mikaels_Crucible.BuffType", new Label("List of BuffType"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Blind, new CheckBox("Use On Blind", false));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Charm, new CheckBox("Use On Charm"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Disarm, new CheckBox("Use On Disarm", false));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Fear, new CheckBox("Use On Fear"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Flee, new CheckBox("Use On Flee"));
                                         //   Clean.Add("Mikaels_Crucible." + BuffType.Frenzy, new CheckBox("Use On Frenzy", false));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Knockback, new CheckBox("Use On Knockback"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Knockup, new CheckBox("Use On Knockup"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.NearSight, new CheckBox("Use On NearSight", false));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Poison, new CheckBox("Use On Poison", false));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Polymorph, new CheckBox("Use On Polymorph"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Silence, new CheckBox("Use On Silence", false));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Slow, new CheckBox("Use On Slow", false));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Snare, new CheckBox("Use On Snare"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Stun, new CheckBox("Use On Stun"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Suppression, new CheckBox("Use On Suppression"));
                                            Clean.Add("Mikaels_Crucible." + BuffType.Taunt, new CheckBox("Use On Taunt"));
                                            var dur = Clean.Add("Mikaels_Crucible.Duration", new Slider("Use only if duration more than", 15));
                                            dur.DisplayName = "Use only if duration more than " + dur.CurrentValue * 50;
                                            dur.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                dur.DisplayName = "Use only if duration more than " + valueargs.NewValue * 50;
                                            };
                                            var min = Clean.Add("Mikaels_Crucible.Delay.Min", new Slider("Min delay {0}", 300, 0, 1000));
                                            var max = Clean.Add("Mikaels_Crucible.Delay.Max", new Slider("Max delay {0}", 500, 0, 2500));
                                            min.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                if (valueargs.NewValue > max.CurrentValue)
                                                {
                                                    send.CurrentValue = valueargs.OldValue;
                                                    Log.UBNotification.ShowNotif("UBAddons Warning", "Min value cann't be greater than max value", "warn");
                                                }
                                            };
                                            max.OnValueChange += delegate (ValueBase<int> send, ValueBase<int>.ValueChangeArgs valueargs)
                                            {
                                                if (valueargs.NewValue < min.CurrentValue)
                                                {
                                                    send.CurrentValue = valueargs.OldValue;
                                                    Log.UBNotification.ShowNotif("UBAddons Warning", "Max value cann't be less than min value", "warn");
                                                }
                                            };
                                            Clean.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region Utility
                        case (int)ItemId.Youmuus_Ghostblade:
                            {
                                if (UtilityMenu != null)
                                {
                                    try
                                    {
                                        if (UtilityMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            UtilityMenu.Remove("Label");
                                        }
                                        UtilityMenu.Add(ItemId.Youmuus_Ghostblade + ".Enabled", new CheckBox("Use Youmuus Ghostblade"));
                                        UtilityMenu.AddSeparator();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Redemption:
                            {
                                if (UtilityMenu != null)
                                {
                                    try
                                    {
                                        if (UtilityMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            UtilityMenu.Remove("Label");
                                        }
                                        UtilityMenu.Add(ItemId.Redemption.ToString(), new GroupLabel("Redemption"));
                                        UtilityMenu.Add(ItemId.Redemption + ".Enabled", new CheckBox("Use Redemption"));
                                        UtilityMenu.Add(ItemId.Redemption + ".Combo", new CheckBox("Only on Combo"));
                                        UtilityMenu.Add(ItemId.Redemption + ".Percent", new Slider("Hit Percent: {0}%", 60));
                                        UtilityMenu.Add(ItemId.Redemption + ".Hit.Enemy", new Slider("Use if hit more than {0} enemy", 3, 1, 5));
                                        UtilityMenu.Add(ItemId.Redemption + ".Hit.Ally", new Slider("Use if hit more than {0} ally", 3, 1, 5));
                                        UtilityMenu.AddSeparator();
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;
                        case (int)ItemId.Tear_of_the_Goddess:
                        case (int)ItemId.Tear_of_the_Goddess_Quick_Charge:
                        case (int)ItemId.Manamune:
                        case (int)ItemId.Manamune_Quick_Charge:
                            {
                                if (UtilityMenu != null)
                                {
                                    try
                                    {
                                        if (UtilityMenu["Label"].Cast<GroupLabel>() != null)
                                        {
                                            UtilityMenu.Remove("Label");
                                        }
                                        if (UtilityMenu["Stack"].Cast<CheckBox>() == null)
                                        {
                                            UtilityMenu.Add("Stack", new CheckBox("Auto Stack"));
                                            UtilityMenu.AddSeparator();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug.Print(e.ToString(), Console_Message.Error);
                                    }
                                }
                            }
                            break;

                        #endregion

                        default:
                            {
                                supporteditem = false;
                                return;
                            }
                    }
                    if (supporteditem)
                    {
                        Log.UBNotification.ShowNotif("UBAdddons Notification", "You just buy a Supported Item, please check menu", "blue");
                    }
                };
                //if (Activator["UBActivator.OnTick"].Cast<CheckBox>().CurrentValue)
                //{
                //    Game.OnTick += Game_On;
                //}
                //if (Activator["UBActivator.OnUpdate"].Cast<CheckBox>().CurrentValue)
                //{
                //    Game.OnUpdate += Game_On;
                //}
                Gapcloser.OnGapcloser += AttackItem.OnGapCloser;
                Orbwalker.OnPostAttack += AttackItem.OnPostAttack;
                Orbwalker.OnPreAttack += AttackItem.OnPreAttack;
                Obj_AI_Base.OnBuffGain += Cleanse.OnBuffGain;
                Obj_AI_Base.OnProcessSpellCast += Spells.OnProcessSpellCast;
                AttackableUnit.OnDamage += Utility.OnDamage;
                GameObject.OnCreate += Spells.GameObject_OnCreate;
            }
        }
        public bool ShouldExecuted()
        {
            return true;
        }

        public void OnLoad()
        {
            Initialize();
        }

        public void Execute()
        {
            AttackItem.Bork();
            AttackItem.Hextech();
            AttackItem.Tiamat();
            Utility.ChangeTrinket();
            Utility.Stack();
            Utility.Utility_Usage();
        }
        /// <summary>
        /// Here Is Called Initialize
        /// </summary>
        public static void Initialize()
        {

            if (Initialized)
            {
                return;
            }
            Initialized = true;
        }
    }
}
