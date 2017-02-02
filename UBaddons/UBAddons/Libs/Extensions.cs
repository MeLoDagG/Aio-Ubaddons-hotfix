using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System;
using System.Linq;
using SharpDX;
using Color = System.Drawing.Color;
using UBAddons.General;
using UBAddons.Log;

namespace UBAddons.Libs
{
    static class Extensions
    {
        #region Boolean
        public static bool IsOrb(this Orbwalker.ActiveModes mode)
        {
            return Orbwalker.ActiveModesFlags.HasFlag(mode);
        }
        public static bool IsUnderEnemyTurret(this Vector3 Pos)
        {
            return EntityManager.Turrets.AllTurrets.Any((Obj_AI_Turret turret) => Player.Instance.Team != turret.Team && Pos.Distance(turret) <= turret.GetAutoAttackRange() && turret.IsValid);
        }
        public static bool IsUnderEnemyTurret(this Vector2 Pos)
        {
            return Pos.To3DWorld().IsUnderEnemyTurret();
        }
        public static bool IsSafePosition(this Vector3 Pos)
        {
            return Pos.CountEnemyChampionsInRange(800) <= Pos.CountAllyChampionsInRange(800) && !Pos.IsUnderEnemyTurret();
        }
        public static bool HasQZileanBuff(this Obj_AI_Base target)
        {
            if (target.IsEnemy)
            {
                return target.HasBuff("ZileanQEnemyBomb");
            }
            else
            {
                return target.HasBuff("ZileanQAllyBomb");
            }
        }
        public static bool IsEscapeFrom(this Obj_AI_Base CheckUnit, Vector3 from)
        {
            var path = Prediction.Position.GetRealPath(CheckUnit);
            if (path == null)
            {
                return false;
            }
            else
            {
                if (CheckUnit.IsFacing(from) || path.First().Distance(from) < CheckUnit.Distance(from))
                {
                    return false;
                }
                if (!CheckUnit.IsFacing(from) && path.First().Distance(from) > CheckUnit.Distance(from))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsInMissileLine(this Obj_AI_Base target, MissileClient missile)
        {
            var rectangle = new Geometry.Polygon.Rectangle(missile.StartPosition, missile.EndPosition, missile.BoundingRadius);
            return rectangle.IsInside(target);
        }
        public static bool IsKillable(this Obj_AI_Base target, SpellSlot slot)
        {
            if (target == null)
            {
                return false;
            }
            return target.Health <= DamageIndicator.DamageDelegate(target, slot);
        }
        public static bool IsUdying(this Obj_AI_Base target)
        {
            return (target is AIHeroClient && ((AIHeroClient)target).HasUndyingBuff(true))
                || target.HasBuffOfType(BuffType.SpellImmunity);
        }
        public static bool IsImportant(this Obj_AI_Minion minion)
        {
            return minion.IsValidTarget()
                && (minion.Name.ToLower().Contains("baron")
                || minion.Name.ToLower().Contains("dragon")
                || minion.Name.ToLower().Contains("herald"));
        }
        public static bool IsSiegeMinion(this Obj_AI_Base minion)
        {
            return minion.BaseSkinName.Contains("Siege");
        }
        #endregion

        #region Creat Method
        public static string Begin = Variables.AddonName + "." + Player.Instance.Hero;
        public static void CreatDrawingOpening(this Menu menu)
        {
            menu.Add(Begin + ".EnableDraw", new CheckBox("Enable Drawing"));
        }
        public static void CreatLaneClearOpening(this Menu menu)
        {
            menu.Add(Begin + ".LaneClear.Killable.Only", new CheckBox("LaneClear only Killable", false));
            menu.Add(Begin + ".LaneClear.NoEnemy.Only", new CheckBox("LaneClear only no Enemy"));
            menu.Add(Begin + ".LaneClear.NoEnemy.Range", new Slider("Range detect {0}0", 100, 0, 150));
        }
        public static void CreatDangerValueBox(this Menu menu)
        {
            menu.Add(Begin + ".Interrupter.DangerValue", new ComboBox("Danger Value", 0, "High", "Medium", "Low"));
        }
        public static void CreatSlotHitChance(this Menu menu, SpellSlot slot, int defaultValue = 80)
        {
            menu.Add(Begin + "." + slot + ".HitChance", new Slider(slot + " hitchance prediction", defaultValue));
        }

        public static void CreatSlotCheckBox(this Menu menu, SpellSlot slot, string additional = null, bool defaultValue = true)
        {
            menu.Add(Begin + "." + slot + ".Enable." + additional, new CheckBox("Use " + slot, defaultValue));
        }

        public static void CreatSlotHitSlider(this Menu menu, SpellSlot slot, int defaultValue, int minvalue, int maxvalue)
        {
            menu.Add(Begin + "." + slot + ".Hit", new Slider("Use " + slot + " only hit {0}", defaultValue, minvalue, maxvalue));
        }

        public static void CreatSlotComboBox(this Menu menu, SpellSlot slot, int defaultValue = 0, params string[] logic)
        {
            menu.Add(Begin + "." + slot + ".LogicBox", new ComboBox(slot + " Logics", defaultValue, logic));
        }

        public static void CreatManaLimit(this Menu menu, bool IsEnergy = false, int defaultValue = 50)
        {
            string additional = IsEnergy ? "" : "%";
            menu.Add(Begin + ".ManaLimit", new Slider("Stop spells " + menu.DisplayName + " if my mana lower {0}" + additional, IsEnergy ? defaultValue * 2 : defaultValue, 0, IsEnergy ? 200 : 100));
        }

        public static void CreatHarassKeyBind(this Menu menu)
        {
            menu.Add(Begin + ".Key", new KeyBind("Auto Harass Key", false, KeyBind.BindTypes.PressToggle));
        }

        public static void CreatExplainAboutLasthit(this Menu menu)
        {
            menu.AddLabel("Farm mode contain: Harass, LaneClear, JungleClear, Lasthit");
        }

        public static void CreatMiscGapCloser(this Menu menu)
        {
            menu.Add(Begin + ".GapCloser", new CheckBox("Prevent idiot anti Gapcloser"));
        }

        public static void CreatColorPicker(this Menu menu, SpellSlot slot)
        {
            if (slot != SpellSlot.Unknown)
            {
                menu.AddGroupLabel(slot + "Draw Setting");
                menu.Add(Begin + "." + slot + ".EnableDraw", new CheckBox("Draw " + slot));
                menu.Add(Begin + "." + slot + ".OnlyReady", new CheckBox("Only Ready", false));
                menu.Add(Begin + "." + slot + ".Color", new ColorPicker.ColorPicker(slot + " Color", ColorPicker.ColorReader.Load(Begin + "." + slot + ".Color", new Random().NextColor().ToSystem())));
            }
            else
            {
                menu.AddGroupLabel("Damage Indicator");
                menu.Add(Begin + ".Damage.Enable", new CheckBox("Enable"));
                menu.Add(Begin + ".Damage.Color", new ColorPicker.ColorPicker("Damage Indicator Color", ColorPicker.ColorReader.Load(Begin + ".Damage.Color", Color.OrangeRed)));
            }
        }

        public static void CreatLasthitOpening(this Menu menu)
        {
            menu.Add(Begin + ".LastHit.Prevent.Combo", new CheckBox("Prevent while combo"));
            menu.Add(Begin + ".LastHit.Only.FarmMode", new CheckBox("Only in farm mode"));
            menu.CreatExplainAboutLasthit();
        }

        #endregion

        #region Get Method

        public static bool VChecked(this Menu menu, string id)
        {
            if (menu[id] != null)
            {
                return menu[id].Cast<CheckBox>().CurrentValue;
            }
            else
                Debug.Print("Check your menu: " + menu.DisplayName + " " + id + " Value you try to get is null", Console_Message.Error);
            return false;
        }

        public static bool VActived(this Menu menu, string id)
        {
            if (menu[id] != null)
            {
                return menu[id].Cast<KeyBind>().CurrentValue;
            }
            else
                Debug.Print("Check your menu: " + menu.DisplayName + " " + id + " Value you try to get is null", Console_Message.Error);
            return false;
        }

        public static int VSliderValue(this Menu menu, string id)
        {
            if (menu[id] != null)
            {
                return menu[id].Cast<Slider>().CurrentValue;
            }
            else
                Debug.Print("Check your menu: " + menu.DisplayName + " " + id + " Value you try to get is null", Console_Message.Error);
            return -1;

        }

        public static int VComboValue(this Menu menu, string id)
        {
            if (menu[id] != null)
            {
                return menu[id].Cast<ComboBox>().CurrentValue;
            }
            else
                Debug.Print("Check your menu: " + menu.DisplayName + " " + id + " Value you try to get is null", Console_Message.Error);
            return -1;
        }

        public static bool GetKillableOnly(this Menu menu)
        {
            if (menu[Begin + ".LaneClear.Killable.Only"] != null)
            {
                return menu[Begin + ".LaneClear.Killable.Only"].Cast<CheckBox>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".LaneClear.Killable.Only is null", Console_Message.Error);
            return false;
        }
        public static bool GetNoEnemyOnly(this Menu menu)
        {
            if (menu[Begin + ".LaneClear.NoEnemy.Only"] != null)
            {
                return menu[Begin + ".LaneClear.NoEnemy.Only"].Cast<CheckBox>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".LaneClear.NoEnemy.Only is null", Console_Message.Error);
            return false;
        }
        public static int GetDetectRange(this Menu menu)
        {
            if (menu[Begin + ".LaneClear.NoEnemy.Range"] != null)
            {
                return menu[Begin + ".LaneClear.NoEnemy.Range"].Cast<Slider>().CurrentValue * 10;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".LaneClear.NoEnemy.Range is null", Console_Message.Error);
            return -1;
        }
        public static bool GetHarassKeyBind(this Menu menu)
        {
            if (menu[Begin + ".Key"] != null)
            {
                return menu[Begin + ".Key"].Cast<KeyBind>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".Key", Console_Message.Error);
            return false;
        }
        public static int GetSlotHitChance(this Menu menu, SpellSlot slot)
        {
            if (menu[Begin + "." + slot + ".HitChance"] != null)
            {
                return menu[Begin + "." + slot + ".HitChance"].Cast<Slider>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + "." + slot + ".HitChance is null", Console_Message.Error);
            return -1;
        }

        public static bool GetSlotCheckBox(this Menu menu, SpellSlot slot, string additional = null)
        {
            if (menu[Begin + "." + slot + ".Enable." + additional] != null)
            {
                return menu[Begin + "." + slot + ".Enable." + additional].Cast<CheckBox>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + "." + slot + ".Enable." + additional + " is null", Console_Message.Error);
            return false;
        }

        public static int GetSlotHitSlider(this Menu menu, SpellSlot slot)
        {
            if (menu[Begin + "." + slot + ".Hit"] != null)
            {
                return menu[Begin + "." + slot + ".Hit"].Cast<Slider>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + "." + slot + ".Hit is null", Console_Message.Error);
            return -1;
        }

        public static int GetSlotComboBox(this Menu menu, SpellSlot slot)
        {
            if (menu[Begin + "." + slot + ".LogicBox"] != null)
            {
                return menu[Begin + "." + slot + ".LogicBox"].Cast<ComboBox>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + "." + slot + ".LogicBox is null", Console_Message.Error);
            return -1;
        }

        public static int GetManaLimit(this Menu menu)
        {
            if (menu[Begin + ".ManaLimit"] != null)
            {
                return menu[Begin + ".ManaLimit"].Cast<Slider>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".ManaLimit is null", Console_Message.Error);
            return -1;
        }

        public static bool GetDrawCheckValue(this Menu menu, SpellSlot slot)
        {
            if (slot != SpellSlot.Unknown)
            {
                return menu[Begin + "." + slot + ".EnableDraw"].Cast<CheckBox>().CurrentValue;
            }
            else
            {
                return menu[Begin + ".Damage.Enable"].Cast<CheckBox>().CurrentValue;
            }
        }
        public static bool GetOnlyReady(this Menu menu, SpellSlot slot)
        {
            if (slot != SpellSlot.Unknown)
            {
                return menu[Begin + "." + slot + ".OnlyReady"].Cast<CheckBox>().CurrentValue;
            }
            else return false;
        }
        public static Color GetColorPicker(this Menu menu, SpellSlot slot)
        {
            if (slot != SpellSlot.Unknown)
            {
                return menu[Begin + "." + slot + ".Color"].Cast<ColorPicker.ColorPicker>().CurrentValue;
            }
            else
            {
                return menu[Begin + ".Damage.Color"].Cast<ColorPicker.ColorPicker>().CurrentValue;
            }
        }

        public static bool PreventIdiotAntiGap(this Menu menu)
        {
            if (menu[Begin + ".GapCloser"] != null)
            {
                return menu[Begin + ".GapCloser"].Cast<CheckBox>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".GapCloser is null", Console_Message.Error);
            return false;
        }

        public static DangerLevel[] GetDangerValue(this Menu menu)
        {
            if (menu[Begin + ".Interrupter.DangerValue"] != null)
            {
                switch (menu[Begin + ".Interrupter.DangerValue"].Cast<ComboBox>().CurrentValue)
                {
                    case 0:
                        return new[] { DangerLevel.High };
                    case 1:
                        return new[] { DangerLevel.High, DangerLevel.Medium };
                    case 2:
                        return new[] { DangerLevel.High, DangerLevel.Medium, DangerLevel.Low };
                    default:
                        return null;
                }
            }
            else Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".Interrupter.DangerValue is null", Console_Message.Error);
            return null;

        }

        public static bool PreventCombo(this Menu menu)
        {
            if (menu[Begin + ".LastHit.Prevent.Combo"] != null)
            {
                return menu[Begin + ".LastHit.Prevent.Combo"].Cast<CheckBox>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".LastHit.Prevent.Combo is null", Console_Message.Error);
            return false;
        }
        public static bool OnlyFarmMode(this Menu menu)
        {
            if (menu[Begin + ".LastHit.Only.FarmMode"] != null)
            {
                return menu[Begin + ".LastHit.Only.FarmMode"].Cast<CheckBox>().CurrentValue;
            }
            else
                Debug.Print("Menu " + menu.DisplayName + " " + Begin + ".LastHit.Only.FarmMode is null", Console_Message.Error);
            return false;
        }

        #endregion

        #region Get Target
        public static AIHeroClient GetKillableTarget(this Spell.SpellBase spell)
        {
            return TargetSelector.GetTarget(
                EntityManager.Heroes.Enemies.Where(t => t != null
                && t.IsValidTarget()
                && !t.HasUndyingBuff(true)
                && !t.HasBuffOfType(BuffType.SpellImmunity)
                && spell.IsInRange(t)
                && t.Health <= DamageIndicator.DamageDelegate(t, spell.Slot)), spell.DamageType);
        }
        public static AIHeroClient GetTarget(this Spell.SpellBase spell, IEnumerable<AIHeroClient> YourTarget, TargetSeclect ifnull = TargetSeclect.LeastHP)
        {
            var correct = YourTarget.Where(x => x.IsValidTarget(spell.Range, false, spell.RangeCheckSource));
            var lastTarget = TargetSelector.GetTarget(correct, spell.DamageType);
            if (lastTarget == null)
            {
                switch (ifnull)
                {
                    case TargetSeclect.SpellTarget:
                        {
                            return spell.GetTarget();
                        };
                    case TargetSeclect.LeastHP:
                        {
                            return EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(spell.Range, false, spell.RangeCheckSource)).OrderBy(x => x.Health).FirstOrDefault();
                        };
                    case TargetSeclect.Default:
                        {
                            return lastTarget;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ifnull));
                }
            }
            return lastTarget;
        }
        public static AIHeroClient GetTarget(this Spell.Chargeable spell, IEnumerable<AIHeroClient> YourTarget, TargetSeclect ifnull = TargetSeclect.LeastHP)
        {
            if (spell.IsCharging)
            {
                var correct = YourTarget.Where(x => x.IsValidTarget(spell.Range, false, spell.RangeCheckSource));
                var lastTarget = TargetSelector.GetTarget(correct, spell.DamageType);
                if (lastTarget == null)
                {
                    switch (ifnull)
                    {
                        case TargetSeclect.SpellTarget:
                            {
                                return spell.GetTarget();
                            };
                        case TargetSeclect.LeastHP:
                            {
                                return EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(spell.Range, false, spell.RangeCheckSource)).OrderBy(x => x.Health).FirstOrDefault();
                            };
                        case TargetSeclect.Default:
                            {
                                return lastTarget;
                            };
                        default:
                            throw new ArgumentOutOfRangeException(nameof(ifnull));
                    }
                }
                else
                {
                    return lastTarget;
                }
            }
            else
            {
                var correct = YourTarget.Where(x => x.IsValidTarget(spell.MaximumRange, false, spell.RangeCheckSource));
                var lastTarget = TargetSelector.GetTarget(correct, spell.DamageType);
                if (lastTarget == null)
                {
                    switch (ifnull)
                    {
                        case TargetSeclect.SpellTarget:
                            {
                                return spell.GetTarget();
                            };
                        case TargetSeclect.LeastHP:
                            {
                                return EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(spell.MaximumRange, false, spell.RangeCheckSource)).OrderBy(x => x.Health).FirstOrDefault();
                            };
                        case TargetSeclect.Default:
                            {
                                return lastTarget;
                            };
                        default:
                            throw new ArgumentOutOfRangeException(nameof(ifnull));
                    }
                }
                else
                {
                    return lastTarget;
                }
            }
        }
        public static AIHeroClient GetTarget(this Spell.SpellBase spell, int additionalRange = 50)
        {
            return TargetSelector.GetTarget(spell.Range + additionalRange, spell.DamageType);
        }
        public static IEnumerable<Obj_AI_Minion> GetJungleMobs(this Spell.SpellBase spell)
        {
            var sourcePos = spell.RangeCheckSource;
            var JungleMobs = EntityManager.MinionsAndMonsters.GetJungleMonsters();
            #warning Add Xerath in here
            string[] AllowedMonsters = { "SRU_Gromp", "SRU_Blue", "SRU_Red", "SRU_Razorbeak", "SRU_Krug", "SRU_Murkwolf","SRU_Crab",
            "SRU_RiftHerald", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Earth", "SRU_Dragon_Air", "SRU_Dragon_Elder", "SRU_Baron",
                "TT_NGolem", "TT_NWraith", "TT_NWolf", "TT_Spiderboss", ""};
            if (JungleMobs.Any(x => AllowedMonsters.Contains(x.BaseSkinName)))
            {
                return JungleMobs.Where(x => AllowedMonsters.Contains(x.BaseSkinName) && x.IsValidTarget(spell.Range, false, sourcePos)).OrderBy(x => x.Distance(Player.Instance));
            }
            else
            {
                return Enumerable.Empty<Obj_AI_Minion>();
            }
        }
        public static IEnumerable<Obj_AI_Minion> GetLaneMinions(this Spell.SpellBase spell, bool IsOnlyKillable = false)
        {
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy);
            if (!minion.Any() || minion == null)
            {
                return Enumerable.Empty<Obj_AI_Minion>();
            }
            if (IsOnlyKillable)
            {
                return minion.Where(x => x.IsValidTarget(spell.Range, false, spell.RangeCheckSource) && x.Health <= DamageIndicator.DamageDelegate(x, spell.Slot)).ToList();
            }
            return minion.Where(x => x.IsValidTarget(spell.Range, false, spell.RangeCheckSource));
        }
        #endregion

        #region Prediction Check
        public static bool CanNext(this PredictionResult pred, Spell.Skillshot spell, int percentHit, bool ignoreYasuoWall)
        {
            if (Player.Instance.Hero.Equals(Champion.Ryze) && spell.Slot.Equals(SpellSlot.Q))
            {
                if ((pred.CollisionObjects.Any() && pred.CollisionObjects.First().HasBuff("RyzeE")) || !pred.GetCollisionObjects<Obj_AI_Minion>().Any(x => x.IsEnemy) || pred.CollisionObjects.Any(x => x.IsValidTarget() && x.IsEnemy && x is AIHeroClient))
                {
                    return spell.IsReady()
                        && spell.IsInRange(pred.CastPosition)
                        && pred.HitChancePercent >= percentHit
                        && (Prediction.Position.Collision.GetYasuoWallCollision(spell.SourcePosition.GetValueOrDefault(Player.Instance.Position), pred.CastPosition) == Vector3.Zero || ignoreYasuoWall);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return spell.IsReady()
                    && spell.IsInRange(pred.CastPosition)
                    && pred.HitChancePercent >= percentHit
                    && (Prediction.Position.Collision.GetYasuoWallCollision(spell.SourcePosition.GetValueOrDefault(Player.Instance.Position), pred.CastPosition) == Vector3.Zero || ignoreYasuoWall);
            }
        }
        public static bool CanNext(this PredictionResult pred, Spell.Chargeable spell, int percentHit, bool ignoreYasuoWall)
        {
            return (spell.IsReady() || spell.IsCharging)
                && spell.Range != spell.MaximumRange ? Player.Instance.Position.IsInRange(pred.CastPosition, spell.Range - 150) : spell.IsInRange(pred.CastPosition)
                && pred.HitChancePercent >= percentHit
                && (Prediction.Position.Collision.GetYasuoWallCollision(spell.SourcePosition.GetValueOrDefault(Player.Instance.Position), pred.CastPosition) == Vector3.Zero || ignoreYasuoWall);
        }
        #endregion

        #region Misc float
        public static float FixToCappedMovementSpeed(this float RawMovementSpeed)
        {
            if (RawMovementSpeed <= 415)
            {
                return RawMovementSpeed;
            }
            else if (RawMovementSpeed <= 490)
            {
                return 415 + (RawMovementSpeed - 415) * 0.8f;
            }
            else
            {
                return 415 + 60 + (RawMovementSpeed - 490) * 0.5f;
            }
        }
        public static float GetDamage(this MissileClient missile)
        {
            if (missile == null)
            {
                return 0;
            }
            else
            {
                if (missile.Target == null || missile.SpellCaster == null)
                {
                    return 0;
                }
                else
                {
                    if (missile.Team.Equals(missile.Target.Team))
                    {
                        return 0;
                    }
                    else
                    {
                        var damage = missile.SpellCaster.TotalAttackDamage;
                        if (missile.SData.Name.Contains("Crit") && missile.SpellCaster is AIHeroClient)
                        {
                            damage += missile.SpellCaster.TotalAttackDamage * ((AIHeroClient)missile.SpellCaster).GetCriticalStrikePercentMod();
                        }
                        if (missile.SData.Name.Contains("Passive"))
                        {
                            damage += missile.SpellCaster.TotalAttackDamage; // There is not solution, but I too lazy to get all passive buff
                        }
                        return damage;
                    }
                }
            }
        }
        public static float PercentPhysicalLifeStealMod(this AIHeroClient hero, bool IsAOESpell = false)
        {
            var result = 0f;
            foreach (var item in hero.InventoryItems)
            {
                if (item.Id.Equals(ItemId.Dorans_Blade))
                {
                    result += 3f;
                }
                if (item.Id.Equals(ItemId.Guardians_Hammer) || item.Id.Equals(ItemId.Bilgewater_Cutlass) || item.Id.Equals(ItemId.Blade_of_the_Ruined_King)
                    || item.Id.Equals(ItemId.Mercurial_Scimitar) || item.Id.Equals(ItemId.Vampiric_Scepter) || item.Id.Equals(ItemId.Vampiric_Scepter))
                {
                    result += 10f;
                }
                if (item.Id.Equals(ItemId.Ravenous_Hydra))
                {
                    result += 12f;
                }
                if (item.Id.Equals(ItemId.Deaths_Dance) || item.Id.Equals(ItemId.Hextech_Gunblade))
                {
                    result += (IsAOESpell ? 5f : 15f);
                }
            }
            if (hero.InventoryItems.Any(x => x.Id.Equals(ItemId.The_Bloodthirster)))
            {
                result += 20f;
            }
            if (hero.InventoryItems.Any(x => x.Id.Equals(ItemId.Spirit_Visage)))
            {
                result *= 1.25f;
            }
            return result;
        }
        public static float PercentSpellLifeStealMod(this AIHeroClient hero, bool IsAOESpell = false)
        {
            var result = 0f;
            foreach (var item in hero.InventoryItems)
            {
                if (item.Id.Equals(ItemId.Hextech_Gunblade))
                {
                    result += (IsAOESpell ? 5f : 15f);
                }
            }
            if (hero.InventoryItems.Any(x => x.Id.Equals(ItemId.Spirit_Visage)))
            {
                result *= 1.25f;
            }
            return result;
        }
        public static float Abs(this Vector3 vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }
        public static float AngleBetween(this Vector3 start, Vector3 to, bool returndegree = true)
        {
            float num1 = Vector3.Dot(start, to);
            float num2 = start.Abs() * to.Abs();
            var num3 = num1 / num2;
            return returndegree ? (float)(Math.Acos(num3) / Math.PI * 180) : (float)Math.Acos(num3);
        }
        public static float CosAngleBetween(this Vector3 start, Vector3 to)
        {
            float num1 = Vector3.Dot(start, to);
            float num2 = start.Abs() * to.Abs();
            return num1 / num2;
        }
        #endregion

        #region Count
        public static int Count_Monster_In_Range(this GameObject Object, float range)
        {
            return EntityManager.MinionsAndMonsters.GetJungleMonsters(Object.Position, range).Count();
        }
        public static int Count_Monster_In_Range(this Vector3 Position, float range)
        {
            return EntityManager.MinionsAndMonsters.GetJungleMonsters(Position, range).Count();
        }
        public static int Count_Monster_In_Range(this Vector2 Position, float range)
        {
            return EntityManager.MinionsAndMonsters.GetJungleMonsters(Position.To3DWorld(), range).Count();
        }
        public static int IndexOf<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            int i = 0;
            foreach (var pair in dictionary)
            {
                if (pair.Key.Equals(key))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        #endregion

        #region Vector Method
        public static Vector3 TrueDirection(this Obj_AI_Base target, int Range = 500)
        {
            Vector2 directionNormalized = target.Direction().Normalized();
            return (target.Position.To2D() + directionNormalized * Range).To3DWorld();
        }
        public static Vector3 EndPosition(this Vector3 start, Vector3 center)
        {
            return start.Extend(center, start.Distance(center) * 2).To3DWorld();
        }
        public static Vector3[] Symmetry(this Vector3 center, Vector3 direction, float Range = 0)
        {
            if (Range.Equals(0))
            {
                Range = center.Distance(direction);
            }
            Vector3 pos1 = center.Extend(direction, Range).To3DWorld();
            Vector3 pos2 = center.Extend(direction, -Range).To3DWorld();
            return new Vector3[] { pos1, pos2};
        }
        public static Vector3 Home(this Obj_AI_Base target)
        {
            var objSpawnPoint = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.Team.Equals(target.Team));
            return objSpawnPoint?.Position ?? Vector3.Zero;
        }
        #endregion
    }
}
