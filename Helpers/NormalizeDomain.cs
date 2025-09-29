namespace invoice.Helpers
{
    public static class NormalizeDomain
    {
        public static string Normalize(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Domain cannot be null or empty.", nameof(domain));

            domain = domain.Trim();

            if (!domain.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !domain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (domain.Contains("localhost") || domain.Contains("127.0.0.1"))
                    domain = "http://" + domain;
                else
                    domain = "https://" + domain;
            }

            if (!domain.EndsWith("/"))
                domain += "/";

            return domain;
        }
    }
}
