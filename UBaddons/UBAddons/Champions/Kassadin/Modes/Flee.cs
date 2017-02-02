using EloBuddy;
using EloBuddy.SDK;

namespace UBAddons.Champions.Kassadin.Modes
{
    class Flee : Kassadin
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
