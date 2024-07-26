namespace DatLycan.Packages.ServiceSystem {
    /// <summary>
    /// Defines the basic functionality for a service.
    /// </summary>
    public interface IService {
        /// <summary>
        /// Gets the priority of the service.
        /// </summary>
        /// <returns>The priority of the service.</returns>
        Priority GetPriority();

        /// <summary>
        /// Determines if the service should start automatically.
        /// </summary>
        /// <returns>True if the service should start automatically; otherwise false.</returns>
        bool DoAutoStart();
        
        /// <summary>
        /// Gets called when the service starts.
        /// </summary>
        void OnStart();
        
        /// <summary>
        /// Gets called when the service stops.
        /// </summary>
        void OnStop();
        
        /// <summary>
        /// Gets called when the service pauses or resumes.
        /// </summary>
        void OnPause(bool state);
        
        /// <summary>
        /// Updates the service. This method is typically called once per frame.
        /// </summary>
        void Update();
    }
    
    public enum Priority {
        VeryHigh,
        High,
        Normal,
        Low,
        VeryLow
    }
}
