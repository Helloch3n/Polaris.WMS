namespace Polaris.WMS.MasterData.Application.Contracts.Locations.Dtos
{
    public class BatchCreateLocationDto
    {
        public Guid ZoneId { get; set; }
        public Guid WarehouseId { get; set; }
        public string AislePrefix { get; set; }
        public int RackCount { get; set; }
        public int LevelCount { get; set; }
    }
}

