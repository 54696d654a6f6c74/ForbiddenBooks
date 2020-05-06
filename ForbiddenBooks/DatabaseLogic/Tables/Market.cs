using System.Collections.Generic;

namespace ForbiddenBooks.DatabaseLogic.Tables
{
    public class Market : Base.Item, Interfaces.IRelatedByCollection
    {
        private ICollection<Magazine> _magazines;
        public virtual ICollection<Magazine> Magazines
        {
            get
            {
                if(_magazines == null)
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