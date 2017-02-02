using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using EloBuddy.SDK.Events;
using UBAddons.Log;
using UBAddons.General;

namespace UBAddons.UBCore.AutoLv
{
    class FileHandle
    {
        private const string ColorFileName = "AutoLevel.json";
        private static string UBAddonsPath = Path.Combine(EloBuddy.Sandbox.SandboxConfig.DataDirectory, "UBAddons");
        private static string FilePath = Path.Combine(UBAddonsPath, ColorFileName);
        internal static List<SkillOrder_Infomation> SpellData = new List<SkillOrder_Infomation>();
        internal static bool Write()
        {
            if (!Directory.Exists(UBAddonsPath))
            {
                Debug.Print("Directory doesnt exist. Creating UBAddons folder", Console_Message.ColorPicker);
                Directory.CreateDirectory(UBAddonsPath);
            }
            string data = JsonConvert.SerializeObject(SpellData.OrderBy(x => x.Key), Formatting.Indented, new StringEnumConverter() { AllowIntegerValues = true });
            File.WriteAllText(FilePath, data);
            return true;
        }
        internal static bool Read()
        {
            if (!Directory.Exists(UBAddonsPath))
            {
                return false;
            }
            else
            {
                if (!File.Exists(FilePath))
                {
                    return false;
                }
                else
                {
                    string read = File.ReadAllText(FilePath);
                    SpellData = JsonConvert.DeserializeObject<List<SkillOrder_Infomation>>(read, new JsonSerializerSettings() { Formatting = Formatting.Indented, });
                    SpellData = SpellData.Distinct().ToList();
                    return true;
                }
            }
        }
        internal static string Preview(int Value)
        {
            if (!SpellData.Any(x => x.Key.Equals(Player.Instance.ChampionName + ".Data." + (Value + 1))))
            {
                return $"Data {Value + 1}: No have data yet.";
            }
            else
            {
                StringBuilder text = new StringBuilder($"Data {Value + 1}: ");
                try
                {
                    var slotlist = SpellData.FirstOrDefault(x => x.Key.Equals(Player.Instance.ChampionName + ".Data." + (Value + 1))).SlotList;
                    switch (Player.Instance.Hero)
                    {
                        case Champion.Ryze:
                            {
                                if (slotlist[5].Equals(SpellSlot.R) && slotlist[10].Equals(SpellSlot.R))
                                {
                                    int countQ = slotlist.Count(x => x.Equals(SpellSlot.Q));
                                    int countW = slotlist.Count(x => x.Equals(SpellSlot.W));
                                    int countE = slotlist.Count(x => x.Equals(SpellSlot.E));
                                    if (!countQ.Equals(6))
                                    {
                                        text.Append($"Q is {(countQ > 6 ? "more" : "less")} than 6");
                                        break;
                                    }
                                    if (!countW.Equals(5))
                                    {
                                        text.Append($"W is {(countW > 5 ? "more" : "less")} than 5");
                                        break;
                                    }
                                    if (!countE.Equals(5))
                                    {
                                        text.Append($"E is {(countE > 5 ? "more" : "less")} than 5");
                                        break;
                                    }
                                    text.Append($"R → ");
                                    SpellSlot[] ordered = new SpellSlot[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, };
                                    ordered = ordered.OrderBy(x => slotlist.LastIndexOf(x)).ToArray();
                                    foreach (var spell in ordered)
                                    {
                                        text.Append($"{spell.ToString()}{(ordered.Last().Equals(spell) ? "." : " → ")}");
                                    }
                                    text.Append($" With ");
                                    for (int i = 1; i <= 3; i++)
                                    {
                                        text.Append($"Level {i} : {slotlist[i - 1]} {(i.Equals(3) ? ". " : "| ")}");
                                    }
                                }
                                else
                                {
                                    text.Append($"Couldn't preview this profile. Check your opition at lv 6 or 11 or 16");

                                }
                            }
                                break;
                        case Champion.Udyr:
                            {
                                if (slotlist.GroupBy(x => x).Any(x => !x.Equals(SpellSlot.Unknown) && x.Count() > 5))
                                {
                                    foreach (var exceptionSlot in slotlist.GroupBy(x => x).Where(x => !x.Equals(SpellSlot.Unknown) && x.Count() > 5))
                                    text.Append($"{exceptionSlot} is more than 5 level");
                                }
                                else
                                {
                                    SpellSlot[] ordered = new SpellSlot[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, };
                                    ordered = ordered.OrderBy(x => slotlist.LastIndexOf(x)).ToArray();
                                    foreach (var spell in ordered)
                                    {
                                        text.Append($"{spell.ToString()}{(ordered.Last().Equals(spell) ? "." : " → ")}");
                                    }
                                    text.Append($" With ");
                                    for (int i = 1; i <= 3; i++)
                                    {
                                        text.Append($"Level {i} : {slotlist[i - 1]} {(i.Equals(3) ? ". " : "| ")}");
                                    }
                                }
                            }
                            break;
                        default:
                            {
                                if (slotlist[5].Equals(SpellSlot.R) && slotlist[10].Equals(SpellSlot.R) && slotlist[15].Equals(SpellSlot.R))
                                {
                                    int countQ = slotlist.Count(x => x.Equals(SpellSlot.Q));
                                    int countW = slotlist.Count(x => x.Equals(SpellSlot.W));
                                    int countE = slotlist.Count(x => x.Equals(SpellSlot.E));
                                    if (!countQ.Equals(5))
                                    {
                                        text.Append($"Q is {(countQ > 5 ? "more" : "less")} than 5");
                                        break;
                                    }
                                    if (!countW.Equals(5))
                                    {
                                        text.Append($"W is {(countW > 5 ? "more" : "less")} than 5");
                                        break;
                                    }
                                    if (!countE.Equals(5))
                                    {
                                        text.Append($"E is {(countE > 5 ? "more" : "less")} than 5");
                                        break;
                                    }
                                    text.Append($"R → ");
                                    SpellSlot[] ordered = new SpellSlot[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, };
                                    ordered = ordered.OrderBy(x => slotlist.LastIndexOf(x)).ToArray();
                                    foreach (var spell in ordered)
                                    {
                                        text.Append($"{spell.ToString()}{(ordered.Last().Equals(spell) ? "." : " → ")}");
                                    }
                                    text.Append($" With ");
                                    for (int i = 1; i <= 3; i++)
                                    {
                                        text.Append($"Level {i} : {slotlist[i - 1]} {(i.Equals(3) ? ". " : "| ")}");
                                    }
                                }
                                else
                                {
                                    text.Append($"Couldn't preview this profile. Check your option at lv 6 or 11 or 16");
                                }
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    text.Append($"Couldn't preview this profile");
                    Debug.Print(e.ToString(), Console_Message.Error);
                }
                return text.ToString();
            }
        }
    }
}
