using System;
using System.Numerics;

namespace Ryujinx.Common.Configuration.Hid
{
    public class MotionInput
    {
        private readonly Vector3 _reference = new Vector3(0, 0, 1);
        private const float Alpha = 0.9f;
        
        private Vector3 AccelerometerAngle{ get; set; }
        private Vector3 ComplementaryAngle { get; set; }

        public ulong   TimeStamp     { get; set; }
        public Vector3[] Accelerometer { get; set; }
        public Vector3[] Gyroscrope    { get; set; }
        public Vector3   Rotation      { get; set; }

        private int index;

        public MotionInput()
        {
            Accelerometer = new Vector3[3];
            Gyroscrope    = new Vector3[3];
            Rotation      = new Vector3();
            AccelerometerAngle = new Vector3();
            ComplementaryAngle = new Vector3();
        }

        public void Update(Vector3 accel, Vector3 gyro, ulong timestamp)
        {
            Accelerometer[index] = accel;
            Gyroscrope[index]    = gyro;

            ulong deltaTime = timestamp - TimeStamp;

            var deltaDegree = GetGyroscope() * (deltaTime / 1000000f);

            if (TimeStamp != 0) // don't update rotation on the first packet
            {
                Rotation += deltaDegree;
            }

            TimeStamp = timestamp;

            Vector3 accelAngle = new Vector3
            {
                X = MathF.Atan2(accel.X, MathF.Sqrt(MathF.Pow(accel.Y, 2) + MathF.Pow(accel.Z, 2))),
                Y = MathF.Atan2(accel.Y, MathF.Sqrt(MathF.Pow(accel.X, 2) + MathF.Pow(accel.Z, 2)))
            };

            AccelerometerAngle = accelAngle;

            Vector3 compAngle = new Vector3
            {
                X = Alpha * -deltaDegree.Y + (1.0f - Alpha * accelAngle.X), //roll
                Y = Alpha * deltaDegree.X + (1.0f - Alpha * accelAngle.Y) // pitch
            };

            ComplementaryAngle = compAngle;

            index++;

            if (index >= Accelerometer.Length)
            {
                index = 0;
            }
        }

        public Vector3 GetAccelerometer()
        {
            Vector3 sum = new Vector3();
            foreach(var vector in Accelerometer)
            {
                sum += vector;
            }

            return sum / Accelerometer.Length;
        }

        public Vector3 GetGyroscope()
        {
            Vector3 sum = new Vector3();
            foreach (var vector in Gyroscrope)
            {
                sum += vector;
            }

            return sum / Gyroscrope.Length;
        }

        public Matrix4x4 GetOrientation()
        {
            return Matrix4x4.CreateFromYawPitchRoll(0, ComplementaryAngle.Y, ComplementaryAngle.X);
        }
    }
}
