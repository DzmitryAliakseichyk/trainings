namespace WebApi.Models.Settings
{
    public class JwtSettingsModel
    {
        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpitarionsOffsetInMinutes { get; set; }
    }
}
