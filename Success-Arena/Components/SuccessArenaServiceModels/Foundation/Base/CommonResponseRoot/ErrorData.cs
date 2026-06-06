using Newtonsoft.Json;
using SuccessArenaServiceModels.Foundation.Base.Enums;

namespace SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot
{
    public class ErrorData
    {
        [JsonProperty("errorType")]
        public ApiErrorTypeSM ApiErrorType { get; set; }

        [JsonProperty("displayMessage")]
        public string DisplayMessage { get; set; }

        [JsonProperty("additionalProps")]
        public Dictionary<string, object>? AdditionalProps { get; set; }

        public ErrorData()
        {
            AdditionalProps = new Dictionary<string, object>();
        }
    }
}
