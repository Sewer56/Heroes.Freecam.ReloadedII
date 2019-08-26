using System;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.ReloadedII.Interfaces;
using SharpDX;
using sonicheroes.utils.freecam.Enums;
using sonicheroes.utils.freecam.Structs;
using sonicheroes.utils.freecam.Utilities;

namespace sonicheroes.utils.freecam
{
    public unsafe class HeroesController
    {
        // Camera Options
        public float RotateSpeed
        {
            get => _rotateSpeed;
            set
            {
                if (value < 0.02F)
                    value = 0.02F;
                
                _rotateSpeed = value;
            }
        }

        public float MoveSpeed
        {
            get => _moveSpeed;
            set
            {
                if (value < 0.02F)
                    value = 0.02F;

                _moveSpeed = value;
            }
        } 

        // Toggles
        public bool EnableHud { get; set; } = true;

        // Shorthand Functions
        public bool IsGameFrozen    => *_gameStatePointer == GameState.InGameSceneFrozen;
        public bool IsCameraEnabled => *_cameraEnabled;

        // Characters
        private CharacterPointers* _characters;

        // Camera Business
        private HeroesCamera* _camera;
        private bool*      _cameraEnabled       = (bool*) 0x00A69880;
        private float      _moveSpeed           = 5F;
        private float      _rotateSpeed         = 2F;

        // Freeze/Unfreeze.
        private static GameState _lastGameState;
        private GameState* _gameStatePointer = (GameState*)0x008D66F0;
        private HeroesCamera _cachedCamera;

        // Hook which conditionally draws HUD.
        private IHook<DrawHud> _drawHUDHook;

        // Constructor
        public HeroesController(WeakReference<IReloadedHooks> reloadedHooks, int port)
        {
            /* There is an array of pointers at 00A4CE98 but the memory they point to is static, so we can use what they point to directly. */
            switch (port)
            {
                case 0:
                    _camera = (HeroesCamera*)0x00A60C30;
                    break;
                case 1:
                    _camera = (HeroesCamera*)0x00A62F54;
                    break;
                case 2:
                    _camera = (HeroesCamera*)0x00A65278;
                    break;
                case 3:
                    _camera = (HeroesCamera*)0x00A6759C;
                    break;
            }

            _characters = &((CharacterPointers*) 0x009CE820)[port];

            if (reloadedHooks.TryGetTarget(out var hooks))
            {
                _drawHUDHook = hooks.CreateHook<DrawHud>(DrawHudImpl, 0x0041DFD0).Activate();
            }
        }

        // Hooks
        private int DrawHudImpl()
        {
            if (EnableHud)
                return _drawHUDHook.OriginalFunction();

            return 0;
        }

        // Methods: Boolean
        public bool IsInMenu()
        {
            return *_gameStatePointer == GameState.Menu || *_gameStatePointer == GameState.Null;
        }

        public bool IsPaused()
        {
            return *_gameStatePointer == GameState.InGamePaused || *_gameStatePointer == GameState.InGamePausedSettings 
                                                                || *_gameStatePointer == GameState.InGamePausedSettingsCamera 
                                                                || *_gameStatePointer == GameState.InGamePausedSettingsRebinding;
        }

        // Methods: Freeze
        public void FreezeGame()
        {
            _lastGameState = *_gameStatePointer;
            * _gameStatePointer = GameState.InGameSceneFrozen;
        }

        public void UnFreezeGame()
        {
            *_gameStatePointer = _lastGameState;
        }

        public void FreezeCamera()
        {
            _cachedCamera = *_camera;
            *_cameraEnabled = false;
        }

        public void UnFreezeCamera()
        {
            ResetCamera();
            *_cameraEnabled = true;
        }

        public void ResetCamera()
        {
            *_camera = _cachedCamera;
        }

        public void ResetCameraRoll()
        {
            (*_camera).RotationRoll = 0F;
        }

        // Methods: Camera
        public void MoveForward(float speed)
        {
            var vectors = _camera[0].GetCameraVectors();            
            ApplySpeedToCameraVectors(ref vectors, MoveSpeed);
            ApplySpeedToCameraVectors(ref vectors, speed);
            _camera[0].MoveCamera(ref vectors.ForwardVector);
        }

        public void MoveLeft(float speed)
        {
            var vectors = _camera[0].GetCameraVectors();
            ApplySpeedToCameraVectors(ref vectors, MoveSpeed);
            ApplySpeedToCameraVectors(ref vectors, speed);
            _camera[0].MoveCamera(ref vectors.LeftVector);
        }

        public void MoveUp(float speed)
        {
            var vectors = _camera[0].GetCameraVectors();
            ApplySpeedToCameraVectors(ref vectors, MoveSpeed);
            ApplySpeedToCameraVectors(ref vectors, speed);
            _camera[0].MoveCamera(ref vectors.UpVector);
        }

        // A. Get point in direction of our rotation.
        // B. Rotate the said direction by the current roll.
        // C. Apply rotation.

        public void RotateUp(float speed)
        {
            Vector3 upVector = new Vector3(0, speed * RotateSpeed, 0);
            upVector = RotateVectorAboutZ(ref upVector, -1 * _camera[0].RotationRoll);

            _camera[0].RotationVertical   += upVector.Y;
            _camera[0].RotationHorizontal += upVector.X;
        }

        public void RotateRight(float speed)
        {
            // "CAST" If the vertical angle is between 90 and 270 degrees, reverse direction.
            if (_camera[0].RotationVertical > 90F && _camera[0].RotationVertical < 270F ||
                _camera[0].RotationVertical < -90F && _camera[0].RotationVertical > -270F)
            {
                speed *= -1;
            }

            Vector3 rightVector = new Vector3(speed * RotateSpeed, 0, 0);
            rightVector = RotateVectorAboutZ(ref rightVector, -1 * _camera[0].RotationRoll);

            _camera[0].RotationVertical += rightVector.Y;
            _camera[0].RotationHorizontal += rightVector.X;
        }

        public void RotateRoll(float speed)
        {
            _camera[0].RotationRoll += speed * RotateSpeed;
        }

        public void TeleportCharacterToCamera()
        {
            (*_characters).LeaderCharacter->positionX = _camera[0].CameraX;
            (*_characters).LeaderCharacter->positionY = _camera[0].CameraY;
            (*_characters).LeaderCharacter->positionZ = _camera[0].CameraZ;
        }

        // Scales camera vectors to set move speed.
        private void ApplySpeedToCameraVectors(ref CameraVectors vectors, float speed)
        {
            vectors.UpVector.X *= speed;
            vectors.UpVector.Y *= speed;
            vectors.UpVector.Z *= speed;

            vectors.LeftVector.X *= speed;
            vectors.LeftVector.Y *= speed;
            vectors.LeftVector.Z *= speed;

            vectors.ForwardVector.X *= speed;
            vectors.ForwardVector.Y *= speed;
            vectors.ForwardVector.Z *= speed;
        }

        private Vector3 RotateVectorAboutZ(ref Vector3 vectorToRotate, float angleDegrees)
        {
            float radiansRotation = MathUtil.DegreesToRadians(angleDegrees);
            Matrix.RotationZ(radiansRotation, out Matrix result);
            Vector3.Transform(ref vectorToRotate, ref result, out Vector3 rotatedResult);
            return rotatedResult;
        }

        /* Mod Loader API */
        public void Suspend()
        {
            _drawHUDHook.Disable();
        }

        public void Resume()
        {
            _drawHUDHook.Enable();
        }

        /* Function Definitions */
        [Function(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DrawHud();
    }
}
