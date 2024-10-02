namespace LinkGreen.Applications.Common.Model
{
    public class CreateCategoryRequest
    {
        public string data { get; set; }

        public int? ParentCategoryId { get; set; }

        public int Depth { get; set; }
    }
}
