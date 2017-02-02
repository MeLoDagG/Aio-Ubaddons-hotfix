namespace UBAddons.Champions.Karma.Modes
{
    class Flee : Karma
    {
        public static void Execute()
        {
            if (E.IsReady())
            {
                E.Cast(player);
            }
        }
    }
}
