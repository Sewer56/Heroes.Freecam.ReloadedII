using System;
using Reloaded_Mod_Template.Utilities;
using SharpDX;

// ReSharper disable ConvertToAutoProperty

namespace Reloaded_Mod_Template.Structs
{
    public struct HeroesCamera
    {
        /* Camera Location */
        private float _cameraX;
        private float _cameraY;
        private float _cameraZ;

        /* Range 0 - 65535 */
        private int _angleVerticalBams;
        private int _angleHorizontalBams;
        private int _angleRollBams;

        private int _float18;
        private int _float1C;
        private int _float20;
        private float _lookAtX;
        private float _lookAtY;
        private float _lookAtZ;

        /* Properties */
        public float CameraX
        {
            get => _cameraX;
            set => _cameraX = value;
        }

        public float CameraY
        {
            get => _cameraY;
            set => _cameraY = value;
        }

        public float CameraZ
        {
            get => _cameraZ;
            set => _cameraZ = value;
        }

        public float RotationHorizontal
        {
            get => BAMSToDegrees(_angleHorizontalBams);
            set => _angleHorizontalBams = DegreesToBAMS(value % 360F);
        }

        public float RotationVertical
        {
            // Do not allow camera to go beyond 90 degrees.
            // We have roll if we want to go up-side down.

            get => BAMSToDegrees(_angleVerticalBams);
            set => _angleVerticalBams = DegreesToBAMS(value % 360);
        }

        public float RotationRoll
        {
            get => BAMSToDegrees(_angleRollBams);
            set => _angleRollBams = DegreesToBAMS(value % 360F);
        }

        public float LookAtX
        {
            get => _lookAtX;
            set => _lookAtX = value;
        }

        public float LookAtY
        {
            get => _lookAtY;
            set => _lookAtY = value;
        }

        public float LookAtZ
        {
            get => _lookAtZ;
            set => _lookAtZ = value;
        }

        /* Methods */

        // Rotation/Direction Methods
        public CameraVectors GetCameraVectors()
        {
            return CameraVectors.FromYawPitchRoll(BAMSToRadians(_angleHorizontalBams), BAMSToRadians(_angleVerticalBams), BAMSToRadians(_angleRollBams));
        }

        public Vector3 GetYawPitchRollRadians()
        {
            return new Vector3(BAMSToRadians(_angleHorizontalBams), BAMSToRadians(_angleVerticalBams), BAMSToRadians(_angleRollBams));
        }

        // Move/Rotate Methods
        public void MoveCamera(ref Vector3 vector)
        {
            _cameraX += vector.X;
            _cameraY += vector.Y;
            _cameraZ += vector.Z;
        }

        /// <summary>
        /// Rotates the camera to the specified X, Y, Z angles in degrees.
        /// </summary>
        public void RotateCameraDegrees(ref Vector3 vector)
        {
            RotationHorizontal = vector.X;
            RotationVertical = vector.Y;
            RotationRoll = vector.Z;
        }

        /// <summary>
        /// Rotates the camera to the specified X, Y, Z angles in degrees.
        /// </summary>
        public void RotateCameraRadians(ref Vector3 vector)
        {
            RotationHorizontal = MathUtil.RadiansToDegrees(vector.X);
            RotationVertical = MathUtil.RadiansToDegrees(vector.Y);
            RotationRoll = MathUtil.RadiansToDegrees(vector.Z);
        }

        /* BAMS To Degrees Conversion */
        private int DegreesToBAMS(float degrees)
        {
            return (int)((degrees / 360F) * 65535F);
        }

        private float BAMSToDegrees(int bams)
        {
            return (bams / 65535F) * 360F;
        }

        private float BAMSToRadians(int bams)
        {
            return (float) ((bams / 65535F) * (Math.PI * 2));
        }
    }
}
