
<h1 align="left">Unity C# Service System</h1>

<div align="left">

[![Status](https://img.shields.io/badge/status-active-success.svg)]()
[![GitHub Issues](https://img.shields.io/github/issues/datlycan/Unity-ServiceSystem.svg)](https://github.com/DatLycan/Unity-ServiceSystem/issues)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](/LICENSE)

</div>

---

<p align="left"> Service System framework for managing and controlling various services, providing functionalities to start, stop, pause, locate, and update services, as well as manage their lifecycle.
    <br> 
</p>

## 📝 Table of Contents

- [Getting Started](#getting_started)
- [Documentation](#documentation)
- [Usage](#usage)
- [Acknowledgments](#acknowledgement)

## 🏁 Getting Started <a name = "getting_started"></a>

### Installing

1. Install the [Git Dependency Resolver](https://github.com/mob-sakai/GitDependencyResolverForUnity).
2. Import it in Unity with the Unity Package Manager using this URL:<br>
   ``https://github.com/DatLycan/Unity-ServiceSystem.git``

## 📦 Documentation <a name = "documentation"></a>

### ServiceHandler API
| Declaration  | Returns  | Description                                         |
|--------------|----------|-----------------------------------------------------|
| Register     | void     | Registers a specified service.                      |
| Unregister   | void     | Unregisters a specified service.                    |
| Start        | void     | Starts a specified service.                         |
| Stop         | void     | Stops a specified service.                          |
| Pause        | void     | Pauses (or resumes) a specified service.            |
| Locate       | IService | Locates and returns the specified service instance. |
| TryGetStatus | bool     | Tries to get the status of the specified service.   |

## 🎈 Usage <a name="usage"></a>

   ### Implementing a Service 
   ```C#
    public class MyService : IService {
        public Priority GetPriority() => Priority.VeryHigh;
        public bool DoAutoStart() => true;
    
        public void OnStart() => Debug.Log("MyService Start");
        public void OnStop() => Debug.Log("MyService Stopped");
        public void Update() => Debug.Log("MyService Update");
    }
   ```

### Managing a Service
   ```C#
    public class MyClass {
       private void MyMethod() {
           if (!ServiceHandler.TryGetStatus(out MyService serviceInstance, out bool isRunning, out bool hasStarted, out bool hasStopped)) return;
           
           Debug.Log($"Instance: {serviceInstance} | IsRunning: {isRunning} | HasStarted: {hasStarted} | HasStopped: {hasStopped}");
           Debug.Log($"Priority: {ServiceHandler.Locate<MyService>().GetPriority()}");
           
           ServiceHandler.Pause<MyService>(true);
           ServiceHandler.Pause<MyService>(false);
           ServiceHandler.Stop<MyService>();
           ServiceHandler.Start<MyService>();
           ServiceHandler.Unregister<MyService>();
           ServiceHandler.Register<MyService>();
           ServiceHandler.Start<MyService>();
       }
    }
   ```
---



## 🎉 Acknowledgements <a name = "acknowledgement"></a>

- *Using [mob-sekai's Git Dependency Resolver For Unity](https://github.com/mob-sakai/GitDependencyResolverForUnity)*


