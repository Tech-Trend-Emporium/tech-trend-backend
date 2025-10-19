namespace General.Dto.Inventory
{
    public class InventoryResponse
    {
        public int Id { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }
        public string ProductName { get; set; } = null!;
    }
}
