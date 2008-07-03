using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VehicleSimulation.Physics
{
    public class Acceleration
    {
        private static readonly float acceleration = 0.01f;
        private static readonly float maxSpeed = 1.5f;

        private static float z = 0;
        private static bool stop = false;

        public static float Accelerate(Keys[] keys)
        {
            if (stop && (z < -acceleration || z > acceleration))
            {
                if (z > 0)
                {
                    z += -acceleration;
                }
                if (z < 0)
                {
                    z += acceleration;
                }
                if ((z > -acceleration && z < acceleration))
                {
                    z = 0;
                }
            }
            else
            {
                stop = false;
            }

            if (!stop && (keys == null || keys.Length == 0 || (keys[0] != Keys.Up && keys[0] != Keys.Down)))
            {
                if (z > 0)
                {
                    z += -acceleration*0.5f;
                }
                if (z < 0)
                {
                    z += acceleration*0.5f;
                }
                if ((z > -acceleration && z < acceleration))
                {
                    z = 0;
                }
            }

            foreach (Keys key in keys)
            {
                if (key == Keys.W || key == Keys.Up)
                {
                    z += -acceleration;
                }
                if (key == Keys.S || key == Keys.Down)
                {
                    z += acceleration;
                }
                if (key == Keys.Space)
                {
                    stop = true;
                }
            }

            if (z < -maxSpeed || z > maxSpeed)
            {
                z = Math.Sign(z)*maxSpeed;
            }

    return z;
        }
    }
}