using System;
using System.Numerics;

namespace Ryujinx.Common.Configuration.Hid
{
    public class MotionInput
    {
        private Vector3  _orientation  { get; set; }
        public ulong     TimeStamp     { get; set; }
        public Vector3   Accelerometer { get; set; }
        public Vector3   Gyroscrope    { get; set; }
        public Vector3   Rotation      { get; set; }

        public MotionInput()
        {
            Accelerometer = new Vector3();
            Gyroscrope    = new Vector3();
            Rotation      = new Vector3();
            _orientation  = new Vector3();
        }

        public void Update(Vector3 accel, Vector3 gyro, ulong timestamp)
        {
            Accelerometer = accel;
            Gyroscrope = gyro;

            float deltaTime = ((timestamp - TimeStamp) / 1000000f);

            var deltaGyro = gyro * deltaTime;

            try
            {
                if (TimeStamp == 0)
                {

                }
                else if (TimeStamp != 0 && deltaGyro.Length() > 0.1f)
                {
                    Rotation += deltaGyro;
                }
                else
                {
                    Gyroscrope = new Vector3();
                    deltaGyro = gyro * deltaTime;

                    return;
                }
            }
            finally
            {
                Vector3 angle = new Vector3
                {
                    X = MathF.Atan2(accel.X, MathF.Sqrt(MathF.Pow(accel.Z, 2) + MathF.Pow(accel.Y, 2))),
                    Y = MathF.Atan2(accel.Y, MathF.Sqrt(MathF.Pow(accel.Z, 2) + MathF.Pow(accel.X, 2))),
                    Z = 89,
                };

                var compAngle = angle;

               /* if (TimeStamp != 0 && deltaGyro.Length() > 0.1f)
                {
                    compAngle = new Vector3()
                    {
                        X = Filter(_orientation.X + deltaGyro.X, angle.X),
                        Y = _orientation.Y + deltaGyro.Y,
                        Z = Filter(_orientation.Z + deltaGyro.Z, angle.Z)
                    };
                }
               */
                _orientation = NormalizeAngle(compAngle);

                TimeStamp = timestamp;
            }           
        }

        public float Filter(float gyroAngle, float accelAngle)
        {
            return 0.85f * gyroAngle + 0.15f * accelAngle;
        }

        public Vector3 NormalizeAngle(Vector3 angles)
        {
            Vector3 normalized = new Vector3()
            {
                X = angles.X % 360,
                Y = angles.Y % 360,
                Z = angles.Z % 360
            };

            normalized.X = normalized.X > 180 ? normalized.X - 360 : normalized.X;
            normalized.Y = normalized.Y > 180 ? normalized.Y - 360 : normalized.Y;
            normalized.Z = normalized.Z > 180 ? normalized.Z - 360 : normalized.Z;

            return normalized;
        }

        public Matrix4x4 GetOrientation()
        {
            Vector3 orientation = NormalizeAngle(_orientation);

            var rotation = orientation * MathF.PI / 180;

            return Matrix4x4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
        }
    }
}
