using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Libs.Base;
using UBAddons.Log;

namespace UBAddons.UBCore.AutoLv
{
    class AutoLv : IModuleBase
    {
        internal static Menu LvMenu;
        public static bool Initialized { get; private set; }
        static AutoLv()
        {
            if (MainMenu.MenuInstances.FirstOrDefault(x => x.Key.Contains("UBAddons")).Value == null)
            {
                Debug.Print("There's has a problem when trying to inject Auto Level up. Please report to Uzumaki Boruto", Console_Message.Error);
            }
            else
            {
                try
                {
                    FileHandle.Read();
                    var Menu = MainMenu.MenuInstances.FirstOrDefault(x => x.Key.Contains("UBAddons")).Value.FirstOrDefault();
                    LvMenu = Menu.AddSubMenu("AutoLv", "UBAddons.AutoLv", "Auto Level for UBAddons");
                    {
                        LvMenu.Add("Enable", new CheckBox("Enable auto Level up"));
                        var min = LvMenu.Add("Delay.Min", new Slider("Min Delay", 300, 0, 2000));
                        var max = LvMenu.Add("Delay.Max", new Slider("Max Delay", 500, 0, 2000));
                        min.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            if (args.NewValue > max.CurrentValue)
                            {
                                sender.CurrentValue = args.OldValue;
                                UBNotification.ShowNotif("UBAddons Warning", "Min value cann't be greater than max value", "warn");
                            }
                        };
                        max.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            if (args.NewValue < min.CurrentValue)
                            {
                                sender.CurrentValue = args.OldValue;
                                UBNotification.ShowNotif("UBAddons Warning", "Max value cann't be less than min value", "warn");
                            }
                        };
                        var reset = LvMenu.Add("Reset", new CheckBox("Tick here to reset"));
                        reset.CurrentValue = false;
                        var fastconfig = LvMenu.Add("Fast", new ComboBox("Fast config", 0, "Choose Here", "R → Q → W → E", "R → Q → E → W", "R → W → Q → E", "R → W → E → Q", "R → E → Q → W", "R → E → W → Q"));
                        fastconfig.CurrentValue = 0;
                        var data = LvMenu.Add("Data", new ComboBox("Your Data", 0, "Data 1", "Data 2", "Data 3"));
                        var label = LvMenu.Add("Label", new Label(""));
                        label.DisplayName = FileHandle.Preview(data.CurrentValue);
                        var save = LvMenu.Add("Save", new CheckBox("Tick here to save your skillorder"));
                        save.CurrentValue = false;
                        var load = LvMenu.Add("Load", new CheckBox("Tick here to load your skillorder"));
                        load.CurrentValue = false;
                        var delete = LvMenu.Add("Delete", new CheckBox("Tick here to delete your skillorder"));
                        delete.CurrentValue = false;
                        LvMenu.AddGroupLabel("Custom Level Settings");
                        for (int i = 1; i <= 18; i++)
                        {
                            LvMenu.Add(Player.Instance.ChampionName + "." + i, new ComboBox("Level " + i, Database.ConvertToInt(Database.SkillOrder(Player.Instance.Hero))[i - 1], "None", "Q", "W", "E", "R"));
                        }
                        reset.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            if (args.NewValue)
                            {
                                for (var i = 1; i <= 18; i++)
                                {
                                    LvMenu[Player.Instance.ChampionName + "." + i].Cast<ComboBox>().CurrentValue = Database.ConvertToInt(Database.SkillOrder(Player.Instance.Hero))[i - 1];
                                }
                                reset.DisplayName = "Reseted";
                                Core.DelayAction(() => reset.DisplayName = "Tick here to reset", 1500);
                                reset.CurrentValue = false;
                            }
                        };
                        fastconfig.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            if (args.NewValue == 0) return;
                            var list = new List<int>();
                            switch (args.NewValue)
                            {
                                case 1:
                                    {
                                        list = new List<int>() { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                                    }
                                    break;
                                case 2:
                                    {
                                        list = new List<int>() { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                                    }
                                    break;
                                case 3:
                                    {
                                        list = new List<int>() { 2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                                    }
                                    break;
                                case 4:
                                    {
                                        list = new List<int>() { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
                                    }
                                    break;
                                case 5:
                                    {
                                        list = new List<int>() { 3, 1, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                                    }
                                    break;
                                case 6:
                                    {
                                        list = new List<int>() { 3, 2, 1, 3, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1 };
                                    }
                                    break;
                            }
                            fastconfig.CurrentValue = 0;
                            for (var i = 1; i <= 18; i++)
                            {
                                LvMenu[Player.Instance.ChampionName + "." + i].Cast<ComboBox>().CurrentValue = list[i -1];
                            }
                        };
                        data.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            label.DisplayName = FileHandle.Preview(args.NewValue);
                        };
                        save.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            if (args.NewValue)
                            {
                                var result = new List<int>();
                                for (var i = 1; i <= 18; i++)
                                {
                                    result.Add(LvMenu.VComboValue(Player.Instance.ChampionName + "." + i));
                                }
                                var skillorder = Database.ConvertToSpellSlot(result);
                                var key = Player.Instance.ChampionName + ".Data." + (LvMenu.VComboValue("Data") + 1);
                                if (FileHandle.SpellData.Any(x => x.Key.Equals(key)))
                                {
                                    FileHandle.SpellData[FileHandle.SpellData.FindIndex(x => x.Key.Equals(key))].SlotList = skillorder;
                                }
                                else
                                {
                                    FileHandle.SpellData.Add(new SkillOrder_Infomation(key, skillorder));
                                }
                                FileHandle.Write();
                                label.DisplayName = FileHandle.Preview(data.CurrentValue);
                                save.DisplayName = "Saved";
                                Core.DelayAction(() => save.DisplayName = "Tick here to save your skillorder", 1500);
                                save.CurrentValue = false;
                            }
                        };
                        load.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            if (args.NewValue)
                            {
                                var key = Player.Instance.ChampionName + ".Data." + (LvMenu.VComboValue("Data") + 1);
                                if (FileHandle.SpellData.Any(x => x.Key.Equals(key)))
                                {
                                    var spelldata = FileHandle.SpellData.FirstOrDefault(x => x.Key.Equals(key)).SlotList;
                                    var result = new List<int>();
                                    for (var i = 1; i <= 18; i++)
                                    {
                                        LvMenu[Player.Instance.ChampionName + "." + i].Cast<ComboBox>().CurrentValue = Database.ConvertToInt(spelldata)[i - 1];
                                    }
                                    label.DisplayName = FileHandle.Preview(data.CurrentValue);
                                    load.DisplayName = "Loaded";
                                    Core.DelayAction(() => load.DisplayName = "Tick here to load your skillorder", 1500);
                                    load.CurrentValue = false;
                                }
                                else
                                {
                                    load.DisplayName = "Couldn't load your data";
                                    Core.DelayAction(() => load.DisplayName = "Tick here to load your skillorder", 1500);
                                    load.CurrentValue = false;
                                }
                            }
                        };
                        delete.OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            if (args.NewValue)
                            {
                                var key = Player.Instance.ChampionName + ".Data." + (LvMenu.VComboValue("Data") + 1);
                                if (FileHandle.SpellData.Any(x => x.Key.Equals(key)))
                                {
                                    FileHandle.SpellData.RemoveAll(x => x.Key.Equals(key));
                                    label.DisplayName = FileHandle.Preview(data.CurrentValue);
                                    delete.DisplayName = "Deleted";
                                    Core.DelayAction(() => delete.DisplayName = "Tick here to delete your skillorder", 1500);
                                    delete.CurrentValue = false;
                                }
                                else
                                {
                                    delete.DisplayName = "Your Data already null";
                                    Core.DelayAction(() => delete.DisplayName = "Tick here to load your skillorder", 1500);
                                    label.DisplayName = FileHandle.Preview(data.CurrentValue);
                                    delete.CurrentValue = false;
                                }
                                FileHandle.Write();
                            }
                        };
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString(), Console_Message.Error);
                }
            }
        }

        private static void OnLevelUp()
        {
            if (!LvMenu.VChecked("Enable") || Player.Instance.SpellTrainingPoints.Equals(0)) return;
            LevelUp(Database.ConvertToSpellSlot()[Player.Instance.Level - 1]);           
        }
        private static void LevelUp(SpellSlot slot)
        {
            var Delay = new Random().Next(LvMenu.VSliderValue("Delay.Min"), LvMenu.VSliderValue("Delay.Max"));
            Core.DelayAction(() =>
            {
                try
                {
                    if (Player.Instance.Spellbook.CanSpellBeUpgraded(slot))
                    {
                        Player.LevelSpell(slot);
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString(), Console_Message.Error);
                }
            }, Delay);
        }

        public void OnLoad()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            Initialized = true;
        }

        public bool ShouldExecuted()
        {
            return LvMenu.VChecked("Enable") && !Player.Instance.SpellTrainingPoints.Equals(0);                 
        }

        public void Execute()
        {
            OnLevelUp();
        }
    }
}
