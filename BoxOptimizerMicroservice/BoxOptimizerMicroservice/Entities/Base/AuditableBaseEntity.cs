namespace BoxOptimizerMicroservice.Entities.Base
{
    public class AuditableBaseEntity : BaseEntity 
    {
        private DateTime _creationTime { get; set; }
        private DateTime? _LastModifiedTime { get; set; }

        public AuditableBaseEntity() : base()
        {
            _creationTime = DateTime.UtcNow;
        }

        public void setModifiedTime()
        {
            _LastModifiedTime = DateTime.UtcNow;
        }

        public DateTime getCreationTime() { return _creationTime; }
        public DateTime? getLastModifiedTime() { return _LastModifiedTime; }
    }
}
