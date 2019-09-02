﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using NosCore.Core.HttpClients.ChannelHttpClient;
using System.Threading.Tasks;

namespace NosCore.Core.HttpClients
{
    public class MasterServerHttpClient
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        private readonly Channel _channel;
        private readonly IChannelHttpClient _channelHttpClient;

        public virtual string ApiUrl { get; set; }
        public virtual bool RequireConnection { get; set; }

        protected MasterServerHttpClient(IHttpClientFactory httpClientFactory, Channel channel, IChannelHttpClient channelHttpClient)
        {
            _httpClientFactory = httpClientFactory;
            _channel = channel;
            _channelHttpClient = channelHttpClient;
        }

        public virtual HttpClient Connect()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_channel.MasterCommunication.ToString());

            if (RequireConnection)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _channelHttpClient.GetOrRefreshToken());
            }
            return client;
        }

        protected T Post<T>(object objectToPost)
        {
            var client = Connect();
            var content = new StringContent(JsonConvert.SerializeObject(objectToPost),
                Encoding.Default, "application/json");
            var response = client.PostAsync(ApiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }

            throw new ArgumentException();
        }


        protected T Patch<T>(object id, object objectToPost)
        {
            var client = Connect();
            var content = new StringContent(JsonConvert.SerializeObject(objectToPost),
                Encoding.Default, "application/json");
            var response = client.PatchAsync($"{ApiUrl}?id={id}", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }

            throw new ArgumentException();
        }

        protected Task Post(object objectToPost)
        {
            var client = Connect();
            var content = new StringContent(JsonConvert.SerializeObject(objectToPost),
                Encoding.Default, "application/json");
            return client.PostAsync(ApiUrl, content);
        }

        protected T Get<T>() => Get<T>(null);

        protected T Get<T>(object id)
        {
            var client = Connect();
            var response = client.GetAsync($"{ApiUrl}{(id!=null? $"?id={id}" : "")}").Result;
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }

            throw new ArgumentException();
        }

        protected Task Delete(object id)
        {
            var client = Connect();
            return client.DeleteAsync($"{ApiUrl}?id={id}");
        }
    }
}