using System;
using System.Numerics;

namespace Ryujinx.Motion
{
    public class MotionInput
    {
        private readonly MotionSensorFilter _filter;

        private int _calibrationFrame = 0;

        public ulong   TimeStamp     { get; set; }
        public Vector3 Accelerometer { get; set; }
        public Vector3 Gyroscrope    { get; set; }
        public Vector3 Rotation      { get; set; }

        public MotionInput()
        {
            Accelerometer = new Vector3();
            Gyroscrope    = new Vector3();
            Rotation      = new Vector3();

            // TODO: RE the correct filter.
            _filter = new MotionSensorFilter(1 / 60f);
        }

        public void Update(Vector3 accel, Vector3 gyro, ulong timestamp, int sensitivity, float deadzone)
        {
            if (gyro.Length() <= 1f && accel.Length() >= 0.8f && accel.Z >= 0.8f)
            {
                _calibrationFrame++;

                if (_calibrationFrame >= 90)
                {
                    gyro = Vector3.Zero;

                    Rotation = Vector3.Zero;

                    _filter.Reset();

                    _calibrationFrame = 0;
                }
            }
            else
            {
                _calibrationFrame = 0;
            }

            Accelerometer = accel;

            if (gyro.Length() < deadzone)
            {
                gyro = Vector3.Zero;
            }

            gyro *= (sensitivity / 100f);

            Gyroscrope = gyro;

            float deltaTime = (timestamp - TimeStamp) / 1000000f;

            Vector3 deltaGyro = gyro * deltaTime;

            if (TimeStamp != 0)
            {
                Rotation += deltaGyro;
            }

            _filter.SamplePeriod = TimeStamp == 0 ? 1 / 60f : deltaTime;
            _filter.Update(accel, DegreeToRad(gyro));

            TimeStamp = timestamp;
        }

        public Matrix4x4 GetOrientation()
        {
            return Matrix4x4.CreateFromQuaternion(_filter.Quaternion);
        }

        private static Vector3 DegreeToRad(Vector3 degree)
        {
            return degree * (MathF.PI / 180);
        }
    }
}
