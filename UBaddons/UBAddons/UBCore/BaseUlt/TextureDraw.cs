using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using UBAddons.General;
using UBAddons.Libs;
using UBAddons.Properties;
using Color = System.Drawing.Color;

namespace UBAddons.UBCore.BaseUlt
{
    class TextureDraw
    {
        public static bool Initialized { get; private set; }
        private static readonly TextureLoader TextureLoader = new TextureLoader();
        private static readonly Sprite RecallSprite;
        private static readonly Sprite Bar;
        static TextureDraw()
        {
            TextureLoader.Load("RecallHUD", Resources.HUD);
            RecallSprite = new Sprite(() => TextureLoader["RecallHUD"]);

            TextureLoader.Load("Bar", Resources.Bar);
            Bar = new Sprite(() => TextureLoader["Bar"]);
        }
        internal static void Draw(Dictionary<int, Teleport_Infomation> dic)
        {
            try
            {
                foreach (var info in dic)
                {
                    var infovalue = info.Value;
                    Vector2 Position = new Vector2(Drawing.Width / 2 - 161.5f, Drawing.Height - Drawing.Height / 5);
                    Vector2 EndPosition = new Vector2(Position.X + 3f * infovalue.Percent, Position.Y);
                    Color color = BaseUlt.CanKill(infovalue) ? Color.Red : infovalue.IsRecall ? (infovalue.Percent > 70f ? Color.Lime : infovalue.Percent > 40f ? Color.YellowGreen : infovalue.Percent > 10f ? Color.Gold : Color.Tomato) : Color.Navy;
                    Bar.Color = color;
                    Bar.Rectangle = new Rectangle(0, 0, (int)(3f * infovalue.Percent), 30);
                    int count = dic.IndexOf(info.Key) + 1;

                    Drawing.DrawText(new Vector2(EndPosition.X + 23f, EndPosition.Y - 5f - (15f * count)), color,
                        $"{((AIHeroClient)infovalue.Enemy)?.ChampionName} ({Math.Round(infovalue.Enemy.Health).ToString("N0")} HP). Remaining {infovalue.Remaining.ToString("N1")} sec ({infovalue.Percent.ToString("N0")}%)", 13);
                    if (BaseUlt.CanKill(infovalue))
                    {
                        var text = $"Press {BaseUlt.BaseMenu.Get<KeyBind>("Base.Key.Enabled").KeyStrings.Item1} or {BaseUlt.BaseMenu.Get<KeyBind>("Base.Key.Enabled").KeyStrings.Item2} for free kill";
                        float xoffset = (300f - text.Length * 6f) / 2f + 16f;
                        Drawing.DrawText(new Vector2(Position.X + xoffset, EndPosition.Y + 20f), Color.AliceBlue, text, 33);
                    }
                    Vector2[] linePos =
                    {
                        new Vector2(EndPosition.X + 15, EndPosition.Y - 3f - 15f * count),
                        new Vector2(EndPosition.X + 15, EndPosition.Y + 42f)
                    };
                    Drawing.DrawLine(linePos[0], linePos[1], 4f, color);
                    DrawHUD(Position);
                }
            }
            catch (Exception e)
            {
                Log.Debug.Print(e.ToString(), Console_Message.Error);
            }
        }

        private static void DrawHUD(Vector2 position)
        {
            {
                try
                {
                    Bar.Draw(new Vector2(position.X + 15f, position.Y + 12f));
                    RecallSprite.Draw(position);
                }
                catch (Exception e)
                {
                    Log.Debug.Print(e.ToString(), Console_Message.Error);
                }
            }
        }
        public static void Initialize()
        {
            if (Initialized)
            {
                return;
            }
            Initialized = true;
        }
    }
}
