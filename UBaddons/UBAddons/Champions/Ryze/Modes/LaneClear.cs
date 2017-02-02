using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ryze.Modes
{
    class LaneClear : Ryze
    {
        public static void Execute()
        {
            if (player.Mana < MenuValue.LaneClear.ManaLimit) return;
            if (ObjectManager.Get<AIHeroClient>().Any(x => x.IsValid && !x.IsDead && !x.IsZombie && player.IsInRange(x, MenuValue.LaneClear.ScanRange)
                && MenuValue.LaneClear.EnableIfNoEnemies)) return;            
            if (MenuValue.LaneClear.UseQ && Q.IsReady())
            {
                var minion = Q.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    var count = minion.Count(x => x.HasBuff("RyzeE"));
                    if (count >= MenuValue.LaneClear.QHit)
                    {
                        Q.Cast(minion.Where(x => x.HasBuff("RyzeE")).First());
                    }
                    if (count == 0 && MenuValue.LaneClear.QHit == 1)
                    {
                        Q.Cast(minion.First());
                    }
                }
            }
            if (MenuValue.LaneClear.UseW && W.IsReady())
            {
                var minion = W.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    var hasE = minion.Where(x => x.HasBuff("RyzeE") && x.Health < HandleDamageIndicator(x, SpellSlot.W));
                    if (hasE.Any())
                    {
                        W.Cast(hasE.First());
                    }
                }
            }
            if (MenuValue.LaneClear.UseE && E.IsReady())
            {
                var minion = E.GetLaneMinions(MenuValue.LaneClear.OnlyKillable);
                if (minion.Any())
                {
                    var hasE = minion.Where(x => x.HasBuff("RyzeE") || x.Health < HandleDamageIndicator(x, SpellSlot.E));
                    if (hasE.Any())
                    {
                        E.Cast(hasE.First());
                    }
                    else
                    {
                        var farmloc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minion, 300, (int)E.Range);
                        if (farmloc.CastPosition.IsValid())
                        {
                            E.Cast(minion.OrderBy(x => x.Distance(farmloc.CastPosition)).First());
                        }
                    }
                }
            }
        }
    }
}
