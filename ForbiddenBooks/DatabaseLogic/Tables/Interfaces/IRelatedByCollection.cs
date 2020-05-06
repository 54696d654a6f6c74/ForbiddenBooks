using System.Collections.Generic;

namespace ForbiddenBooks.DatabaseLogic.Tables.Interfaces
{
    public interface IRelatedByCollection
    {
        public ICollection<Magazine> Magazines { get;  set; }
    }
}
