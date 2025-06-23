using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using LMAS.Scripts.Manager;

namespace LMAS.Scripts
{
    public class APIClient : MonoSingleton<APIClient>
    {
        private HttpClient _client;

        protected override void Awake()
        {
            base.Awake();

            // HttpClientHandler로 Keep-Alive 기본 활성화
            var handler = new HttpClientHandler
            {
                // 필요에 따라 Proxy, Redirect 등 설정
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(APISetting.APIUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };

            // 명시적으로 Connection: keep-alive 쓰기
            _client.DefaultRequestHeaders.ConnectionClose = false;
        }
    }
}