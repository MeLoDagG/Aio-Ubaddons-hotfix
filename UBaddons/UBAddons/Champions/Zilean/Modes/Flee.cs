
namespace UBAddons.Champions.Zilean.Modes
{
    internal class Flee : Zilean
    {
        public static void Execute()
        {
            if (player.HasBuff("TimeWarp")) return;
            if (E.IsReady())
            {
                E.Cast(player);
            }
            if (!E.IsReady() && W.IsReady())
            {
                W.Cast();
            }
        }
    }
}
