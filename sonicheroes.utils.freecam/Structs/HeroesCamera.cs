using System;
using System.Runtime.InteropServices;
using SharpDX;
using sonicheroes.utils.freecam.Utilities;

// ReSharper disable ConvertToAutoProperty

namespace sonicheroes.utils.freecam.Structs
{
    [StructLayout(LayoutKind.Sequential, Size = 0x2324)]
    public struct HeroesCamera
    {
        /* Camera Location */
        private float _cameraX;
        private float _cameraY;
        private float _cameraZ;

        /* Range 0 - 65535 */
        public uint AngleVerticalBams;
        public uint AngleHorizontalBams;
        public uint AngleRollBams;

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
            get => BAMSToDegrees(AngleHorizontalBams);
            set => AngleHorizontalBams = DegreesToBAMS(value);
        }

        public float RotationVertical
        {
            get => BAMSToDegrees(AngleVerticalBams);
            set => AngleVerticalBams = DegreesToBAMS(value);
        }

        public float RotationRoll
        {
            get => BAMSToDegrees(AngleRollBams);
            set => AngleRollBams = DegreesToBAMS(value);
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
            return CameraVectors.FromYawPitchRoll(BAMSToRadians(AngleHorizontalBams), BAMSToRadians(AngleVerticalBams), BAMSToRadians(AngleRollBams));
        }

        public Vector3 GetYawPitchRollRadians()
        {
            return new Vector3(BAMSToRadians(AngleHorizontalBams), BAMSToRadians(AngleVerticalBams), BAMSToRadians(AngleRollBams));
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
        private uint DegreesToBAMS(float degrees)
        {
            return (uint)(((degrees % 360F) / 360F) * 65535F);
        }

        private float BAMSToDegrees(uint bams)
        {
            return ((bams % 65535F) / 65535F) * 360F;
        }

        private float BAMSToRadians(uint bams)
        {
            return (float) (((bams % 65535F) / 65535F) * (Math.PI * 2));
        }
    }
}
