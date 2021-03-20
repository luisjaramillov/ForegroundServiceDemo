using Realms;

namespace ForegroundServiceDemo.Models
{
    public class AlertRealm : RealmObject
    {
        public string Id { get; set; }
        public string Event { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Date { get; set; }
    }
}
