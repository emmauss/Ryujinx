using System;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
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

            _filter = new MotionSensorFilter(1f / 60f);
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

                _filter.Update(gyro.X, gyro.Y, -gyro.Z, accel.X, accel.Y, accel.Z);

                TimeStamp = timestamp;
            }           
        }

        public Matrix4x4 GetOrientation()
        {
            var filteredQuat = _filter.Quaternion;
            Quaternion quaternion = new Quaternion(filteredQuat[2], filteredQuat[1], filteredQuat[0], -filteredQuat[3]);

            var matrix = Matrix4x4.CreateRotationZ(DegreeToRad(25)) * Matrix4x4.CreateFromQuaternion(quaternion);

            return matrix;
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
