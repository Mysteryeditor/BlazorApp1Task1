using Microsoft.JSInterop;
using System.Text.Json;
namespace BlazorApp1Task1.Authentication
{
    public class BrowserStorageService
    {
        private const string StorageType = "sessionStorage";
        private readonly IJSRuntime _runtime;
        public BrowserStorageService(IJSRuntime jSRuntime)
        {
            _runtime = jSRuntime;
        }

        public async Task SaveToStorage<TData>(string key, TData value)
        {
            await _runtime.InvokeVoidAsync($"{StorageType}.setItem", key, Serialize(value));
        }

        public async Task<TData?> GetFromStorage<TData>(string key)
        {
            var serializedData = await _runtime.InvokeAsync<string?>($"{StorageType}.getItem", key);
            return Deserialize<TData>(serializedData);
        }
        public async Task RemoveFromStorage<TData>(string key)
        {
            await _runtime.InvokeVoidAsync($"{StorageType}.removeItem", key);
        }
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new();

        private static string Serialize<TData>(TData data)=>JsonSerializer.Serialize(data, _jsonSerializerOptions);

        private static TData? Deserialize<TData>(string? jsonData)
        {
            if (!String.IsNullOrEmpty(jsonData)) {
                return JsonSerializer.Deserialize<TData>(jsonData, _jsonSerializerOptions);
            }
            return default;
        }
    }
}
