using Newtonsoft.Json;

namespace WeeklyRobot.Service
{
    public static class JsonHelper
    {
        public static T DeserializeObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string SerializeObject(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
