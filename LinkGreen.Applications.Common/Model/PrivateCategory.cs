namespace LinkGreen.Applications.Common.Model
{
    public class PrivateCategory 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public int Depth { get; set; }
   
    }
}