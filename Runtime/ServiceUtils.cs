using System;
using System.Collections.Generic;
using DatLycan.Packages.Utils;
using UnityEngine;

namespace DatLycan.Packages.ServiceSystem {
    public static class ServiceUtils {
        private static IReadOnlyList<Type> ServiceTypes { get; set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize() {
            ServiceTypes = PredefinedAssemblyUtils.GetTypes(typeof(IService));
            InitializeAllServices();
        }

        public static IService GetOrCreateServiceInstance(Type type) {
            if (!type.IsSubclassOf(typeof(MonoBehaviour))) 
                return Activator.CreateInstance(type) as IService;
            
            GameObject go = new($"{type.Name} [Service]", type);
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.GetComponent<IService>();
        }
        
        private static void InitializeAllServices() {
            foreach (Type type in ServiceTypes) {
                IService service = GetOrCreateServiceInstance(type);
                bool doAutoStart = service.DoAutoStart();
                
                if (type.IsSubclassOf(typeof(MonoBehaviour))) {
                    Component component = service as Component;
                    UnityEngine.Object.Destroy(component!.gameObject);
                } 
                
                if (!doAutoStart) continue;

                ServiceHandler.Register(service);
            }
            ServiceHandler.Sort();
            ServiceHandler.StartAllServices();
        }
    }
}