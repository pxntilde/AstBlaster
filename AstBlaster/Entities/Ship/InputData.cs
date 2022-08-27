using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Entities.Ship
{
    /// <summary>
    /// Input data for interfacince controllers with ships
    /// </summary>
    public record InputData
    {
        public Single Thrust;
        public Single Yaw;
        public Boolean UnlockThrust;
        public Boolean UnlockRotation;
        public Boolean Fire;

        /// <summary>
        /// Creates a new InputData record
        /// </summary>
        /// <param name="thrust">Thrust amount</param>
        /// <param name="yaw">Yaw amount</param>
        /// <param name="unlockThrust">Whether to unlock thrust</param>
        /// <param name="unlockRotation">Whether to unlock rotational</param>
        public InputData(Single thrust, Single yaw, Boolean unlockThrust, Boolean unlockRotation, Boolean fire)
        {
            Thrust = thrust;
            Yaw = yaw;
            UnlockThrust = unlockThrust;
            UnlockRotation = unlockRotation;
            Fire = fire;
        }
    }
}
