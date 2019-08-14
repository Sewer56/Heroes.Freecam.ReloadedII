using System;
using System.Diagnostics;
using Heroes.Controller.Hook.Interfaces;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;

namespace sonicheroes.utils.freecam
{
    public class Program : IMod, IExports
    {
        private const string ControllerHookModId = "sonicheroes.controller.hook";

        private IModLoader _modLoader;

        private WeakReference<IControllerHook> _controllerHook;
        private WeakReference<IReloadedHooks> _reloadedHooks;

        private Freecam _freecam;

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
                _freecam.Suspend();
        }

        private void Initialize()
        {
            _controllerHook = _modLoader.GetController<IControllerHook>();
            _reloadedHooks  = _modLoader.GetController<IReloadedHooks>();
            _freecam        = new Freecam(_controllerHook, _reloadedHooks);
        }

        /* Mod loader actions. */
        public void Suspend() => _freecam.Suspend();
        public void Resume()  => _freecam.Resume();
        public void Unload()  => Suspend();

        public bool CanUnload()  => true;
        public bool CanSuspend() => true;

        /* Automatically called by the mod loader when the mod is about to be unloaded. */
        public Action Disposing { get; }
        public Type[] GetTypes() => new Type[0];
    }
}
