using System;
using System.Collections.Generic;
using System.Text;

namespace ForegroundServiceDemo.Interfaces
{
    public interface ISyncService
    {
        void StartService();
        void StopService();
        void ValidateBatteryOptimization();
    }
}
