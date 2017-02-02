namespace UBAddons.Champions.Lulu.Modes
{
    class Flee : Lulu
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
