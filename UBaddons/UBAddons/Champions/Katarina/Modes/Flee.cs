using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System.Linq;

namespace UBAddons.Champions.Katarina.Modes
{
    class Flee : Katarina
    {
        public static void Execute()
        {
            FleeTo();
        }
        internal static void FleeTo(Vector3? destination = null)
        {
            Vector3 location = (destination ?? Game.CursorPos);
            if (!E.IsReady() || !MenuValue.Flee.UseE) return;
            var rectangle = new Geometry.Polygon.Rectangle(player.Position, location, 115f);
            var dagger = Dagger.Keys.Where(d => d.IsValid && E.IsInRange(d.Position) && rectangle.IsInside(d)).OrderByDescending(x => x.Distance(location));
            var Minions = EntityManager.MinionsAndMonsters.Minions.Where(m => m.IsValidTarget(E.Range) && rectangle.IsInside(m)).OrderByDescending(x => x.Distance(location));
            var monsters = EntityManager.MinionsAndMonsters.Monsters.Where(m => m.IsValidTarget(E.Range) && rectangle.IsInside(m)).OrderByDescending(x => x.Distance(location));
            var champs = EntityManager.Heroes.AllHeroes.Where(c => c.IsValidTarget(E.Range) && rectangle.IsInside(c)).OrderByDescending(x => x.Distance(location));
            var plant = ObjectManager.Get<Obj_AI_Minion>().Where(p => p.IsValidTarget(E.Range) && p.BaseSkinName.Contains("SRU_Plant") && rectangle.IsInside(p)).OrderByDescending(x => x.Distance(location));
            if (dagger.Any() && MenuValue.Flee.EDagger)
            {
                E.Cast(dagger.First().Position);
            }
            else
            {
                if (champs.Any(x => x.IsAlly))
                {
                    E.Cast(champs.FirstOrDefault(x => x.IsAlly));
                }
                else if (Minions.Any(x => x.IsAlly))
                {
                    E.Cast(Minions.FirstOrDefault(x => x.IsAlly));
                }
            }
            if (MenuValue.Flee.EMinion)
            {
                if (plant.Any())
                {
                    E.Cast(plant.First());
                }
                if (Minions.Any())
                {
                    E.Cast(Minions.First());
                }
            }
            if (player.HealthPercent > MenuValue.Flee.HP)
            {
                if (monsters.Any() && MenuValue.Flee.EMonster)
                {
                    E.Cast(monsters.First());
                }
                if (champs.Any() && MenuValue.Flee.EChamp)
                {
                    E.Cast(champs.First());
                }
            }
        }
    }
}
