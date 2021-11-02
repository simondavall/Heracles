namespace Heracles.Domain
{
    public abstract class BaseEntity
    {
        // This can be modified to BaseEntity<TId> to support multiple key types (e.g. Guid)
        public int Id { get; set; }

    }
}
