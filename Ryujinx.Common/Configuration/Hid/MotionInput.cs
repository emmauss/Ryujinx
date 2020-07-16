using System.Numerics;

namespace Ryujinx.Common.Configuration.Hid
{
    public class MotionInput
    {
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
    }
}
