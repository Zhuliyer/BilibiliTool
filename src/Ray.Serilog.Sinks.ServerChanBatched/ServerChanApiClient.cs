using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ray.Serilog.Sinks.Batched;

namespace Ray.Serilog.Sinks.ServerChanBatched
{
    public class ServerChanApiClient : IPushService
    {
        //http://sc.ftqq.com/9.version

        private const string Host = "http://sc.ftqq.com";

        private readonly Uri _apiUrl;
        private readonly string _title;
        private readonly HttpClient _httpClient = new HttpClient();

        public ServerChanApiClient(string scKey, string title = "Ray.BiliBiliTool任务日报")
        {
            _title = title;
            _apiUrl = new Uri($"{Host}/{scKey}.send");
        }

        public override string Name => "Server酱";

        public override HttpResponseMessage PushMessage(string message)
        {
            base.PushMessage(message);
            var dic = new Dictionary<string, string>
            {
                {"text", _title},
                {"desp", BuildMsg(message)}
            };
            var content = new FormUrlEncodedContent(dic);
            var response = _httpClient.PostAsync(_apiUrl, content).GetAwaiter().GetResult();
            return response;
        }

        public override string BuildMsg(string msg)
        {
            //return msg.Replace(Environment.NewLine, "<br/>");//无效
            msg = msg.Replace(Environment.NewLine, Environment.NewLine + Environment.NewLine);


            msg += $"{Environment.NewLine}{Environment.NewLine}### 检测到当前为老版Server酱,即将失效,建议更换其他推送方式或更新至Server酱Turbo版";

            return msg;
        }
    }
}
