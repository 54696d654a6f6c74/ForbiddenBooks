using System.Collections.Generic;

namespace ForbiddenBooks.DatabaseLogic.Tables
{
    public class User : Base.Person, Interfaces.IRelatedByCollection
    {
        public int AcessLevel { get; set; }
        public decimal Balance { get; set; }
        private ICollection<Magazine> _magazines;
        public virtual ICollection<Magazine> Magazines {
           get
           {
                if (_magazines == null)
                    _magazines = new List<Magazine>();
                return _magazines;
           }
           set
           {
                _magazines = value;
           }
        }
    }
}
