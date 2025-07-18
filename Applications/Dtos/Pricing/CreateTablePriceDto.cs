﻿namespace Applications.Dtos.Pricing
{
    public class CreateTablePriceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
     
        public List<TablePriceItemInputDto> Items { get; set; } = [];
    }

}
