using System;

namespace ForegroundServiceDemo.EventsArgs
{
    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
