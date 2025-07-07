namespace invoice.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public string CoverImage { get; set; }
        public string Color { get; set; }
        public string Currency { get; set; }
        public bool IsActivated { get; set; }

        //language 
    }
}
