
namespace UBAddons.Champions.Kennen.Modes
{
    class Flee : Kennen
    {
        public static void Execute()
        {
            if (E.IsReady() && (E.ToggleState == 1 || !player.HasBuff("KennenLightningRush")))
            {
                E.Cast();
            }
        }
    }
}
