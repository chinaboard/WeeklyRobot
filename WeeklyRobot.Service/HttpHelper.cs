using System;
using System.Net;
using System.Net.Http;

namespace WeeklyRobot.Service
{
    public static class HttpHelper
    {
        public static string Post(string url, PostBody postBody, int timeout = 5000)
        {
            try
            {
                Logger.Debug($"Post\t{url}");

                using (var handler = new HttpClientHandler() { CookieContainer = new CookieContainer() })
                using (var client = new HttpClient(handler))
                {
                    var content = new FormUrlEncodedContent(postBody.Body);
                    var magicCodeName = string.Empty;
                    if (Config.TryGet("MagicCodeType", out magicCodeName))
                    {
                        var magicCodeValue = Config.TryGet("MagicCodeValue", "");
                        var magicCodePath = Config.TryGet("MagicCodePath", "/");
                        var magicCodeDomain = Config.TryGet("MagicCodeDomain", ".baidu.com");
                        handler.CookieContainer.Add(new Cookie(magicCodeName, magicCodeValue, magicCodePath, magicCodeDomain));
                    }
                    var result = client.PostAsync(url, content).Result;
                    result.EnsureSuccessStatusCode();

                    return result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return e.Message;
            }
        }

    }
}
