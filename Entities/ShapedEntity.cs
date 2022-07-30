using Music_API.Entities;

namespace Music_Api.Entities
{
    public class ShapedEntity
    {
		public ShapedEntity()
		{
			Entity = new Entity();
		}

		public int Id { get; set; }
		public Entity Entity { get; set; }
	}
}