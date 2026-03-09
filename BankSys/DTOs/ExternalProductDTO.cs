namespace BankSys.DTOs
{
    public class ExternalProductsResponse
    {
        public List<ExternalProductDTO> Products { get; set; }
    }


    public class ExternalProductDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
    }
}
