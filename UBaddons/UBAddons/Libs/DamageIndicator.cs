using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;
using System.Linq;
using Color = System.Drawing.Color;

namespace UBAddons.Libs
{
    internal static class DamageIndicator
    {
        public static Color Color;
        private const int BarWidth = 107;
        private const int BarHeight = 9;

        internal delegate float DamageDelegateHandle(Obj_AI_Base unit, SpellSlot? slot = null);
        public static DamageDelegateHandle DamageDelegate { get; set; }

        public static void Initalize(Color color)
        {
            Color = color;
            Drawing.OnEndScene += DrawingOnEndScene;
        }
        public static void InitalizeOnMonster(Color color)
        {
            Color = color;
            Drawing.OnEndScene += DrawingOnEndScene;
        }
        private static void DrawingOnEndScene(EventArgs args)
        {
            if (!UBAddons.PluginInstance.EnableDraw || !UBAddons.PluginInstance.EnableDamageIndicator) return;
            if (DamageDelegate == null) return;
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.VisibleOnScreen && x.IsValidTarget() && x.IsHPBarRendered))
            {
                Vector2 BarOffset = new Vector2(1, 9.2f);
                Vector2 TextOffset = new Vector2(1, -15);

                float health = enemy.TotalShieldHealth();
                float maxHealth = enemy.MaxHealth + enemy.TotalShield();
                float damage = DamageDelegate.Invoke(enemy);
                float afterHealth = (health - damage);
                float damagePercent = (afterHealth > 0f ? afterHealth : 0f) / maxHealth;
                float currentHealthPercent = health / maxHealth;
                int SpecialX = enemy.ChampionName == "Jhin" || enemy.ChampionName == "Annie" ? -12 : 0;
                int SpecialY = enemy.ChampionName == "Jhin" || enemy.ChampionName == "Annie" ? -11 : 0;
                Vector2 barPosition = new Vector2(enemy.HPBarPosition.X + enemy.HPBarXOffset + SpecialX, enemy.HPBarPosition.Y + enemy.HPBarYOffset + SpecialY);
                Vector2 barPoint = barPosition + new Vector2(BarOffset.X, BarOffset.Y);
                Vector2 startPoint = new Vector2(damagePercent * BarWidth, 0);
                Vector2 endPoint = new Vector2(currentHealthPercent * BarWidth, 0);

                Drawing.DrawLine(barPoint + startPoint, barPoint + endPoint, BarHeight, Color);

                Vector2 textPoint = barPosition + new Vector2(TextOffset.X, TextOffset.Y);
                string text = Math.Min((int)(damage / health * 100), 100) + "%";
                Drawing.DrawText(textPoint + startPoint, Color, text, 6);
            }
        }
    }
}   
