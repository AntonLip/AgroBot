using AgroBot.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AgroBot.Models.ModelsDB
{
    public class ApplicationUser : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsRegistred { get; set; }
        public List<string> Role { get; set; }
    }
}
