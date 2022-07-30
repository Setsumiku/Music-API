namespace Music_API.Entities
{
    public class LinkCollectionWrapper<T> : LinkResourceBase
    {
        public IEnumerable<T> Value { get; set; } = new List<T>();

        public LinkCollectionWrapper()
        {
        }

        public LinkCollectionWrapper(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}