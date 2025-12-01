using System.Collections.Generic;

namespace Backend.DTOs
{
    public class VendaDto
    {
        public string Vendedor { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    public class CommissionRequest
    {
        public List<VendaDto> Vendas { get; set; } = new List<VendaDto>();
    }
}
