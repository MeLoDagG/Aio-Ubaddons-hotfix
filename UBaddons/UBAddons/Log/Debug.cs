using System;
using UBAddons.General;

namespace UBAddons.Log
{
    class Debug
    {
        public static void Print(string text, Console_Message mess)
        {
            switch (mess)
            {
                case Console_Message.ColorPicker:
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " - " + Variables.AddonName + "] " + text);
                        Console.ResetColor();
                    }
                    break;
                case Console_Message.Error:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " - " + Variables.AddonName + "] " + text);
                        Console.ResetColor();
                    }
                    break;
                case Console_Message.Notifications:
                    {
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " - " + Variables.AddonName + "] " + text);
                    }
                    break;
                case Console_Message.Outdate:
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " - " + Variables.AddonName + "] " + text);
                        Console.ResetColor();
                    }
                    break;
                case Console_Message.Warning:
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + " - " + Variables.AddonName + "] " + text);
                        Console.ResetColor();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mess));
            }
        }
    }
}
