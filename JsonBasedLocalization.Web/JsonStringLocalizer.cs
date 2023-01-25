using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace JsonBasedLocalization.Web
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _serializer = new();

        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments))
                    : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new StreamReader(stream);
            using JsonTextReader reader = new JsonTextReader(streamReader);

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }
                var key = reader.Value as string;
                reader.Read();
                var value = _serializer.Deserialize<string>(reader);
                yield return new LocalizedString(key, value);
            }
        }

        private string GetString(string key)
        {
            //Resources/ar-EG.json
            //Resources/en-US.json
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var fullFilePath = Path.GetFullPath(filePath);
            if (File.Exists(fullFilePath))
            {
                var cachekey=$"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                //locale_en-US_welcome
                var cacheValue = _cache.GetString(cachekey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return cacheValue;
                }
                var result = GetValueFromJSON(key, fullFilePath);
                if (!string.IsNullOrEmpty(result))
                {
                    _cache.SetString(cachekey, result);
                }
                return result;
            }
            return String.Empty;

        }

        public string GetValueFromJSON(string propertyName, string filePath)
        {
            if (String.IsNullOrEmpty(propertyName) || String.IsNullOrEmpty(filePath))
            {
                return String.Empty;
            }

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new StreamReader(stream);
            using JsonTextReader reader = new JsonTextReader(streamReader);

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                {
                    reader.Read();
                    return _serializer.Deserialize<string>(reader);
                }
            }

            return String.Empty;

        }
    }
}
