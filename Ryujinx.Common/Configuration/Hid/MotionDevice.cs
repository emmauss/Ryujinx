using Ryujinx.Common.DSU;
using Ryujinx.Configuration;
using System;
using System.Numerics;

namespace Ryujinx.Common.Configuration.Hid
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
                string host = config.UseAltServer ? config.DsuServerHost : ConfigurationState.Instance.Hid.DsuServerHost;
                int    port = config.UseAltServer ? config.DsuServerPort : ConfigurationState.Instance.Hid.DsuServerPort;

                _motionSource.RegisterClient((int)player, host, port);
                _motionSource.RequestData((int)player, config.Slot);

                if (config.ControllerType == ControllerType.JoyconPair && !config.MirrorInput)
                {
                    _motionSource.RequestData((int)player, config.AltSlot);
                }
            }
        }

        public void Poll(PlayerIndex player, int slot, int sensitivity)
        {
            Orientation = new float[9];

            if (!ConfigurationState.Instance.Hid.EnableDsuClient)
            {
                Accelerometer = new Vector3();
                Gyroscope     = new Vector3();

                return;
            }

            var input = _motionSource.GetData((int)player, slot);

            Gyroscope     = Truncate(input.GetGyroscope() * 0.0027f * sensitivity / 100);
            Accelerometer = Truncate(input.GetAccelerometer());
            Rotation      = Truncate(input.Rotation * 0.0027f * sensitivity / 100);

            var orientation = input.GetOrientation();

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
