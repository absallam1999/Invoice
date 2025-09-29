namespace invoice.Core.DTO.Page
{
    public class PageReadDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }
        public bool InFooter { get; set; }
        public bool InHeader { get; set; }


        //public string LanguageId { get; set; }

        ////public string StoreName { get; set; }
        //public string LanguageName { get; set; }
    }
}