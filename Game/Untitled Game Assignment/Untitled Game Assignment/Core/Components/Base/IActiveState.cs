namespace UntitledGameAssignment.Core.Components
{
    /// <summary>
    /// interface to have an active state
    /// </summary>
    public interface IActiveState
    {
        /// <summary>
        /// check is enabled this loop
        /// </summary>
        bool IsEnabled { get; }
        /// <summary>
        /// enable
        /// </summary>
        void Enable();
        /// <summary>
        /// disbale 
        /// </summary>
        void Disable();
        /// <summary>
        /// toggles active state
        /// </summary>
        void Toggle();
        /// <summary>
        /// sets active state to value
        /// </summary>
        /// <param name="value">the value we want the active state to be</param>
        void SetEnabled( bool value );
    }
}
