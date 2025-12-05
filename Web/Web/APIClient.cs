using DataModels.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Web
{
    public static class APIClient
    {
        private static readonly HttpClient _client = new();
        public static UserViewModel? CurrentUser { get; set; } = null;

        public static void Initialize(IConfiguration configuration)
        {
            _client.BaseAddress = new Uri(configuration["IPAddress"]);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Асинхронный GET запрос
        public static async Task<T?> GetAsync<T>(string requestUrl)
        {
            try
            {
                var response = await _client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex.Message}");
            }
        }

        // Асинхронный POST запрос (возвращает десериализованный ответ)
        public static async Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUrl, TRequest model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(requestUrl, data);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(result);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex.Message}");
            }
        }

        // Асинхронный DELETE запрос
        public static async Task<bool> DeleteAsync(string requestUrl)
        {
            try
            {
                var response = await _client.DeleteAsync(requestUrl);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении: {ex.Message}");
            }
        }

        // Асинхронный PUT запрос
        public static async Task<TResponse?> PutAsync<TRequest, TResponse>(string requestUrl, TRequest model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(requestUrl, data);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении: {ex.Message}");
            }
        }

        // Метод для получения базового URL (для использования в JavaScript)
        public static string GetBaseUrl()
        {
            return _client.BaseAddress?.ToString() ?? "";
        }
    }
}