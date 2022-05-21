using AgroBot.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AgroBot.Models.ModelsDB
{
    public class Route : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Kilo { get; set; }
        public string Goods { get; set; }
        public long DriverChatId { get; set; }
        public DateTime AppointDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime FullffilDate { get; set; }
        public long LogicChatId { get; set; }
        public bool IsDeleted { get; set; }
        public List<CheckPoint> Points { get; set; }
    }
}
