namespace BBallStats.Data.Entities
{
	public class Team
	{
		public string Id { get; set; }
		public required string Name { get; set; }
		public string? LogoLink { get; set; }
    }
}