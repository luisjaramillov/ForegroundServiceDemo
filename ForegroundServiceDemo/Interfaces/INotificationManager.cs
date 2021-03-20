﻿using System;

namespace ForegroundServiceDemo.Interfaces
{
    public interface INotificationManager
    {
        event EventHandler NotificationReceived;
        void Initialize();
        int ScheduleNotification(string title, string message);
        void ReceiveNotification(string title, string message);
        void RebuildNotificationChannel();
        void CancelNotification();
    }
}
