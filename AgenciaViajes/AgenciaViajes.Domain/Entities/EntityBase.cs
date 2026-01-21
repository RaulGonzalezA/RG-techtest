using System;
using System.Collections.Generic;
using System.Text;

namespace AgenciaViajes.Domain.Entities
{
    public class EntityBase
    {
        private EntityBase() { 
            Id = string.Empty; 
        }

        public EntityBase(string id) {
            Id = id;
        }
        public string Id { get; private set; }
    }
}
