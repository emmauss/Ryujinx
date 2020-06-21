using Ryujinx.Common.Configuration.Hid;
using Ryujinx.Configuration;
using System;
using System.Numerics;

namespace Ryujinx.Motion
{
    public class MotionDevice
    {
        public Vector3 Gyroscope     { get; private set; }
        public Vector3 Accelerometer { get; private set; }
        public Vector3 Rotation      { get; private set; }
        public float[] Orientation   { get; private set; }

        private Client _motionSource;

        public MotionDevice(Client motionSource)
        {
            _motionSource = motionSource;
        }

        public void RegisterController(PlayerIndex player)
        {
            InputConfig config = ConfigurationState.Instance.Hid.InputConfig.Value.Find(x => x.PlayerIndex == player);

            if (config != null && config.EnableMotion)
            {
                string host = config.DsuServerHost;
                int    port = config.DsuServerPort;

                _motionSource.RegisterClient((int)player, host, port);
                _motionSource.RequestData((int)player, config.Slot);

                if (config.ControllerType == ControllerType.JoyconPair && !config.MirrorInput)
                {
                    _motionSource.RequestData((int)player, config.AltSlot);
                }
            }
        }

        public void Poll(PlayerIndex player, int slot)
        {
            InputConfig config = ConfigurationState.Instance.Hid.InputConfig.Value.Find(x => x.PlayerIndex == player);

            Orientation = new float[9];

            if (!config.EnableMotion)
            {
                Accelerometer = new Vector3();
                Gyroscope     = new Vector3();

                return;
            }

            MotionInput input = _motionSource.GetData((int)player, slot);

            Gyroscope     = Truncate(input.Gyroscrope * 0.0027f);
            Accelerometer = Truncate(input.Accelerometer);
            Rotation      = Truncate(input.Rotation * 0.0027f);

            Matrix4x4 orientation = input.GetOrientation();

            Orientation[0] = Math.Clamp(orientation.M11, -1, 1);
            Orientation[1] = Math.Clamp(orientation.M12, -1, 1);
            Orientation[2] = Math.Clamp(orientation.M13, -1, 1);
            Orientation[3] = Math.Clamp(orientation.M21, -1, 1);
            Orientation[4] = Math.Clamp(orientation.M22, -1, 1);
            Orientation[5] = Math.Clamp(orientation.M23, -1, 1);
            Orientation[6] = Math.Clamp(orientation.M31, -1, 1);
            Orientation[7] = Math.Clamp(orientation.M32, -1, 1);
            Orientation[8] = Math.Clamp(orientation.M33, -1, 1);
        }

        private Vector3 Truncate(Vector3 value)
        {
            value.X = (int)(value.X * 1000) * 0.001f;
            value.Y = (int)(value.Y * 1000) * 0.001f;
            value.Z = (int)(value.Z * 1000) * 0.001f;

            return value;
        }
    }
}
