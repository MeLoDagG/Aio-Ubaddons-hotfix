using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ahri.Modes
{
    class PermaActive : Ahri
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    var pred = Q.GetPrediction(Target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var Target = W.GetKillableTarget();
                if (Target != null)
                {
                    W.Cast();
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    var pred = E.GetPrediction(target);
                    if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
            {
                var target = TargetSelector.GetTarget(EntityManager.Heroes.Enemies.Where(t => t != null
                && t.IsValid
                && player.IsInRange(t, R.Range + R.Radius)
                && t.Health <= HandleDamageIndicator(t, SpellSlot.R)), DamageType.Magical);
                if (target != null)
                {
                    var Pos = player.Position.CirclesIntersection(R.Range, target.Position, R.Radius - 100);
                    if (Pos.Count() != 0)
                    {
                        R.Cast(Pos.OrderBy(x => x.Distance(Game.CursorPos)).First().To3DWorld());
                    }
                }
            }
            if (LastTurretTarget.IsMe && player.IsUnderEnemyturret())
            {
                var spawn = ObjectManager.Get<Obj_SpawnPoint>().Where(x => x.Team == player.Team).First();
                var haslowhptarget = EntityManager.Heroes.Enemies.Any(x => x.Health < player.GetSpellDamage(x, SpellSlot.R, DamageLibrary.SpellStages.DamagePerStack));
                if (!haslowhptarget)
                {
                    if (R.IsReady() && (player.HasBuff("ahritumble") || R.ToggleState == 2))
                    {
                        R.Cast(player.Position.Extend(spawn, R.Range).To3DWorld());
                    }
                }
            }
        }
    }
}
