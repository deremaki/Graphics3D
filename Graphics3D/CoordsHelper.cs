using Microsoft.Xna.Framework;
using System;

namespace Graphics3D
{
    public static class CoordsHelper
    {
        public static Vector3 SphericalToCartesian(this Vector3Spherical vec)
        {
            var outCart = new Vector3();
            outCart.X = (float)(vec.Radius * Math.Sin(vec.Theta) * Math.Cos(vec.Phi));
            outCart.Y = -(float)(vec.Radius * Math.Sin(vec.Theta) * Math.Sin(vec.Phi));
            outCart.Z = (float)(vec.Radius * Math.Cos(vec.Theta));

            return outCart;
        }

        public static Vector3Spherical CartesianToSpherical(this Vector3 cartCoords)
        {
            var vec = new Vector3Spherical();

            var module = CalculateModule(cartCoords);
            vec.Radius = module;
            vec.Theta = (float)Math.Acos(cartCoords.Z / module);
            vec.Phi = (float)(Math.Atan(-cartCoords.Y / cartCoords.X));

            return vec;
        }

        public static float CalculateModule(this Vector3 vec)
        {
            return (float)(Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z));
        }

        public static float GetXRotation(this Vector3 direction)
        {
            Vector3Spherical vec = direction.CartesianToSpherical();
            return MathHelper.ToDegrees(90.0f - vec.Theta);
        }

        public static float GetYRotation(this Vector3 direction)
        {
            Vector3Spherical vec = direction.CartesianToSpherical();
            return MathHelper.ToDegrees(90.0f - vec.Phi);
        }
    }

    public struct Vector3Spherical
    {
        public float Radius;
        public float Theta;
        public float Phi;
    }
}
