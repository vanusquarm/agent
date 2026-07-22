namespace Statement.Web.Helpers
{
    public class DateFormatAttribute : Attribute
    {
        public string Format { get; set; } = "yyyy-MM-dd";
        public string Example { get; set; }
        public string Description { get; set; }

        public DateFormatAttribute(string example, string description = "")
        {
            Example = example;
            Description = description;
        }
    }
}
