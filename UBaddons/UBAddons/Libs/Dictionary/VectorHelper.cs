using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

namespace UBAddons.Libs.Dictionary
{
    class VectorHelper
    {
        /// <summary>
        /// Checking Collision
        /// </summary>
        /// <param name="start">Start Position</param>
        /// <param name="end">End Position</param>
        /// <param name="range">Range</param>
        /// <param name="width">Width of spell</param>
        /// <param name="speed">Speed of spell</param>
        /// <param name="castdelay">Delay of spell</param>
        /// <returns></returns>
        public static Vector2 Linear_Collision_Point(Vector3 start, Vector3 end, uint range, int width, int speed, int castdelay)
        {
            var possiblecolldies = EntityManager.Enemies.Where(x => x.IsValidTarget(range));
            var spellpolygon = new Geometry.Polygon.Rectangle(start, start.Extend(end, range).To3D(), width);
            var time = start.Distance(end) / speed * 1000 + castdelay;
            var collidetarget = possiblecolldies.OrderBy(x => x.Distance(start)).FirstOrDefault(x => spellpolygon.IsInside(Prediction.Position.PredictUnitPosition(x, (int)time + 1)));
            if (collidetarget != null)
            {
                return start.Extend(end, start.Distance(collidetarget));
            }
            return Vector2.Zero;
        }
    }
}
