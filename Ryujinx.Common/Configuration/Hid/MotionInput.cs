using System;
using System.Numerics;

namespace Ryujinx.Common.Configuration.Hid
{
    public class MotionInput
    {
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
                    _isUp = accel.Z < 0;
                    _isLeft = accel.X < 0;
                    _isBack = accel.Y < 0;

                    _isRollClockwise = !_isUp;
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

                if((_isLeft && accel.X > 0) || (!_isLeft && accel.X < 0))
                {
                    if (_isUp)
                    {
                        _isRollClockwise = accel.X > 0;
                    }

                    _isLeft = accel.X < 0;
                }

                if ((_isBack && accel.Y > 0) || (!_isBack && accel.Y < 0))
                {
                    if (_isUp)
                    {
                        _isPitchClockwise = accel.Y > 0;
                    }

                    _isBack = accel.Y < 0;
                }

                _isUp = accel.Z < 0;

               // angle.X = -GetRelativePitchAngle(accel.Y, angle.X);
                angle.Y = GetRelativeRollAngle(accel.X, angle.Y); 

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
            Vector3 orientation = _orientation;

            var rotation = orientation * MathF.PI / 180;

            var quat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z);
            quat *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, rotation.Y);
            quat *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.X);

            return Matrix4x4.CreateFromQuaternion(quat);

           // return Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z));
        }

        private float RadToDegree(float radian)
        {
            return radian * 180 / MathF.PI;
        }
    }
}
