using System;
using System.Diagnostics;
using Heroes.Controller.Hook.Interfaces;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using Heroes.SDK;

namespace sonicheroes.utils.freecam
{
    public class Program : IMod
    {
        private const string ControllerHookModId = "sonicheroes.controller.hook";

        private IModLoader _modLoader;

        private WeakReference<IControllerHook> _controllerHook;
        private WeakReference<IReloadedHooks> _reloadedHooks;

        private Freecam[] _freeCameras;

        public void Start(IModLoaderV1 loader)
        {
            _modLoader = (IModLoader)loader;
            _modLoader.ModUnloading += ModUnloading;
            _modLoader.ModLoading   += ModLoading;

            /* Your mod code starts here. */
            Initialize();
        }

        /* Initialize/Uninitialize when Controller Hook Loading/Unloading */
        private void ModLoading(IModV1 mod, IModConfigV1 modConfig)
        {
            if (modConfig.ModId == ControllerHookModId)
                Initialize();
        }

        private void ModUnloading(IModV1 mod, IModConfigV1 modConfig)
        {
            if (modConfig.ModId == ControllerHookModId)
                DoSuspend();
        }

        private void DoSuspend()
        {
            foreach (var cam in _freeCameras)
                cam.Suspend();
        }

        private void DoResume()
        {
            foreach (var cam in _freeCameras)
                cam.Resume();
        }

        private void Initialize()
        {
            // Reloaded II controllers etc.
            _controllerHook = _modLoader.GetController<IControllerHook>();
            _reloadedHooks  = _modLoader.GetController<IReloadedHooks>();
            _reloadedHooks.TryGetTarget(out var reloadedHooks);
            SDK.Init(reloadedHooks);

            _freeCameras    = new Freecam[4];
            for (int x = 0; x < _freeCameras.Length; x++)
                _freeCameras[x] = new Freecam(_controllerHook, x);
        }

        /* Mod loader actions. */
        public void Suspend() => DoSuspend();
        public void Resume()  => DoResume();
        public void Unload()  => Suspend();

        public bool CanUnload()  => true;
        public bool CanSuspend() => true;

        /* Automatically called by the mod loader when the mod is about to be unloaded. */
        public Action Disposing { get; }
    }
}
