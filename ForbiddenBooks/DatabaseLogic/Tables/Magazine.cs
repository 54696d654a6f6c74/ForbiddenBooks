namespace ForbiddenBooks.DatabaseLogic.Tables
{
    public class Magazine : Base.Item
    {
        public virtual Genre Genre {get; set;}
        public decimal Price { get; set; }
        public int AccsessLevel { get; set; }
        public virtual Author Author { get; set; }

        public virtual User userOwner { get; set; }
        public virtual Market marketOwner { get; set; }
    }
}
