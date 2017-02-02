
namespace UBAddons.Champions.Cassiopeia.Modes
{
    class Flee : Cassiopeia
    {
        public static void Execute()
        {
            var target = Q.GetTarget();
            if (target != null)
            {
                var pred = Q.GetPrediction(target);
                Q.Cast(pred.CastPosition);
            }
        }
    }
}
