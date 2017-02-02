using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;
using UBAddons.Libs;
using UBAddons.General;

namespace UBAddons.UBCore.Activator
{
    class AttackItem
    {
        internal static void Tiamat()
        {
            if (!ItemList.Tiamat.Any(x => x.IsOwned() && x.IsReady()) || Main.AttackMenu["Tiamat"].Cast<GroupLabel>() == null) return;
            var tiamat = ItemList.Tiamat.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (Player.HasBuffOfType(BuffType.Invisibility) && Main.AttackMenu.VChecked("Tiamat.Stealth")) return;
            if (Orbwalker.ActiveModes.LaneClear.IsOrb() || Orbwalker.ActiveModes.JungleClear.IsOrb())
            {
                var Count = Math.Max(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 400).Count(), EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, 400).Count());
                if (Count >= Main.AttackMenu.VSliderValue("Tiamat.Clear.Hit"))
                {
                    tiamat.Cast();
                }
            }
            if (tiamat.Id.Equals(ItemId.Titanic_Hydra) || !Main.AttackMenu.VChecked("Tiamat.Enabled") || Main.AttackMenu.VComboValue("Tiamat.Style").Equals(0)) return;
            var target = TargetSelector.GetTarget(400, DamageType.Physical);
            if (!Orbwalker.ActiveModes.Combo.IsOrb() && Main.AttackMenu.VChecked("Tiamat.Combo") && target.Health > ItemDamage.TiamatDamage(target)) return;
            tiamat.Cast();
        }
        internal static void Bork()
        {
            if (!ItemList.Bork.Any(x => x.IsOwned() && x.IsReady()) || Main.AttackMenu["Bork"].Cast<GroupLabel>() == null) return;
            var bork = ItemList.Bork.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (!Main.AttackMenu.VChecked("Bork.Enabled")) return;
            if (Player.HasBuffOfType(BuffType.Invisibility) && Main.AttackMenu.VChecked("Bork.Stealth")) return;
            if (Player.Instance.HealthPercent > Main.AttackMenu.VSliderValue("Bork.MyHP")) return;
            var target = TargetSelector.GetTarget(550, DamageType.Physical);
            if (!Orbwalker.ActiveModes.Combo.IsOrb() && Main.AttackMenu.VChecked("Bork.Combo") && target.Health > ItemDamage.BladeDamage(target)) return;
            if (target.HealthPercent > Main.AttackMenu.VSliderValue("Bork.EnemyHP")) return;
            bork.Cast(target);
        }
        internal static void Hextech()
        {
            if (!ItemList.Hextech.Any(x => x.IsOwned() && x.IsReady()) || Main.AttackMenu["Hextech"].Cast<GroupLabel>() == null) return;
            var Hextech = ItemList.Hextech.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (!Main.AttackMenu.VChecked("Hextech.Enabled")) return;
            if (Player.HasBuffOfType(BuffType.Invisibility) && Main.AttackMenu.VChecked("Hextech.Stealth")) return;
            var target = TargetSelector.GetTarget(700, DamageType.Physical);
            var hextech = Hextech.Id.Equals(ItemId.Hextech_GLP_800) ? HextechItem.Ice : Hextech.Id.Equals(ItemId.Hextech_Gunblade) ? HextechItem.Gun : Hextech.Id.Equals(ItemId.Hextech_Protobelt_01) ? HextechItem.Fire : HextechItem.Fire;
            if (!Orbwalker.ActiveModes.Combo.IsOrb() && Main.AttackMenu.VChecked("Hextech.Combo") && target.Health > ItemDamage.HextechDamage(target, hextech)) return;
            Hextech.Cast(target);
        }
        internal static void OnPostAttack(AttackableUnit target, EventArgs args)
        {
            var Target = target as AIHeroClient;
            if (Target == null || !Target.IsValidTarget()) return;
            if (!ItemList.Tiamat.Any(x => x.IsOwned() && x.IsReady())) return;
            var tiamat = ItemList.Tiamat.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (Player.HasBuffOfType(BuffType.Invisibility) && Main.AttackMenu.VChecked("Tiamat.Stealth")) return;
            if (tiamat.Id.Equals(ItemId.Titanic_Hydra))
            {
                if (Main.AttackMenu["Titanic_Hydra"].Cast<GroupLabel>() != null && Main.AttackMenu.VComboValue("Titanic_Hydra.Style").Equals(0))
                {
                    if (!Orbwalker.ActiveModes.Combo.IsOrb() && Main.AttackMenu.VChecked("Titanic_Hydra.Combo") && target.Health > ItemDamage.TitanicDamage(Target)) return;
                    tiamat.Cast();
                    Orbwalker.ResetAutoAttack();
                }
            }
            else
            {
                if (Main.AttackMenu["Tiamat"].Cast<GroupLabel>() != null && Main.AttackMenu.VComboValue("Tiamat.Style").Equals(0))
                {
                    if (!Orbwalker.ActiveModes.Combo.IsOrb() && Main.AttackMenu.VChecked("Tiamat.Combo") && target.Health > ItemDamage.TiamatDamage(Target)) return;
                    tiamat.Cast();
                }
            }
        }
        internal static void OnPreAttack(AttackableUnit target, EventArgs args)
        {
            var Target = target as AIHeroClient;
            if (Target == null || !Target.IsValidTarget()) return;
            if (!ItemList.Tiamat.Any(x => x.IsOwned() && x.IsReady())) return;
            var tiamat = ItemList.Tiamat.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (Player.HasBuffOfType(BuffType.Invisibility) && Main.AttackMenu.VChecked("Tiamat.Stealth")) return;
            if (tiamat.Id.Equals(ItemId.Titanic_Hydra))
            {
                if (Main.AttackMenu["Titanic_Hydra"].Cast<GroupLabel>() != null && Main.AttackMenu.VComboValue("Titanic_Hydra.Style").Equals(0))
                {
                    if (!Orbwalker.ActiveModes.Combo.IsOrb() && Main.AttackMenu.VChecked("Titanic_Hydra.Combo") && target.Health > ItemDamage.TitanicDamage(Target)) return;
                    tiamat.Cast();
                    Orbwalker.ResetAutoAttack();
                }
            }
        }

        internal static void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!ItemList.Hextech.Any(x => x.IsOwned() && x.IsReady()) || Main.AttackMenu["Hextech"].Cast<GroupLabel>() == null || sender.IsAlly) return;
            var Hextech = ItemList.Hextech.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (!Main.AttackMenu.VChecked("Hextech.Enabled") || !Main.AttackMenu.VChecked("Hextech.AntiGapcloser")) return;
            if (Hextech.Id.Equals(ItemId.Hextech_GLP_800))
            {
                Hextech.Cast(args.End);
            }
            if (Hextech.Id.Equals(ItemId.Hextech_Gunblade))
            {
                Hextech.Cast(sender);
            }
            if (Hextech.Id.Equals(ItemId.Hextech_Protobelt_01))
            {
                Hextech.Cast(args.End.Extend(Player.Instance.Position, args.End.Distance(Player.Instance) + 275).To3DWorld());
            }
        }
    }
}
