using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;
using UBAddons.General;

namespace UBAddons.Champions.Lissandra.Modes
{
    class Combo : Lissandra
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health <= HandleDamageIndicator(x));
            if (MenuValue.Combo.UseQ && Q.IsReady())
            {
                var Target = Q.GetTarget(Champ);
                if (Target != null)
                {
                    CastQ(Target);
                }
            }
            if (MenuValue.Combo.UseW && W.IsReady())
            {
                var Target = W.GetTarget();
                if (Target != null)
                {
                    W.Cast();
                }
            }
            if (MenuValue.Combo.UseE && E.IsReady())
            {
                var Target = TargetSelector.GetTarget(200, DamageType.Magical, Game.CursorPos);
                if (Target != null && E.IsInRange(Target))
                {
                    CastE(Target, true);
                }
            }
            if (MenuValue.Combo.UseR && R.IsReady())
            {
                if (player.CountEnemyChampionsInRange(R.Range) >= MenuValue.Combo.RHit)
                {
                    R.Cast(player);
                }
                var Target = R.GetTarget(Champ, TargetSeclect.Default);
                if (Target != null)
                {
                    R.Cast(Target);
                }
            }
        }
    }
}
