using System.IO;
using System.Net;
using System.Text;

namespace WeeklyRobot.Service
{
    public static class HttpHelper
    {
        public static string Post(string url, PostBody postBody, int timeout = 5000)
        {
            var bytes = Encoding.UTF8.GetBytes(postBody.ToString());
            var request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = "POST";
            request.Timeout = timeout;

            request.ContentLength = bytes.Length;


            request.ContentType = Config.TryGet(nameof(request.ContentType), "application /x-www-form-urlencoded; charset=UTF-8");


            var magicCodeName = string.Empty;

            if (Config.TryGet("MagicCodeType", out magicCodeName))
            {
                var magicCodeValue = Config.TryGet("MagicCodeValue", "");
                var magicCodePath = Config.TryGet("MagicCodePath", "/");
                var magicCodeDomain = Config.TryGet("MagicCodeDomain", ".baidu.com");
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Cookie(magicCodeName, magicCodeValue, magicCodePath, magicCodeDomain));
            }

            var proxyAddress = string.Empty;
            if (Config.TryGet("WebProxy", out proxyAddress))
            {
                request.Proxy = new WebProxy(proxyAddress);
            }

            using (var reqstream = request.GetRequestStream())
            {
                reqstream.Write(bytes, 0, bytes.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            }
        }

    }
}
