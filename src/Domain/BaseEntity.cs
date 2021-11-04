namespace Heracles.Domain
{
    public abstract class BaseEntity<T> where T : notnull
    {
        // This can be modified to BaseEntity<TId> to support multiple key types (e.g. Guid)
        public T Id { get; set; }

    }
}
