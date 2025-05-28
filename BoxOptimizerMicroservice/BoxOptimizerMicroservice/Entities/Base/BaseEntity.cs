using System.ComponentModel.DataAnnotations;

namespace BoxOptimizerMicroservice.Entities.Base
{
    public abstract class BaseEntity
    {
        [Key]
        private Guid _id { get; set; }

        public BaseEntity()
        {
            _id = Guid.NewGuid();
        }

        public Guid GetGuid() { return _id; }
    }
}
