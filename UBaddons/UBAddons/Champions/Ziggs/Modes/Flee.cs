using SharpDX;
using EloBuddy.SDK;
using EloBuddy;

namespace UBAddons.Champions.Ziggs.Modes
{
    class Flee : Ziggs
    {
        public static void Execute()
        {
            Flee_To();
        }
        public static void Flee_To(Vector3? Destination = null)
        {
            Vector3 destination = (Destination ?? Game.CursorPos);
            if (W.IsReady() && W.ToggleState != 2)
            {
                if (W.Cast(destination.Extend(player.Position, destination.Distance(player) + 20).To3DWorld()))
                {
                    Core.DelayAction(() => Player.CastSpell(SpellSlot.W), 250);
                }
            }
        }
    }
}
