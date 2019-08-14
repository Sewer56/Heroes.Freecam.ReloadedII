using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Reloaded_Mod_Template.Utilities
{
    /// <summary>
    /// The CameraVectors class calculates and provides the forward, right and up vector for a generic
    /// rotation of the camera.
    /// </summary>
    public struct CameraVectors
    {
        /// <summary>
        /// This is the forward unit vector (Z direction) used for the purpose
        /// of navigating through space. It is calculated by transforming the forward vector
        /// 0,0,-1 by a rotation matrix geenrated from Yaw, Pitch and Roll, then normalizing.
        /// </summary>
        public Vector3 ForwardVector;

        /// <summary>
        /// This is the left unit vector (X direction) used for the purpose of navigating through
        /// space. It is calculated by crossing the up vector 0,1,0 and the forward vector, then normalizing.
        /// </summary>
        public Vector3 LeftVector;

        /// <summary>
        /// This is the right unit vector (X direction) used for the purpose of navigating through
        /// space. It is calculated by crossing the forward & right vector, then normalizing.
        /// </summary>
        public Vector3 UpVector;

        /// <summary>
        /// Calculates the camera movement vectors from 
        /// </summary>
        /// <param name="yawRadians">The direction that moves the camera left and right.</param>
        /// <param name="pitchRadians">The direction that moves the camera up and down.</param>
        /// <param name="rollRadians">Self explanatory.</param>
        public static CameraVectors FromYawPitchRoll(float yawRadians, float pitchRadians, float rollRadians)
        {
            CameraVectors cameraVectors = new CameraVectors();

            Matrix.RotationYawPitchRoll(yawRadians, pitchRadians, rollRadians, out Matrix result);

            cameraVectors.ForwardVector = (Vector3)Vector3.Transform(Vector3.ForwardRH, result);
            cameraVectors.ForwardVector.Normalize();

            cameraVectors.LeftVector = (Vector3)Vector3.Transform(Vector3.Left, result);
            cameraVectors.LeftVector.Normalize();

            cameraVectors.UpVector = (Vector3)Vector3.Transform(Vector3.Up, result);
            cameraVectors.UpVector.Normalize();

            return cameraVectors;
        }

    }
}
