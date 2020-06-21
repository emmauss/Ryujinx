using System;
using System.Numerics;

namespace Ryujinx.Motion
{
    // MadgwickAHRS class. Implementation of Madgwick's IMU and AHRS algorithms.
    // See: http://www.x-io.co.uk/node/8#open_source_ahrs_and_imu_algorithms
    // Based on 
    // https://github.com/xioTechnologies/Open-Source-AHRS-With-x-IMU/blob/master/x-IMU%20IMU%20and%20AHRS%20Algorithms/x-IMU%20IMU%20and%20AHRS%20Algorithms/AHRS/MahonyAHRS.cs
    public class MotionSensorFilter
    {
        /// <summary>
        /// Gets or sets the sample period.
        /// </summary>
        public float SamplePeriod { get; set; }

        /// <summary>
        /// Gets or sets the algorithm proportional gain.
        /// </summary>
        public float Kp { get; set; }

        /// <summary>
        /// Gets or sets the algorithm integral gain.
        /// </summary>
        public float Ki { get; set; }

        /// <summary>
        /// Gets or sets the Quaternion output.
        /// </summary>
        public float[] Quaternion { get; set; }

        /// <summary>
        /// Gets or sets the integral error.
        /// </summary>
        private Vector3 _intergralError { get; set; }

        /// <summary>
        /// Gets or sets the integral error.
        /// </summary>
        public float SampleRateCoefficient { get; set; } = 0.45f;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionSensorFilter"/> class.
        /// </summary>
        /// <param name="samplePeriod">
        /// Sample period.
        /// </param>
        public MotionSensorFilter(float samplePeriod)
            : this(samplePeriod, 1f, 0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionSensorFilter"/> class.
        /// </summary>
        /// <param name="samplePeriod">
        /// Sample period.
        /// </param>
        /// <param name="kp">
        /// Algorithm proportional gain.
        /// </param> 
        public MotionSensorFilter(float samplePeriod, float kp)
            : this(samplePeriod, kp, 0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionSensorFilter"/> class.
        /// </summary>
        /// <param name="samplePeriod">
        /// Sample period.
        /// </param>
        /// <param name="kp">
        /// Algorithm proportional gain.
        /// </param>
        /// <param name="ki">
        /// Algorithm integral gain.
        /// </param>
        public MotionSensorFilter(float samplePeriod, float kp, float ki)
        {
            SamplePeriod = samplePeriod;

            Kp = kp;
            Ki = ki;

            Quaternion      = new float[] { 1f, 0f, 0f, 0f };
            _intergralError = new Vector3();
        }

        /// <summary>
        /// Algorithm IMU update method. Requires only gyroscope and accelerometer data.
        /// </summary>
        /// <param name="gx">
        /// Gyroscope x axis measurement in radians/s.
        /// </param>
        /// <param name="gy">
        /// Gyroscope y axis measurement in radians/s.
        /// </param>
        /// <param name="gz">
        /// Gyroscope z axis measurement in radians/s.
        /// </param>
        /// <param name="ax">
        /// Accelerometer x axis measurement in any calibrated units.
        /// </param>
        /// <param name="ay">
        /// Accelerometer y axis measurement in any calibrated units.
        /// </param>
        /// <param name="az">
        /// Accelerometer z axis measurement in any calibrated units.
        /// </param>
        public void Update(float gx, float gy, float gz, float ax, float ay, float az)
        {
            float q1 = Quaternion[0];
            float q2 = Quaternion[1];
            float q3 = Quaternion[2];
            float q4 = Quaternion[3];
            float norm;
            
            Vector3 gravity = new Vector3();
            Vector3 error   = new Vector3();
            Vector3 delta   = new Vector3();

            // Normalise accelerometer measurement
            norm = MathF.Sqrt(ax * ax + ay * ay + az * az);

            if (norm == 0f)
            {
                return; // handle NaN
            }

            norm = 1f / norm; // use reciprocal for division

            ax *= norm;
            ay *= norm;
            az *= norm;

            // Estimated direction of gravity
            gravity.X = 2.0f * (q2 * q4 - q1 * q3);
            gravity.Y = 2.0f * (q1 * q2 + q3 * q4);
            gravity.Z = q1 * q1 - q2 * q2 - q3 * q3 + q4 * q4;

            // Error is cross product between estimated direction and measured direction of gravity
            error.X = ay * gravity.Z - az * gravity.Y;
            error.Y = az * gravity.X - ax * gravity.Z;
            error.Z = ax * gravity.Y - ay * gravity.X;
            
            if (Ki > 0f)
            {
                _intergralError += error; // accumulate integral error
            }
            else
            {
                _intergralError = new Vector3(); // prevent integral wind up
            }

            // Apply feedback terms
            gx = gx + Kp * error.X + Ki * _intergralError.X;
            gy = gy + Kp * error.Y + Ki * _intergralError.Y;
            gz = gz + Kp * error.Z + Ki * _intergralError.Z;

            // Integrate rate of change of quaternion
            delta.X = q2;
            delta.Y = q3;
            delta.Z = q4;

            q1 += (-q2 * gx - q3 * gy - q4 * gz) * (SampleRateCoefficient * SamplePeriod);
            q2 = delta.X + (q1 * gx + delta.Y * gz - delta.Z * gy)  * (SampleRateCoefficient * SamplePeriod);
            q3 = delta.Y + (q1 * gy - delta.X * gz + delta.Z * gx)  * (SampleRateCoefficient * SamplePeriod);
            q4 = delta.Z + (q1 * gz + delta.X * gy - delta.Y * gx)  * (SampleRateCoefficient * SamplePeriod);

            // Normalise quaternion
            norm = MathF.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);
            norm = 1.0f / norm;
            
            Quaternion[0] = q1 * norm;
            Quaternion[1] = q2 * norm;
            Quaternion[2] = q3 * norm;
            Quaternion[3] = q4 * norm;
        }
    }
}
