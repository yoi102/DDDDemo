namespace JWT
{
    public class JWTOptions
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string Key { get; set; }
        public int ExpireSeconds { get; set; }
    }
}
