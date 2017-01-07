using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.Components
{
    #region Component Boilerplate
    public enum ComponentFlags : int
    {
        IS_PLAYER = 0,
        POSITION = 1,
        LABEL = 2,
        DISPLAY = 3,
        MOVEMENT = 4,
        INVENTORY = 5,
        COLLISION = 6
    }

    public class ECSContainer
    {
        public ECSContainer()
        {
        }

        // Entities
        public int EntityCount { get; private set; } = 0;
        public List<Entity> Entities { get; private set; } = new List<Entity>();

        // Component Arrays
        public Dictionary<Guid, Position> Positions = new Dictionary<Guid, Position>();
        public Dictionary<Guid, Display> Displays = new Dictionary<Guid, Display>();
        public Dictionary<Guid, Movement> Movements = new Dictionary<Guid, Movement>();
        public Dictionary<Guid, Label> Labels = new Dictionary<Guid, Label>();
        public Dictionary<Guid, Inventory> Inventories = new Dictionary<Guid, Inventory>();
        public Dictionary<Guid, Collision> Collisions = new Dictionary<Guid, Collision>();

        // Manager Properties
        public List<Action> DelayedActions { get; private set; } = new List<Action>();

        public Guid CreateEntity(params ComponentFlags[] flags)
        {
            Guid id = Guid.NewGuid();
            this.Entities.Add(new Entity(id, flags));
            return id;
        }

        public Guid AddEntity(BaseEntity entity)
        {
            Guid id = this.CreateEntity(entity.Flags.ToArray());
            if (entity.Position != null) { this.Positions.Add(id, entity.Position); }
            if (entity.Movement != null) { this.Movements.Add(id, entity.Movement); }
            if (entity.Label != null) { this.Labels.Add(id, entity.Label); }
            if (entity.Display != null) { this.Displays.Add(id, entity.Display); }
            if (entity.Inventory != null) { this.Inventories.Add(id, entity.Inventory); }
            if (entity.Collision != null) { this.Collisions.Add(id, entity.Collision); }
            return id;
        }

        public void AppendEntity(BaseEntity additions, Guid id)
        {
            Entity entity = this.Entities.Where(x => x.Id == id).First();
            if (entity != null && additions != null)
            {
                entity.AddComponentFlags(additions.Flags.ToArray());
                if (additions.Position != null) { this.Positions[id] = additions.Position; }
                if (additions.Movement != null) { this.Movements[id] = additions.Movement; }
                if (additions.Label != null) { this.Labels[id] = additions.Label; }
                if (additions.Display != null) { this.Displays[id] = additions.Display; }
                if (additions.Inventory != null) { this.Inventories[id] = additions.Inventory; }
                if (additions.Collision != null) { this.Collisions[id] = additions.Collision; }
            }
        }

        public Entity GetEntity(Guid id)
        {
            return this.Entities.Where(x => x.Id == id).FirstOrDefault();
        }

        public void DestroyEntity(Entity removal)
        {
            this.Entities.Remove(removal);
            this.Positions.Remove(removal.Id);
            this.Movements.Remove(removal.Id);
            this.Displays.Remove(removal.Id);
            this.Labels.Remove(removal.Id);
            this.Inventories.Remove(removal.Id);
            this.Collisions.Remove(removal.Id);
        }

        public void DestroyEntity(Guid id)
        {
            Entity found = this.Entities.Where(x => x.Id == id).FirstOrDefault();
            if (found != null)
            {
                this.DestroyEntity(found);
            }
        }

        public void InvokeDelayedActions()
        {
            foreach (Action action in this.DelayedActions)
            {
                action();
            }
            this.DelayedActions.Clear();
        }
    }
    #endregion

    #region Components
    public class Position
    {
        public Vector2 OriginPosition { get; set; }
    }

    public enum WhenToShowLabel
    {
        ALWAYS,
        PLAYER_CLOSE,
        PLAYER_FAR,
        NEVER
    }

    public class Label
    {
        public string Text;
        public float Scale;
        public Vector2 Origin;
        public Vector2 Displacement;
        public Color Color;
        public SpriteEffects SpriteEffect;
        public float Rotation;
        public WhenToShowLabel WhenToShow;
        public int DistanceRenderBuffer;
    }

    public enum DisplayLayer : int
    {
        BACKGROUND = 100,
        FLOOR = 80,
        NORMAL = 60,
        FOREGROUND = 40,
        SUPER = 20,
        TOP = 0
    }

    public class Display
    {
        public string SpriteSheetKey;
        public int SpriteSheetSize;
        public int SpriteSheetCol;
        public int SpriteSheetRow;
        public Color Color;
        public float Scale;
        public Vector2 Origin;
        public SpriteEffects SpriteEffect;
        public float Rotation;
        public float Opacity;
        public DisplayLayer Layer;
    }

    public enum MovementType
    {
        NONE,
        INPUT,
        AI,
        DIRECTED
    }

    public enum TargetReachedBehavior
    {
        NONE,
        DESTROY,
        HIDE
    }

    public class Movement
    {
        public double BaseVelocity;
        public double Velocity;
        public MovementType MovementType;
        public Vector2 TargetPosition;
        public TargetReachedBehavior TargetReachedBehavior;
    }

    public class Inventory
    {
        public List<Guid> EntitiesOwned;
    }

    public enum CollisionType
    {
        NONE,
        REACTOR,
        ITEM,
        DAMAGE,
        EFFECT
    }

    public class Collision
    {
        public CollisionType CollisionType;
        public List<Guid> CollidedEntities = new List<Guid>();
        public HashSet<Guid> CheckedEntities = new HashSet<Guid>();
        public bool Solid;
        public int CollisionRadius;
    }
    #endregion
}
