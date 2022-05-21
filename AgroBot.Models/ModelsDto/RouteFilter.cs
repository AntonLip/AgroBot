using System;

namespace AgroBot.Models.ModelsDto
{
    public class RouteFilter
    {
        public string Name { get; set; }
        public int Kilo { get; set; }
        public string Goods { get; set; }
        public long DriverChatId { get; set; }
        public DateTime AppointDateStart { get; set; }
        public DateTime AppointDateEnd { get; set; }
        public DateTime CreatedDateStart { get; set; }
        public DateTime CreatedDateEnd { get; set; }

        public DateTime FullffilDateStart { get; set; }
        public DateTime FullffilDateEnd { get; set; }
        public long LogicChatId { get; set; }
    }
}
