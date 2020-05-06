namespace ForbiddenBooks.DatabaseLogic.Tables.Base
{
    public abstract class Person : Interfaces.IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
