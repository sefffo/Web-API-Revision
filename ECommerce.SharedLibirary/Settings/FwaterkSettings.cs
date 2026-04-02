namespace ECommerce.SharedLibirary.Settings
{

    public class FawaterakSettings
    {
        public string ApiKey { get; set; } = default!;
        public string ProviderKey { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
        public string SuccessUrl { get; set; } = default!;
        public string FailUrl { get; set; } = default!;
        public string PendingUrl { get; set; } = default!;
        public string CallbackUrl { get; set; } = default!;
    }
}
