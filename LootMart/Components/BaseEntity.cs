using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.Components
{
    public class BaseEntity
    {
        public BaseEntity()
        {

        }

        public BaseEntity(params ComponentFlags[] flags)
        {
            foreach (ComponentFlags flag in flags)
            {
                Flags.Add(flag);
            }
        }

        public List<ComponentFlags> Flags { get; private set; } = new List<ComponentFlags>();
        public Movement Movement;
        public Position Position;
        public Display Display;
        public Label Label;
        public Inventory Inventory;
        public Collision Collision;
    }
}
