using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AddItemDTO
    {
        public required string ProductId { get; set; }
        public required int Quantity { get; set; }
        public List<SelectedOptionDTO> SelectedOptions { get; set; } = new List<SelectedOptionDTO>();
    }

}
