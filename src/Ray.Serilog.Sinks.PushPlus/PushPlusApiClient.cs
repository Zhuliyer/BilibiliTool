using System;
using System.Net.Http;
using System.Text;
using Ray.Serilog.Sinks.Batched;

namespace Ray.Serilog.Sinks.PushPlus
{
    public class PushPlusApiClient : IPushService
    {
        //http://www.pushplus.plus/doc/

        private const string Host = "http://www.pushplus.plus/send";

        private readonly Uri _apiUrl;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _token;
        private readonly string _topic;
        private readonly string _channel;
        private readonly string _webhook;

        public PushPlusApiClient(
            string token,
            string topic = null,
            string channel = "",
            string webhook = ""
            )
        {
            _apiUrl = new Uri(Host);
            _token = token;
            _topic = topic;
            _channel = channel;
            _webhook = webhook;
        }

        public override string Name => "PushPlus";

        private PushPlusChannelType ChannelType
        {
            get
            {
                var re = PushPlusChannelType.wechat;

                if (_channel.IsNullOrEmpty()) return re;

                bool suc = Enum.TryParse<PushPlusChannelType>(_channel, true, out PushPlusChannelType channel);
                if (suc) re = channel;

                return re;
            }
        }

        public override HttpResponseMessage PushMessage(string message)
        {
            base.PushMessage(message);

            var json = new
            {
                token = _token,

                topic = _topic,
                channel = this.ChannelType.ToString(),
                webhook = _webhook,

                title = "Ray.BiliBiliTool任务日报",
                content = BuildMsg(message),//换行有问题，这里使用<br/>替换\r\n

                template = PushPlusMsgType.html.ToString()
            }.ToJson();

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync(_apiUrl, content).GetAwaiter().GetResult();
            return response;
        }

        public override string BuildMsg(string msg)
        {
            return msg.Replace(Environment.NewLine, "<br/>");

            /*
             * 公众号预览可以正常换行，但是详情页换行失败，需要使用<br/>实现
             */
        }
    }

    public enum PushPlusMsgType
    {
        html,
        json,
        markdown,
        cloudMonitor,
        jenkins,
        route
    }

    public enum PushPlusChannelType
    {
        wechat,
        webhook,
        cp,
        sms,
        mail
    }
}
