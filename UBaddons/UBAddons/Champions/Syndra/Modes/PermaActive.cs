using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Syndra.Modes
{
    class PermaActive : Syndra
    {
        public static void Execute()
        {
            if (R.Level == 3 && R.Range < 750)
            {
                R.Range = 750;
            }
            if (Balls.Any())
            {
                foreach (var ball in Balls.Where(x => E.IsInRange(x)))
                {
                    var Vector = player.Position.Extend(ball, EQ.Range);
                    var Rectangle = new Geometry.Polygon.Rectangle(Player.Instance.Position.To2D(), Vector, EQ.Width);
                    var Count = EntityManager.Heroes.Enemies.Count(x => x.IsValid() && !x.IsDead && Rectangle.IsInside(Prediction.Position.PredictUnitPosition(x, 300)));
                    if (Count >= MenuValue.General.EQHit)
                    {
                        E.Cast(ball);
                    }
                }
            }
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    var pred = Q.GetPrediction(Target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
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
                    TakeShit_and_Cast(Target);
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = EQ.GetKillableTarget();
                if (Target != null)
                {
                    CastE(Target, MenuValue.Misc.QKS, MenuValue.Misc.WKS);
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
            {
                var Target = R.GetKillableTarget();
                if (Target != null)
                {
                    R.Cast(Target);
                }
            }
        }
    }
}
