using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DatLycan.Packages.ServiceSystem {
    /// <summary>
    /// A static class that manages and controls various services.
    /// </summary>
    public static class ServiceHandler {
        // A list of registered service entries.
        private static readonly List<ServiceEntry> serviceEntries = new();

        /// <summary>
        /// Starts the specified service.
        /// </summary>
        /// <param name="service">The service to start.</param>
        public static void Start(IService service) => HandleStart(service.GetType());
        
        /// <summary>
        /// Starts a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to start.</typeparam>
        public static void Start<T>() where T : IService => HandleStart(typeof(T));
        
        /// <summary>
        /// Stops the specified service.
        /// </summary>
        /// <param name="service">The service to stop.</param>
        public static void Stop(IService service) => HandleStop(service.GetType());
        
        /// <summary>
        /// Stops a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to stop.</typeparam>
        public static void Stop<T>() where T : IService => HandleStop(typeof(T));

        /// <summary>
        /// Pauses or resumes the specified service.
        /// </summary>
        /// <param name="service">The service to pause or resume.</param>
        /// <param name="state">The state to set (false for running, true for paused).</param>
        public static void Pause(IService service, bool state) => HandlePause(service.GetType(), state);
        
        /// <summary>
        /// Pauses or resumes a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to pause or resume.</typeparam>
        /// <param name="state">The state to set (false for running, true for paused).</param>
        public static void Pause<T>(bool state) where T : IService => HandlePause(typeof(T), state);
        
        /// <summary>
        /// Locates and returns the specified service.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located service.</returns>
        public static IService Locate(IService service) => HandleLocate<IService>(service.GetType());
        
        /// <summary>
        /// Locates and returns a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to locate.</typeparam>
        /// <returns>The located service of type T.</returns>
        public static T Locate<T>() where T : class, IService => HandleLocate<T>(typeof(T));

        /// <summary>
        /// Tries to get the status of the specified service.
        /// </summary>
        /// <param name="service">The service to check.</param>
        /// <param name="isRunning">Outputs whether the service is running.</param>
        /// <param name="hasStarted">Outputs whether the service has started.</param>
        /// <param name="hasStopped">Outputs whether the service has stopped.</param>
        /// <returns>True if the service is found; otherwise false.</returns>
        public static bool TryGetStatus(IService service, out bool isRunning, out bool hasStarted, out bool hasStopped) 
            => HandleTryGetStatus(service.GetType(), out service, out isRunning, out hasStarted, out hasStopped);
        
        /// <summary>
        /// Tries to get the status of a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to check.</typeparam>
        /// <param name="serviceInstance">Outputs the service instance.</param>
        /// <param name="isRunning">Outputs whether the service is running.</param>
        /// <param name="hasStarted">Outputs whether the service has started.</param>
        /// <param name="hasStopped">Outputs whether the service has stopped.</param>
        /// <returns>True if the service is found; otherwise false.</returns>
        public static bool TryGetStatus<T>(out T serviceInstance, out bool isRunning, out bool hasStarted, out bool hasStopped) where T : class, IService 
            => HandleTryGetStatus(typeof(T), out serviceInstance, out isRunning, out hasStarted, out hasStopped);

        /// <summary>
        /// Starts all registered services.
        /// </summary>
        public static void StartAllServices() => serviceEntries.ForEach(se => Start(se.Service));
        
        /// <summary>
        /// Stops all registered services.
        /// </summary>
        public static void StopAllServices() => serviceEntries.ForEach(se => Stop(se.Service));

        /// <summary>
        /// Registers the specified service.
        /// </summary>
        /// <param name="service">The service to register.</param>
        public static void Register(IService service) => HandleRegister(service.GetType());
        
        /// <summary>
        /// Registers a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        public static void Register<T>() where T : IService => HandleRegister(typeof(T));

        /// <summary>
        /// Unregisters the specified service.
        /// </summary>
        /// <param name="service">The service to unregister.</param>
        public static void Unregister(IService service) => HandleUnregister(service.GetType());
        
        /// <summary>
        /// Unregisters a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to unregister.</typeparam>
        public static void Unregister<T>() where T : IService => HandleUnregister(typeof(T));
        
        /// <summary>
        /// Sorts the registered services by their priority.
        /// </summary>
        public static void Sort() {
            System.Random rand = new();
            serviceEntries.Sort((x, y) => {
                int priorityComparison = x.Service.GetPriority().CompareTo(y.Service.GetPriority());
                return priorityComparison == 0 ? rand.Next(-1, 2) : priorityComparison;
            });
        }

        /// <summary>
        /// Clears all registered services.
        /// </summary>
        public static void Clear() => serviceEntries.Clear();

        /// <summary>
        /// Updates all running services every frame.
        /// </summary>
        public static void Update() => serviceEntries.ForEach(se => {
            if (se.IsRunning) se.Service.Update();
        });


        private static void HandleStart(Type type) {
            if (!TryGetServiceEntry(type, out ServiceEntry entry)) return;

            if (entry.HasStarted) {
                Debug.LogWarning($"[{type.Name}] has already started.");
                return;
            }

            Debug.Log($"Starting service [{entry.Service}] ...");
            entry.Service.OnStart();
            entry.HasStarted = true;
            entry.HasStopped = false;
            entry.IsRunning = true;
        }

        private static void HandleStop(Type type) {
            if (!TryGetServiceEntry(type, out ServiceEntry entry)) return;
            
            if (entry.HasStopped) {
                Debug.LogWarning($"[{type.Name}] has already stopped.");
                return;
            }

            Debug.Log($"Stopping service: {entry.Service}");
            entry.Service.OnStop();
            entry.HasStarted = false;
            entry.HasStopped = true;
            entry.IsRunning = false;
        }
        
        private static void HandlePause(Type type, bool state) {
            if (!TryGetServiceEntry(type, out ServiceEntry entry)) return;

            if (!entry.HasStarted) {
                Debug.LogWarning($"[{type.Name}] has never started.");
                return;
            }
            
            entry.Service.OnPause(state);
            entry.IsRunning = !state;
            
            Debug.Log($"Paused service [{entry.Service}]: {state}");
        }
        
        private static T HandleLocate<T>(Type type) where T : class, IService {
            if (!TryGetServiceEntry(type, out ServiceEntry entry)) return null;
            return entry.Service as T;
        } 
        
        private static bool HandleTryGetStatus<T>(Type type, out T serviceInstance, out bool isRunning, out bool hasStarted, out bool hasStopped) where T : class, IService {
            if (!TryGetServiceEntry(type, out ServiceEntry entry)) {
                serviceInstance = default;
                isRunning = default;
                hasStarted = default;
                hasStopped = default;
                return false;
            }

            serviceInstance = entry.Service as T;
            isRunning = entry.IsRunning;
            hasStarted = entry.HasStarted;
            hasStopped = entry.HasStopped;
            return true;
        }
        
        private static void HandleRegister(Type type) {
            if (TryGetServiceEntry(type, out ServiceEntry _, false)) {
                Debug.LogWarning($"[{type.Name}] is already registered.");
                return;
            }

            IService service = ServiceUtils.GetOrCreateServiceInstance(type);
            serviceEntries.Add(new ServiceEntry(service));
            Debug.Log($"Registered service: [{service}]");
        }

        private static void HandleUnregister(Type type) {
            if (!TryGetServiceEntry(type, out ServiceEntry entry)) {
                return;
            }
            
            serviceEntries.Remove(entry);
            Debug.Log($"Unregistered service: [{entry.Service}]");
            
            if (type.IsSubclassOf(typeof(MonoBehaviour))) {
                Component component = entry.Service as Component;
                Object.Destroy(component!.gameObject);
            } 
        }

        private static bool TryGetServiceEntry(Type type, out ServiceEntry entry, bool warn = true) {
            ServiceEntry foundEntry = serviceEntries.Find(se => se.Service.GetType() == type);
            if (foundEntry != null) {
                entry = foundEntry;
                return true;
            }

            entry = null;
            if (warn) Debug.LogWarning($"[{type.Name}] couldn't be located. Is the service registered?");
            return false;
        }

      
        private class ServiceEntry {
            public readonly IService Service;
            public bool HasStarted, HasStopped, IsRunning;

            public ServiceEntry(IService service) {
                Service = service;
                HasStarted = false;
                HasStopped = false;
                IsRunning = false;
            }
        }
    }
}
