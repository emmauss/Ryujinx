using System;
using System.Numerics;

namespace Ryujinx.Common.Configuration.Hid
{
    public class MotionInput
    {
        private Vector3 _orientation { get; set; }

        public ulong     TimeStamp     { get; set; }
        public Vector3[] Accelerometer { get; set; }
        public Vector3[] Gyroscrope    { get; set; }
        public Vector3   Rotation      { get; set; }

        private int index;

        public MotionInput()
        {
            Accelerometer = new Vector3[3];
            Gyroscrope    = new Vector3[3];
            Rotation      = new Vector3();
            _orientation  = new Vector3();
        }

        public void Update(Vector3 accel, Vector3 gyro, ulong timestamp)
        {
            Accelerometer[index] = accel;

            ulong deltaTime = timestamp - TimeStamp;

            var deltaGyro = gyro * (deltaTime / 1000000f);

            try
            {
                if (TimeStamp == 0)
                {
                    Vector3 accelAngle = new Vector3
                    {
                        X = 0,
                        Y = 0,
                        Z = 0
                    };

                    _orientation = accelAngle;
                }
                else if (TimeStamp != 0 && deltaGyro.Length() > 0.1f)
                {
                    Gyroscrope[index] = gyro;

                    deltaGyro = GetGyroscope() * (deltaTime / 1000000f);

                    Rotation += deltaGyro;

                    Vector3 compAngle = new Vector3
                    {
                        X = _orientation.X + deltaGyro.X,
                        Y = _orientation.Y + deltaGyro.Y,
                        Z = _orientation.Z + deltaGyro.Z
                    };

                    _orientation = compAngle;
                }
                else
                {
                    Gyroscrope[index] = new Vector3();

                    return;
                }
            }
            finally
            {
                TimeStamp = timestamp;

                index++;

                if (index >= Accelerometer.Length)
                {
                    index = 0;
                }
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
            var rotation = _orientation * MathF.PI / 180;
            return Matrix4x4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
        }
    }
}
