using Newtonsoft.Json;

namespace LinkGreen.Applications.Common.Model
{
    internal class ApiResult<T>
    {
        public string Result { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }

        public T Item => JsonConvert.DeserializeObject<T>(Result);
    }
}
