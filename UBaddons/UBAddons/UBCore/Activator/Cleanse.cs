using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using UBAddons.Libs;

namespace UBAddons.UBCore.Activator
{
    class Cleanse
    {
        internal static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!(sender is AIHeroClient) || !sender.IsAlly || !args.Buff.Caster.IsEnemy) return;
            var Sender = sender as AIHeroClient;
            if (Sender.IsMe)
            {
                if (Spells.Cleanse != null && Spells.Cleanse.IsReady())
                {
                    if (Main.Clean["Cleanse"].Cast<GroupLabel>() == null || !Main.Clean.VChecked("Cleanse.Enabled")) return;
                    if (Main.Clean["Cleanse." + args.Buff.Type].Cast<CheckBox>() == null || !Main.Clean.VChecked("Cleanse." + args.Buff.Type)) return;
                    var Duration = (args.Buff.EndTime - Game.Time) * 1000;
                    if (Duration < Main.Clean.VSliderValue("Cleanse.Duration") * 50) return;
                    if (sender.CountEnemyChampionsInRange(1500) < float.Epsilon) return;
                    var Delay = new Random().Next(Main.Clean.VSliderValue("Cleanse.Delay.Min"), Main.Clean.VSliderValue("Cleanse.Delay.Max"));
                    Core.DelayAction(() =>
                    {
                        Spells.Cleanse.Cast();
                    }, Delay);
                }
                else
                {
                    if (!ItemList.Clean.Any(x => x.IsOwned() && x.IsReady())) return;
                    var qss = ItemList.Clean.FirstOrDefault(x => x.IsOwned() && x.IsReady());
                    string local = qss.Id.Equals(ItemId.Mikaels_Crucible) ? qss.Id.ToString() : "QSS";
                    if (Main.Clean[local].Cast<GroupLabel>() == null || !Main.Clean.VChecked(local + ".Enabled")
                        || !Main.Clean.VChecked(local + ".Enabled." + Sender.ChampionName)) return;
                    if (Main.Clean[local + "." + args.Buff.Type].Cast<CheckBox>() == null || !Main.Clean.VChecked(local + "." + args.Buff.Type)) return;
                    var Duration = (args.Buff.EndTime - Game.Time) * 1000;
                    if (Duration < Main.Clean.VSliderValue(local + ".Duration") * 50) return;
                    if (sender.CountEnemyChampionsInRange(1500) < float.Epsilon) return;
                    var Delay = new Random().Next(Main.Clean.VSliderValue(local + ".Delay.Min"), Main.Clean.VSliderValue(local + ".Delay.Max"));
                    Core.DelayAction(() =>
                    {
                        if (qss.Id.Equals(ItemId.Mikaels_Crucible))
                        {
                            qss.Cast(Sender);
                        }
                        else
                        {
                            qss.Cast();
                        }
                    }, Delay);
                }
            }
            else
            {
                var mikael = ItemList.Clean.FirstOrDefault(x => x.Id.Equals(ItemId.Mikaels_Crucible));
                if (!mikael.IsOwned() || !mikael.IsReady()) return;
                if (Main.Clean[mikael.Id.ToString()].Cast<GroupLabel>() == null || !Main.Clean.VChecked(mikael.Id + ".Enabled") 
                    || !Main.Clean.VChecked(mikael.Id + ".Enabled." + Sender.ChampionName)) return;
                if (Main.Clean[mikael.Id + "." + args.Buff.Type].Cast<CheckBox>() == null || !Main.Clean.VChecked(mikael.Id + "." + args.Buff.Type)) return;
                var Duration = (args.Buff.EndTime - Game.Time) * 1000;
                if (Duration < Main.Clean.VSliderValue(mikael.Id + ".Duration") * 50) return;
                if (sender.CountEnemyChampionsInRange(1500) < float.Epsilon) return;
                var Delay = new Random().Next(Main.Clean.VSliderValue(mikael.Id + ".Delay.Min"), Main.Clean.VSliderValue(mikael.Id + ".Delay.Max"));
                Core.DelayAction(() => mikael.Cast(Sender), Delay);
            }
        }
    }
}
