namespace ForbiddenBooks.DatabaseLogic.Tables.Base
{
    public abstract class Item : Interfaces.IEntity
    {
        public int Id { get; set; }
        public string Name { get;  set; }
    }
}
