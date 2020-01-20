using Heroes.SDK.Classes.PseudoNativeClasses;
using Heroes.SDK.Definitions.Enums;
using Heroes.SDK.Definitions.Structures.World.Camera;
using Reloaded.Hooks.Definitions;
using Heroes.SDK.API;
using System.Numerics;
using Heroes.SDK.Definitions.Enums.Custom;

namespace sonicheroes.utils.freecam
{
    public unsafe class HeroesController
    {
        /// <summary>
        /// Minimum speed of rotation or movement.
        /// </summary>
        public const float MinimumSpeed = 0.02F;

        // Camera Options
        public float RotateSpeed
        {
            get => _rotateSpeed;
            set
            {
                if (value < MinimumSpeed)
                    value = MinimumSpeed;
                
                _rotateSpeed = value;
            }
        }

        public float MoveSpeed
        {
            get => _moveSpeed;
            set
            {
                if (value < MinimumSpeed)
                    value = MinimumSpeed;

                _moveSpeed = value;
            }
        } 

        // Toggles
        public bool EnableHud { get; set; } = true;

        // Shorthand Functions
        public bool IsGameFrozen        => State.GameState == GameState.InGameSceneFrozen;
        public ref bool IsCameraEnabled => ref Camera.IsCameraEnabled;

        // Camera Business
        private ref HeroesCamera _camera => ref Camera.Cameras[_port];
        private float      _moveSpeed           = 5F;
        private float      _rotateSpeed         = 2F;

        // Freeze/Unfreeze.
        private static GameState _lastGameState;
        private HeroesCamera _cachedCamera;

        // Hook which conditionally draws HUD.
        private IHook<HudFunctions.DispGDisp> _drawHUDHook;
        private int _port;

        // Constructor
        public HeroesController(int port)
        {
            /* There is an array of pointers at 00A4CE98 but the memory they point to is static, so we can use what they point to directly. */
            _port       = port;
            _drawHUDHook = HudFunctions.Fun_DrawHud.Hook(DrawHudImpl).Activate();
        }

        // Hooks
        private int DrawHudImpl() => EnableHud ? _drawHUDHook.OriginalFunction() : 0;

        // Methods: Boolean
        public bool IsInMenu() => State.IsInMainMenu();
        public bool IsPaused() => State.IsPaused();

        // Methods: Freeze
        public void FreezeGame()
        {
            _lastGameState = State.GameState;
            State.GameState = GameState.InGameSceneFrozen;
        }

        public void UnFreezeGame()
        {
            State.GameState = _lastGameState;
        }

        public void FreezeCamera()
        {
            _cachedCamera = _camera;
            Camera.IsCameraEnabled = false;
        }

        public void UnFreezeCamera()
        {
            ResetCamera();
            Camera.IsCameraEnabled = true;
        }

        public void ResetCamera() => _camera = _cachedCamera;
        public void ResetCameraRoll() => _camera.Rotation.RotationRoll = 0f;

        // Methods: Camera
        public void MoveForward(float speed) => _camera.MoveBy(new Vector3(0, 0, speed * MoveSpeed));
        public void MoveLeft(float speed) => _camera.MoveBy(new Vector3(speed * MoveSpeed, 0, 0));
        public void MoveUp(float speed) => _camera.MoveBy(new Vector3(0, speed * MoveSpeed, 0));
        public void Rotate(Vector3 rotation) => _camera.RotateBy(rotation * RotateSpeed, Transform.Relative, true);

        // Methods: Utility
        public void TeleportCharacterToCamera() => Player.GetCurrentCharacterFly((Players) _port).AsReference().Position = _camera.Position;

        /* Mod Loader API */
        public void Suspend() => _drawHUDHook.Disable();
        public void Resume()  => _drawHUDHook.Enable();
    }
}
