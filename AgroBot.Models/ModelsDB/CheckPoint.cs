using AgroBot.Models.Interfaces;
using System;

namespace AgroBot.Models.ModelsDB
{
    public class CheckPoint : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsFullfil { get; set; }
    }
}
