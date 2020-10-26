using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using FluentAssertions;
using Homework1.Database.Entities;
using Homework1.Models.Requests;

namespace Homework1.Test.Extensions
{
    public static class HttpClientExtensions
    {
        private static readonly Dictionary<RoleNames, IEnumerable<string>> DefaultCookies =
            new Dictionary<RoleNames, IEnumerable<string>>();

        public static HttpClient WithRole(this HttpClient client, RoleNames? role)
        {
            if (role == null)
            {
                client.DefaultRequestHeaders.Remove("Cookie");
                return client;
            }

            client.DefaultRequestHeaders.Add("Cookie", DefaultCookies[role.Value]);
            return client;
        }

        public static async Task CreateDefaultCookies(this HttpClient client)
        {
            DefaultCookies[RoleNames.User] = await client.Login(TestData.User.Email, TestData.User.Password);
            DefaultCookies[RoleNames.Manager] = await client.Login(TestData.Manager.Email, TestData.Manager.Password);
            DefaultCookies[RoleNames.Admin] = await client.Login(TestData.Admin.Email, TestData.Admin.Password);
        }

        public static Task<HttpResponseMessage> PatchAsJsonAsync(this HttpClient client, string requestUri,
            Type type, object value)
        {
            return client.PatchAsync(requestUri, new ObjectContent(type, value, new JsonMediaTypeFormatter()));
        }

        public static Task<HttpResponseMessage> DeleteAsJsonAsync(this HttpClient client, string requestUri,
            Type type, object value)
        {
            return client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri)
            {
                Content = new ObjectContent(type, value, new JsonMediaTypeFormatter())
            });
        }

        private static async Task<IEnumerable<string>> Login(this HttpClient client, string email, string password)
        {
            var request = new LoginRequest()
            {
                Email = email,
                Password = password
            };

            var response = await client.PostAsJsonAsync("api/v1/login", request);
            response.StatusCode.Should().Be(200);
            return response.Headers.FirstOrDefault(h => h.Key == "Set-Cookie").Value;
        }
    }
}