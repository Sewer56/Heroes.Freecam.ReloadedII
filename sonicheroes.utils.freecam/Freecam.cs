using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Heroes.Controller.Hook.Interfaces;
using Heroes.Controller.Hook.Interfaces.Enums;
using Heroes.Controller.Hook.Interfaces.Structures;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded_Mod_Template;
using Reloaded_Mod_Template.Utilities;

namespace sonicheroes.utils.freecam
{
    public class Freecam
    {
        private WeakReference<IControllerHook> _controllerHook;
        private HeroesController _heroesController; // Named HeroesController as it lets us Control Sonic Heroes

        /* Creation / Destruction */
        public Freecam(WeakReference<IControllerHook> controllerHook, WeakReference<IReloadedHooks> reloadedHooks)
        {
            _controllerHook   = controllerHook;
            _heroesController = new HeroesController(reloadedHooks);

            if (_controllerHook.TryGetTarget(out var target))
                target.OnInput += OnInput;
        }

        /* Reloaded API Suspend/Resume */
        public void Suspend()
        {
            if (_controllerHook.TryGetTarget(out var target))
            {
                target.OnInput -= OnInput;
                _heroesController.Suspend();
            }
        }

        public void Resume()
        {
            if (_controllerHook.TryGetTarget(out var target))
            {
                target.OnInput += OnInput;
                _heroesController.Resume();
            }
        }

        /* Controller Handler */
        private void OnInput(ExtendedHeroesController inputs, int port)
        {
            // Only process inputs ingame.
            if (port != 0)
                return;

            if (!_heroesController.IsInMenu() && Utility.IsWindowActivated())
            {
                // Toggle Freeze On/Off
                if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.TeamBlast) && ButtonPressed(inputs.OneFramePressButtonFlag, ButtonFlags.Jump))
                {
                    if (_heroesController.IsGameFrozen)
                        _heroesController.UnFreezeGame();
                    else
                        _heroesController.FreezeGame();
                }

                // Toggle Camera On/Off
                if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.TeamBlast) && ButtonPressed(inputs.OneFramePressButtonFlag, ButtonFlags.FormationR))
                {
                    if (_heroesController.IsCameraEnabled)
                        _heroesController.FreezeCamera();
                    else
                        _heroesController.UnFreezeCamera();
                }

                // Process Remaining Inputs
                if (!_heroesController.IsCameraEnabled && !_heroesController.IsPaused())
                    HandleFreeMode(ref inputs);
            }
        }

        private void HandleFreeMode(ref ExtendedHeroesController inputs)
        {
            // Camera Movement Sticks
            _heroesController.MoveForward(inputs.LeftStickY * -1);
            _heroesController.MoveLeft(inputs.LeftStickX * -1);

            // Camera Rotation Sticks
            _heroesController.RotateUp(inputs.RightStickY * -1);
            _heroesController.RotateRight(inputs.RightStickX * -1);

            // Camera Roll Triggers
            // Note: The game sets Camera L/R button flags if the pressure is above 0.
            // We disallow bumpers if rotating with triggers.
            if (inputs.LeftTriggerPressure > 0)
            {
                _heroesController.RotateRoll(inputs.LeftTriggerPressure / (float)byte.MaxValue);
            }
            else
            {
                // Lift Camera Down (LB)
                if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.CameraL))
                    _heroesController.MoveUp(-1F);
            }

            if (inputs.RightTriggerPressure > 0)
            {
                _heroesController.RotateRoll((inputs.RightTriggerPressure / (float)byte.MaxValue) * -1F);
            }
            else
            {
                // Lift Camera Up (RB)
                if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.CameraR))
                    _heroesController.MoveUp(1F);
            }

            // Modify Move Speed (DPAD UD)
            if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.DpadUp))
                _heroesController.MoveSpeed -= (_heroesController.MoveSpeed * 0.011619440F); // Calculated using Geometric Progression; Approx 1 second for 2x increase.

            if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.DpadDown))
                _heroesController.MoveSpeed += (_heroesController.MoveSpeed * 0.011619440F); // Calculated using Geometric Progression; Approx 1 second for 2x increase.

            // Modify Rotate Speed (DPAD LR)
            if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.DpadRight))
                _heroesController.RotateSpeed += (_heroesController.RotateSpeed * 0.011619440F);

            if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.DpadLeft))
                _heroesController.RotateSpeed -= (_heroesController.RotateSpeed * 0.011619440F);

            // Reset Camera (X)
            if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.Action))
                _heroesController.ResetCamera();

            // Toggle HUD (B)
            if (ButtonPressed(inputs.OneFramePressButtonFlag, ButtonFlags.FormationR))
            {
                _heroesController.EnableHud = !_heroesController.EnableHud;
            }

            // Teleport Character (A)
            if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.Jump))
                _heroesController.TeleportCharacterToCamera();

            // Reset Roll (Y)
            if (ButtonPressed(inputs.ButtonFlags, ButtonFlags.FormationL))
                _heroesController.ResetCameraRoll();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ButtonPressed(ButtonFlags buttons, ButtonFlags button)
        {
            return (buttons & button) != 0;
        }
    }
}
