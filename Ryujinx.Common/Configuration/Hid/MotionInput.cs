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
                    X = RadToDegree(MathF.Atan2(accel.Y, MathF.Sqrt(MathF.Pow(accel.X, 2) + MathF.Pow(accel.Z, 2)))),
                    Y = RadToDegree(MathF.Atan2(accel.X, MathF.Sqrt(MathF.Pow(accel.Z, 2) + MathF.Pow(accel.Y, 2)))),
                    Z = 0
                };

                angle.X = GetRelativeAngle(accel.Y, accel.Z, -angle.X);
                angle.Y = GetRelativeAngle(accel.X, accel.Z, angle.Y);

                var compAngle = angle;

                if (TimeStamp != 0)
                {
                    compAngle = new Vector3()
                    {
                        X = Filter(_orientation.X + deltaGyro.X, angle.X),
                        Z = _orientation.Z + deltaGyro.Z,
                        Y = Filter(_orientation.Y + deltaGyro.Y, angle.Y)
                    };
                }
               
                _orientation = compAngle;

                TimeStamp = timestamp;
            }           
        }

        private float GetRelativeAngle(float axis, float baseAxis, float angle)
        {
            angle = MathF.Max(-90, MathF.Min(angle, 90));

            angle = (axis <= 0, baseAxis <= 0) switch
            {
                (true, true) => angle,
                (true, false) => -180 - angle,
                (false, true) => angle,
                (false, false) => 180 - angle
            };

            return angle;
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

            normalized.X = normalized.X < 0 ? normalized.X + 360 : normalized.X;
            normalized.Y = normalized.Y < 0 ? normalized.Y + 360 : normalized.Y;
            normalized.Z = normalized.Z < 0 ? normalized.Z + 360 : normalized.Z;

            return normalized;
        }

        public Matrix4x4 GetOrientation()
        {
            Vector3 orientation = _orientation;

            var rotation = orientation * MathF.PI / 180;

            return Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z));
        }

        private float RadToDegree(float radian)
        {
            return radian * 180 / MathF.PI;
        }
    }
}
