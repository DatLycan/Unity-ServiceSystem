using DatLycan.Packages.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace DatLycan.Packages.ServiceSystem {
    internal static class ServiceHandlerBootstrapper {
        private static PlayerLoopSystem serviceHandlerUpdateSystem;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize() {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            
            if (!InsertStateMachineManager<Update>(ref currentPlayerLoop, 0)) {
                Debug.LogWarning("Unable to register ServiceHandler into the Update loop.");
                return;
            }
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;
#endif
            
            static void OnPlayerModeStateChanged(PlayModeStateChange state) {
                if (state != PlayModeStateChange.ExitingPlayMode) return;
                
                PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    
                RemoveStateMachineManager<Update>(ref currentPlayerLoop, serviceHandlerUpdateSystem);
                    
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                ServiceHandler.Clear();
            }
        }

        private static void RemoveStateMachineManager<T>(ref PlayerLoopSystem loop, PlayerLoopSystem system) {
            PlayerLoopUtils.RemoveSystem<T>(ref loop, in system);
        }

        private static bool InsertStateMachineManager<T>(ref PlayerLoopSystem loop, int index) {
            serviceHandlerUpdateSystem = new PlayerLoopSystem {
                type = typeof(ServiceHandler),
                updateDelegate = ServiceHandler.Update,
                subSystemList = null
            };
            return PlayerLoopUtils.InsertSystem<T>(ref loop, in serviceHandlerUpdateSystem, index);
        }
    }
}
