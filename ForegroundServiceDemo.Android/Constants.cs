using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForegroundServiceDemo.Droid
{
	public static class Constants
	{
		public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
		public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
		public const string SERVICE_STARTED_KEY = "has_service_been_started";
		public const string BROADCAST_MESSAGE_KEY = "broadcast_message";
		public const string NOTIFICATION_BROADCAST_ACTION = "ForegroundServiceDemo.Notification.Action";

		public const string ACTION_START_SERVICE = "ForegroundServiceDemo.action.START_SERVICE";
		public const string ACTION_STOP_SERVICE = "ForegroundServiceDemo.action.STOP_SERVICE";
		public const string ACTION_RESTART_TIMER = "ForegroundServiceDemo.action.RESTART_TIMER";
		public const string ACTION_MAIN_ACTIVITY = "ForegroundServiceDemo.action.MAIN_ACTIVITY";
	}
}