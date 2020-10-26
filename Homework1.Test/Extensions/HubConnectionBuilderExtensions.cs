using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Homework1.Database.Entities;
using Homework1.Models.Requests;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace Homework1.Test.Extensions
{
    public static class HubConnectionBuilderExtensions
    {
        public static async Task<IHubConnectionBuilder> WithUrl(this IHubConnectionBuilder builder, Uri url,
            string email, string password, Action<HttpConnectionOptions> configureHttpConnection = null)
        {
            var cookies = await Login(email, password);

            return builder.WithUrl(url, options => {
                options.Cookies.Add(cookies);
                configureHttpConnection?.Invoke(options);
            });
        }

        private static async Task<CookieCollection> Login(string email, string password)
        {
            var uri = new Uri("https://localhost:5001/api/v1/login");
            var handler = new HttpClientHandler();
            var cookies = new CookieContainer();
            var client = new HttpClient(handler);

            handler.CookieContainer = cookies;

            var response = await client.PostAsJsonAsync(uri, new LoginRequest
            {
                Email = email,
                Password = password
            });

            response.StatusCode.Should().BeEquivalentTo(200);
            return cookies.GetCookies(uri);
        }
    }
}