namespace invoice.Core.DTO.PaymentResponse.TapPayments
{
    public class TapUserDto
    {
        public string Email { get; set; }
        public string BusinessName { get; set; }
        public string Country { get; set; } = "SA";
        public string CountryCode { get; set; } = "+966";
        public string PhoneNumber { get; set; }

    }


    public class CreateLeadDto
    {
        public string Country { get; set; } = "AE";
        public BrandMinimalDto Brand { get; set; }

    }
        public class BrandMinimalDto
        {
            public List<BrandNameDto> Name { get; set; } = new List<BrandNameDto>();
        }

        public class BrandNameDto
        {
            public string Text { get; set; } = "brandName";
            public string Lang { get; set; } = "en";
        }



        public class CreateConnectDto
        {
            public string Scope { get; set; } = "merchant";
            public List<string> Data { get; set; } = new() { "operation", "brand", "entity", "merchant" };
            public LeadRefDto Lead { get; set; }
            public RedirectDto Redirect { get; set; }
            public PostDto Post { get; set; }
        }

        public class LeadRefDto
        {
            public string Id { get; set; }
        }

        public class RedirectDto
        {
            public string Url { get; set; }
        }

        public class PostDto
        {
            public string Url { get; set; }
          //  public string UserId { get; set; }
        }

        public class OnboardingRequest
        {
            public string UserId { get; set; }
            public string AccountId { get; set; }
            public string Status { get; set; }
        }

    }

