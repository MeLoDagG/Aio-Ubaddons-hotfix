using EloBuddy;
using EloBuddy.SDK;

namespace UBMiddle.Champions.Camille.Modes
{
    class Flee : Camille
    {
        public static void Execute()
        {
            if (R.IsReady())
            {
                R.Cast(player.Position.Extend(Game.CursorPos, R.Range).To3DWorld());
            }
        }
    }
}
