﻿using Core.Models.Works;

namespace Core.Models.Pricing
{
    public class TablePriceItem
    {
        public int TablePriceItemId { get; set; }

        public required decimal Price { get; set; }
        public int TablePriceId { get; set; }
        public TablePrice TablePrice { get; set; } = null!;
        public int WorkTypeId { get; set; }
        public WorkType WorkType { get; set; } = null!;


    }
}