using EloBuddy;
using SharpDX;
using EloBuddy.SDK;
using System.Linq;

namespace UBAddons.Champions.Amumu.Modes
{
    class Flee : Amumu
    {
        public static void Execute()
        {
            FleeTo();
        }
        internal static void FleeTo(Vector3? destination = null)
        {
            Vector3 location = (destination ?? Game.CursorPos);
            if (!E.IsReady() || !MenuValue.Flee.UseQ) return;
            var rectangle = new Geometry.Polygon.Rectangle(player.Position, location, 115f);
            var Enemyminions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(E.Range) && rectangle.IsInside(m)).OrderByDescending(x => x.Distance(location));
            var monsters = EntityManager.MinionsAndMonsters.Monsters.Where(m => m.IsValidTarget(E.Range) && rectangle.IsInside(m)).OrderByDescending(x => x.Distance(location));
            var champs = EntityManager.Heroes.Enemies.Where(c => c.IsValidTarget(E.Range) && rectangle.IsInside(c)).OrderByDescending(x => x.Distance(location));
            if (MenuValue.Flee.QMinion)
            {
                if (Enemyminions.Any())
                {
                    Q.Cast(Enemyminions.First());
                }
            }
            if (player.HealthPercent > MenuValue.Flee.HP)
            {
                if (monsters.Any() && MenuValue.Flee.QMonster)
                {
                    Q.Cast(monsters.First());
                }
                if (champs.Any() && MenuValue.Flee.QChamp)
                {
                    Q.Cast(champs.First());
                }
            }
        }
    }
}
