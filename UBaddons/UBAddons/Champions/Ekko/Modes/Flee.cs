using EloBuddy;
using EloBuddy.SDK;
using System.Linq;


namespace UBAddons.Champions.Ekko.Modes
{
    class Flee : Ekko
    {
        public static void Execute()
        {
            if (!MenuValue.Flee.DisableWall || Libs.Dictionary.JumpSpot.GetJumpSpot(MenuValue.Flee.Distance) == null)
            {
                E.Cast(player.Position.Extend(Game.CursorPos, E.Range).To3DWorld());
            }
            if (player.HasBuff("ekkoeattackbuff"))
            {
                var rectangle = new Geometry.Polygon.Rectangle(player.Position, Game.CursorPos, 115f);
                var Enemyminions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget(player.GetAutoAttackRange()) && rectangle.IsInside(m)).OrderBy(x => x.Distance(Game.CursorPos));
                var monsters = EntityManager.MinionsAndMonsters.Monsters.Where(m => m.IsValidTarget(player.GetAutoAttackRange()) && rectangle.IsInside(m)).OrderBy(x => x.Distance(Game.CursorPos));
                var champs = EntityManager.Heroes.Enemies.Where(c => c.IsValidTarget(player.GetAutoAttackRange()) && rectangle.IsInside(c)).OrderBy(x => x.Distance(Game.CursorPos));
                if (Enemyminions.Any())
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Enemyminions.FirstOrDefault());
                }
                if (player.HealthPercent > MenuValue.Flee.HP)
                {
                    if (monsters.Any() && MenuValue.Flee.AAMonster)
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, monsters.FirstOrDefault());
                    }
                    if (champs.Any() && MenuValue.Flee.AAChamp)
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, monsters.FirstOrDefault());
                    }
                }
            }
        }        
    }
}
