using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System.Linq;

namespace UBAddons.Champions.Alistar.Modes
{
    class Flee : Alistar
    {
        public static void Execute()
        {

        }
        internal static void FleeTo(Vector3? destination = null)
        {
            Vector3 location = (destination ?? Game.CursorPos);
            if (!E.IsReady()) return;
            var rectangle = new Geometry.Polygon.Rectangle(player.Position, location, 115f);
            var Enemyminions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(E.Range) && rectangle.IsInside(m)).OrderByDescending(x => x.Distance(location));
            var monsters = EntityManager.MinionsAndMonsters.Monsters.Where(m => m.IsValidTarget(E.Range) && rectangle.IsInside(m)).OrderByDescending(x => x.Distance(location));
            var champs = EntityManager.Heroes.Enemies.Where(c => c.IsValidTarget(E.Range) && rectangle.IsInside(c)).OrderByDescending(x => x.Distance(location));
            if (MenuValue.Flee.UseW)
            {
                if (Enemyminions.Any())
                {
                    E.Cast(Enemyminions.First());
                }
            }
            if (player.HealthPercent > MenuValue.Flee.HP)
            {
                if (monsters.Any() && MenuValue.Flee.WMonster)
                {
                    E.Cast(monsters.First());
                }
                if (champs.Any() && MenuValue.Flee.WChamp)
                {
                    E.Cast(champs.First());
                }
            }
        }
    }
}
