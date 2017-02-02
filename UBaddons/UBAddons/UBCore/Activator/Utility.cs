using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBAddons.Libs;
using UBAddons.General;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;

namespace UBAddons.UBCore.Activator
{
    class Utility
    {
        public static float LastStack;
        public static bool Bought;
        public static bool IsReady
        {
            get { return ItemList.Stack.Any(i => i.IsOwned() && Game.Time - LastStack > 1.5f); }
        }

        internal static void Stack()
        {
            if (!IsReady) return;
            if (!Player.Instance.IsInShopRange() || Main.UtilityMenu.Get<CheckBox>("Stack") == null || !Main.UtilityMenu.VChecked("Stack")) return;
            var Q = new Spell.Skillshot(SpellSlot.Q, 500, SkillShotType.Linear);
            var E = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Linear);
            var W = new Spell.Skillshot(SpellSlot.E, 500, SkillShotType.Linear);
            var R = new Spell.Skillshot(SpellSlot.R, 300, SkillShotType.Circular);
            #region Champion
            switch (Player.Instance.ChampionName)
            {
                case "Ahri":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W);
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Anivia":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && R.IsReady())
                        {
                            Player.CastSpell(SpellSlot.R, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Annie":
                    {
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E);
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Aurelion Sol":
                    {
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W);
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Azir":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W);
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E);
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Brand":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Cassiopeia":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "ChoGath":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Corki":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E);
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Diana":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W);
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E);
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Ekko":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Ezreal":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Karthus":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E);
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Kassadin":
                    {
                        if (IsReady && W.IsReady())
                        {
                            Player.CastSpell(SpellSlot.W);
                            LastStack = Game.Time;
                        }
                        if (IsReady && E.IsReady())
                        {
                            Player.CastSpell(SpellSlot.E, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                        if (IsReady && R.IsReady())
                        {
                            Player.CastSpell(SpellSlot.R, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                case "Ryze":
                    {
                        if (IsReady && Q.IsReady())
                        {
                            Player.CastSpell(SpellSlot.Q, Player.Instance.Position.Extend(Game.CursorPos, 200).To3DWorld());
                            LastStack = Game.Time;
                        }
                    }
                    break;
                default:
                    break;
            }
            #endregion
        }
        internal static void ChangeTrinket()
        {
            if (Main.Activator == null || Main.UtilityMenu.VComboValue("Trinket" + Player.Instance.ChampionName).Equals(0) || Player.Instance.Level < 9
                || !Player.Instance.IsInShopRange() || Bought || !Shop.CanShop || !Game.MapId.Equals(GameMapId.SummonersRift)) return;
            switch (Main.UtilityMenu.VComboValue("Trinket" + Player.Instance.ChampionName))
            {
                case 1:
                    {
                        if (Shop.BuyItem(ItemId.Farsight_Alteration))
                        {
                            Bought = true;
                        }
                    }
                    break;
                case 2:
                    {
                        if (Shop.BuyItem(ItemId.Oracle_Alteration))
                        {
                            Bought = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        internal static void Utility_Usage()
        {
            if (!ItemList.Utility.Any(x => x.IsOwned() && x.IsReady())) return;
            var item = ItemList.Utility.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (item.Id.Equals(ItemId.Youmuus_Ghostblade))
            {
                if (!Orbwalker.ActiveModes.Combo.IsOrb() || !Main.UtilityMenu.VChecked(item.Id + ".Enabled")) return;
                if (Player.Instance.CountEnemyChampionsInRange(1000) < float.Epsilon || Player.Instance.CountAllyChampionsInRange(1000) < 2) return;
                item.Cast();
            }
            else
            {
                if ((!Orbwalker.ActiveModes.Combo.IsOrb() && Main.UtilityMenu.VChecked(item.Id + ".Combo"))|| !Main.UtilityMenu.VChecked(item.Id + ".Enabled")) return;
                var spell = new Spell.Skillshot(item.Slots.Last(), 5500, SkillShotType.Circular, 1000, int.MaxValue, 300, DamageType.True)
                {
                    AllowedCollisionCount = int.MaxValue,
                };
                var castPos = spell.GetBestCircularCastPosition(EntityManager.Heroes.Allies, Main.UtilityMenu.VSliderValue(item.Id + ".Percent"));
                var castPos2 = spell.GetBestCircularCastPosition(EntityManager.Heroes.Enemies, Main.UtilityMenu.VSliderValue(item.Id + ".Percent"));
                if (castPos.HitNumber > Main.UtilityMenu.VSliderValue(item.Id + ".Hit.Enemy"))
                {
                    item.Cast(castPos.CastPosition);
                }
                if (castPos2.HitNumber > Main.UtilityMenu.VSliderValue(item.Id + ".Hit.Ally"))
                {
                    item.Cast(castPos2.CastPosition);
                }
            }
        }
        internal static void OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!args.Target.IsMe || Player.Instance.IsInShopRange()) return;
            if (Player.Instance.HasBuff("RegenerationPotion")
                || Player.Instance.HasBuff("ItemMiniRegenPotion")
                || Player.Instance.HasBuff("ItemCrystalFlask")
                || Player.Instance.HasBuff("ItemDarkCrystalFlask")
                || Player.Instance.HasBuff("ItemCrystalFlaskJungle"))
                return;
            if (!ItemList.Potions.Any(x => x.IsOwned() && x.IsReady())) return;
            var potions = ItemList.Potions.FirstOrDefault(x => x.IsOwned() && x.IsReady());
            if (Main.Potions.Get<CheckBox>(potions.Id.ToString()) == null ||!Main.Potions.VChecked(potions.Id.ToString()) || Main.Potions.VSliderValue(potions.Id + ".HP") > Player.Instance.HealthPercent) return;
            if (potions.Id.Equals(ItemId.Hunters_Potion) && Player.Instance.Mana + 35 >= Player.Instance.MaxMana && Main.Potions.VChecked(potions.Id + ".MP") && !Variables.IsNomana) return;
            if (potions.Id.Equals(ItemId.Corrupting_Potion) && Player.Instance.Mana + 35 >= Player.Instance.MaxMana && Main.Potions.VChecked(potions.Id + ".MP") && !Variables.IsNomana) return;
            potions.Cast();
        }
    }
}
