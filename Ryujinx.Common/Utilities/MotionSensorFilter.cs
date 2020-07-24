﻿using System;

namespace Ryujinx.Common.Utilities
{
    /// <summary>
    /// MadgwickAHRS class. Implementation of Madgwick's IMU and AHRS algorithms.
    /// </summary>
    /// <remarks>
    /// See: http://www.x-io.co.uk/node/8#open_source_ahrs_and_imu_algorithms
    /// Based on 
    /// https://github.com/xioTechnologies/Open-Source-AHRS-With-x-IMU/blob/master/x-IMU%20IMU%20and%20AHRS%20Algorithms/x-IMU%20IMU%20and%20AHRS%20Algorithms/AHRS/MahonyAHRS.cs
    /// </remarks>
    public class MotionSensorFilter
    {/// <summary>
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
        private float[] _intergralError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MadgwickAHRS"/> class.
        /// </summary>
        /// <param name="samplePeriod">
        /// Sample period.
        /// </param>
        public MotionSensorFilter(float samplePeriod)
            : this(samplePeriod, 1f, 0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MadgwickAHRS"/> class.
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
        /// Initializes a new instance of the <see cref="MadgwickAHRS"/> class.
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
            _intergralError = new float[] { 0f, 0f, 0f };
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
            float q1 = Quaternion[0], q2 = Quaternion[1], q3 = Quaternion[2], q4 = Quaternion[3];   // short name local variable for readability
            float norm;
            float vx, vy, vz;
            float ex, ey, ez;
            float pa, pb, pc;

            // Normalise accelerometer measurement
            norm = (float)Math.Sqrt(ax * ax + ay * ay + az * az);
            if (norm == 0f) return; // handle NaN
            norm = 1 / norm;        // use reciprocal for division

            ax *= norm;
            ay *= norm;
            az *= norm;

            // Estimated direction of gravity
            vx = 2.0f * (q2 * q4 - q1 * q3);
            vy = 2.0f * (q1 * q2 + q3 * q4);
            vz = q1 * q1 - q2 * q2 - q3 * q3 + q4 * q4;

            // Error is cross product between estimated direction and measured direction of gravity
            ex = (ay * vz - az * vy);
            ey = (az * vx - ax * vz);
            ez = (ax * vy - ay * vx);
            
            if (Ki > 0f)
            {
                _intergralError[0] += ex;      // accumulate integral error
                _intergralError[1] += ey;
                _intergralError[2] += ez;
            }
            else
            {
                _intergralError[0] = 0.0f;     // prevent integral wind up
                _intergralError[1] = 0.0f;
                _intergralError[2] = 0.0f;
            }

            // Apply feedback terms
            gx = gx + Kp * ex + Ki * _intergralError[0];
            gy = gy + Kp * ey + Ki * _intergralError[1];
            gz = gz + Kp * ez + Ki * _intergralError[2];

            // Integrate rate of change of quaternion
            pa = q2;
            pb = q3;
            pc = q4;
            q1 = q1 + (-q2 * gx - q3 * gy - q4 * gz) * (0.5f * SamplePeriod);
            q2 = pa + (q1 * gx + pb * gz - pc * gy)  * (0.5f * SamplePeriod);
            q3 = pb + (q1 * gy - pa * gz + pc * gx)  * (0.5f * SamplePeriod);
            q4 = pc + (q1 * gz + pa * gy - pb * gx)  * (0.5f * SamplePeriod);

            // Normalise quaternion
            norm = (float)Math.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);
            norm = 1.0f / norm;
            Quaternion[0] = q1 * norm;
            Quaternion[1] = q2 * norm;
            Quaternion[2] = q3 * norm;
            Quaternion[3] = q4 * norm;
        }
    }
}