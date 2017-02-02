using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System;

namespace UBAddons.General
{
    class UBDrawings
    {
        public static void DrawCircularTimer(Vector3 position, float radius, System.Drawing.Color color, float startDegree, float currentPercent, float width = 4f, int quality = -1)
        {
            float PI2 = (float)Math.PI * 2;
            if (quality == -1)
            {
                quality = (int)(radius / 7 + 100);
            }
            float length = currentPercent / 16.2f;
            var points = new Vector3[(int)(Math.Abs(quality * length / PI2) + 1)];
            Vector2 pos = position.To2D();
            var rad = new Vector2(0, radius);

            for (var i = 0; i <= (int)(Math.Abs(quality * length / PI2)); i++)
            {
                points[i] = (pos + rad).RotateAroundPoint(pos, startDegree + PI2 * i / quality * (length > 0 ? 1 : -1)).To3D((int)position.Z);
            }
            Line.DrawLine(color, width, points);
        }
        public static void DrawLinearTimer(Vector2 position, float currentPercent, System.Drawing.Color color)
        {
            position = new Vector2(position.X - 50, position.Y - 35);
            Vector2 end = new Vector2(position.X + 102, position.Y);
            System.Drawing.Color color2 = color.GetBrightness() > 0.65f ? System.Drawing.Color.Black : System.Drawing.Color.White;
            Vector2 CurrentEndPos = new Vector2(position.X + 1 - currentPercent, position.Y + 8.5f);
            Drawing.DrawLine(position.X, position.Y, end.X, end.Y, 2f, color2);
            Drawing.DrawLine(position.X, position.Y, position.X, position.Y + 17, 2f, color2);
            Drawing.DrawLine(end.X, end.Y, end.X, end.Y + 17, 2f, color2);
            Drawing.DrawLine(position.X, position.Y + 17, end.X, end.Y + 17, 2f, color2);
            Drawing.DrawLine(position.X + 1, position.Y + 8.5f, CurrentEndPos.X, CurrentEndPos.Y, 15, color);
        }
        public static void DrawTimer(Vector2 position, float currentTime, System.Drawing.Color color)
        {
            position = new Vector2(position.X - 30, position.Y - 60);
            Text text = new Text(Math.Round(currentTime, 2).ToString("N2"), new System.Drawing.Font("Comic Sans MS", 20, System.Drawing.FontStyle.Bold))
            {
                Color = color,
                Position = position,
            };
            text.Draw();
            //Drawing.DrawText(position.X, position.Y - 30, color, currentTime.ToString("N2") , 20);
        }
        public static void DrawText(Vector2 position, string Textt, System.Drawing.Color color)
        {
            Text text = new Text(Textt, new System.Drawing.Font("Comic Sans MS", 13))
            {
                Color = color,
                Position = position,
            };
            text.Draw();
        }
    }
}
