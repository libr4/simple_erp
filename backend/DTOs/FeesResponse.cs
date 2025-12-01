namespace Backend.DTOs
{
    public class FeesResponse
    {
        public decimal ValorOriginal { get; set; }
        public int DiasAtraso { get; set; }
        public decimal Juros { get; set; }
        public decimal ValorComJuros { get; set; }
    }
}
