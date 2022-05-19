using System;
using System.Collections.Generic;

namespace AgroBot.Models
{
    public class RouteDto
    {
        public string Name { get; set; }
        public int Kilo { get; set; }
        public string Goods { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CheckPointDto> Points { get; set; }
    }
}
