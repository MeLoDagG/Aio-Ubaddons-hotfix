namespace UBAddons.Champions.Kayle.Modes
{
    class Flee : Kayle
    {
        public static void Execute()
        {
            if (W.IsReady())
            {
                W.Cast(player);
            }
        }
    }
}
