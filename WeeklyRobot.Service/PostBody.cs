using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeeklyRobot.Service
{
    public class PostBody : IEnumerable<KeyValuePair<string, string>>
    {
        private static Random _rand = new Random(DateTime.Now.GetHashCode());
        public Dictionary<string, string> Body = new Dictionary<string, string>();

        public PostBody(string title, string[] content, int type)
        {
            Body["mod"] = Config.TryGet("mod", "OnSubmitWork");
            var htmlContent = new StringBuilder();

            content.ToList().ForEach(line => htmlContent.Append($"<p>{line}</p>"));

            Body["Content"] = htmlContent.ToString();
            Body["WorkType"] = type.ToString();
            Body["Title"] = title;
            Body["BUType"] = "0";
            Body["IsTop"] = "0";
            Body["Tag"] = "0";
            Body["____v"] = _rand.NextDouble().ToString();
        }

        public string this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public string Get(string key)
        {
            var result = string.Empty;
            if (!string.IsNullOrWhiteSpace(key))
            {
                Body.TryGetValue(key, out result);
            }
            return result;
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            Body[key] = value;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var item in Body)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var kvp in Body)
            {
                builder.Append($"{kvp.Key}={kvp.Value}&");
            }
            return builder.ToString().TrimEnd('&');
        }
    }
}
