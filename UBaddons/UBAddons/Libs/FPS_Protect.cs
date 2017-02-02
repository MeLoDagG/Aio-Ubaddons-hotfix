using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace UBAddons.Libs
{
    class FPS_Protect
    {
        private static int Delay, LastTick;
        public static bool CheckFps()
        {
            Delay += Core.GameTickCount - LastTick;
            LastTick = Core.GameTickCount;
            if (!UBCore.CoreMenu.Core.VChecked("Core.Enable.FPS"))
            {
                return true;
            }
            var rate = UBCore.CoreMenu.Core.VSliderValue("Core.Calculate");
            if (Game.FPS < UBCore.CoreMenu.Core.VSliderValue("Core.Min.FPS"))
            {
                rate = Math.Min(10, UBCore.CoreMenu.Core.VSliderValue("Core.Calculate"));
            }
            if (Delay >= 1000f / rate)
            {
                return true;
            }
            return false;
        }
    }
}
