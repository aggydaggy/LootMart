using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.Components
{
    public class Entity
    {
        public Entity(Guid id, params ComponentFlags[] flags)
        {
            this.Id = id;

            foreach (ComponentFlags flag in flags)
            {
                this.ComponentFlags[(int)flag] = true;
            }
        }

        public Guid Id { get; }
        public BitArray ComponentFlags { get; } = new BitArray(Enum.GetNames(typeof(ComponentFlags)).Length);
    }

    // Extension Methods take place of Bit Masking
    public static class EntityExtensions
    {
        public static void AddComponentFlags(this Entity e, params ComponentFlags[] flags)
        {
            foreach (ComponentFlags flag in flags)
            {
                e.ComponentFlags[(int)flag] = true;
            }
        }

        public static void RemoveComponentFlags(this Entity e, params ComponentFlags[] flags)
        {
            foreach (ComponentFlags flag in flags)
            {
                e.ComponentFlags[(int)flag] = false;
            }
        }

        public static bool HasComponents(this Entity e, params ComponentFlags[] flags)
        {
            foreach (ComponentFlags flag in flags)
            {
                if (!e.ComponentFlags[(int)flag])
                {
                    return false;
                }
            }
            return true;
        }


        // Shortcut Extensions
        public static bool HasDrawableSprite(this Entity e)
        {
            return (e.ComponentFlags[(int)ComponentFlags.DISPLAY] && e.ComponentFlags[(int)ComponentFlags.POSITION]);
        }

        public static bool HasDrawableLabel(this Entity e)
        {
            return (e.ComponentFlags[(int)ComponentFlags.LABEL] && e.ComponentFlags[(int)ComponentFlags.POSITION]);
        }

        public static bool IsMovable(this Entity e)
        {
            return (e.ComponentFlags[(int)ComponentFlags.MOVEMENT] && e.ComponentFlags[(int)ComponentFlags.POSITION]);
        }

        public static bool IsCollidable(this Entity e)
        {
            return (e.ComponentFlags[(int)ComponentFlags.DISPLAY] && e.ComponentFlags[(int)ComponentFlags.POSITION] && e.ComponentFlags[(int)ComponentFlags.COLLISION]);
        }
    }
}
