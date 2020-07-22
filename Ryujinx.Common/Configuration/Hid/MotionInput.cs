using System;
using System.Numerics;
using Ryujinx.Common.Utilities;

namespace Ryujinx.Common.Configuration.Hid
{
    public class MotionInput
    {
        private MotionSensorFilter _filter;
        private bool _isPitchClockwise;
        private bool _isRollClockwise;
        private bool _isUp;
        private bool _isLeft;
        private bool _isBack;
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

            _filter = new MotionSensorFilter(1f / 60f, 0.1f);
        }

        public void Update(Vector3 accel, Vector3 gyro, ulong timestamp)
        {
            Accelerometer = accel;
            Gyroscrope = gyro;

            float deltaTime = (timestamp - TimeStamp) / 1000000f;

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
                gyro.X = DegreeToRad(gyro.X);
                gyro.Y = DegreeToRad(gyro.Y);
                gyro.Z = DegreeToRad(gyro.Z);

                _filter.Update(gyro.X, gyro.Y, gyro.Z, accel.Y, accel.Z, accel.X);

                TimeStamp = timestamp;
            }           
        }

        private float GetRelativeRollAngle(float axis, float angle)
        {
            angle = MathF.Max(-90, MathF.Min(angle, 90));

            angle = (axis <= 0, _isUp) switch
            {
                (false, false) => !_isRollClockwise ? -180 - angle : 180 - angle,
                (false, true) => !_isRollClockwise ?  -360 + angle : angle,
                (true, false) => !_isRollClockwise ? -180 - angle : 180 - angle,
                (true, true) => !_isRollClockwise ? angle : -360 + angle
            };

            return angle;
        }

        private float GetRelativePitchAngle(float axis, float angle)
        {
            angle = MathF.Max(-90, MathF.Min(angle, 90));

            angle = (axis <= 0, _isUp) switch
            {
                (false, false) => !_isPitchClockwise ? -180 - angle : 180 - angle,
                (false, true) => !_isPitchClockwise ? -360 + angle : angle,
                (true, false) => !_isPitchClockwise ? -180 - angle : 180 - angle,
                (true, true) => !_isPitchClockwise ? angle : -360 + angle
            };

            return angle;
        }

        public float Filter(float gyroAngle, float accelAngle)
        {
            return 0.85f * gyroAngle + 0.15f * accelAngle;
        }

        public Matrix4x4 GetOrientation()
        {
            /* Vector3 orientation = _orientation;

             var rotation = orientation * MathF.PI / 180;*/

            var filteredQuat = _filter.Quaternion;

            Quaternion quaternion = new Quaternion(filteredQuat[0], filteredQuat[1], filteredQuat[2], filteredQuat[3]);

            return Matrix4x4.CreateFromQuaternion(quaternion);
        }

        private float RadToDegree(float radian)
        {
            return radian * 180 / MathF.PI;
        }
        private float DegreeToRad(float degree)
        {
            return degree / 180 * MathF.PI;
        }
    }
}
