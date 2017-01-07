using LootMart.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.Systems
{
    public static class CollisionSystem
    {
        public static List<Entity>[,] CreatePartitionGrid(int cols, int rows)
        {
            int gridHeight = ((rows * Constants.Tiles.TileScale * Constants.Sprites.TileGrid) / Constants.Systems.Collision.MapCellSize) + 5;
            int gridWidth = ((cols * Constants.Tiles.TileScale * Constants.Sprites.TileGrid) / Constants.Systems.Collision.MapCellSize) + 5;
            int gridCells = gridWidth * gridHeight;
            return new List<Entity>[gridWidth, gridHeight];
        }

        public static void CheckForCollisions(ECSContainer ecsContainer, List<Entity>[,] partitionGrid)
        {
            IEnumerable<Entity> collidableEntities = ecsContainer.Entities.Where(x => x.IsCollidable());
            foreach (Entity entity in collidableEntities)
            {
                //Check all 4 corners of the entity, place in grid for each if it's not already in there.
                Collision entityCollision = ecsContainer.Collisions[entity.Id];
                Position entityPosition = ecsContainer.Positions[entity.Id];
                Vector2 nw = new Vector2(entityPosition.OriginPosition.X - entityCollision.CollisionRadius, entityPosition.OriginPosition.Y - entityCollision.CollisionRadius);
                Vector2 ne = new Vector2(entityPosition.OriginPosition.X + entityCollision.CollisionRadius, entityPosition.OriginPosition.Y - entityCollision.CollisionRadius);
                Vector2 sw = new Vector2(entityPosition.OriginPosition.X + entityCollision.CollisionRadius, entityPosition.OriginPosition.Y + entityCollision.CollisionRadius);
                Vector2 se = new Vector2(entityPosition.OriginPosition.X - entityCollision.CollisionRadius, entityPosition.OriginPosition.Y + entityCollision.CollisionRadius);

                Vector2 gridNW = SpatialGridPosition(nw);
                Vector2 gridNE = SpatialGridPosition(ne);
                Vector2 gridSW = SpatialGridPosition(sw);
                Vector2 gridSE = SpatialGridPosition(se);

                AddToBucket(ref partitionGrid, gridNW, entity);
                if (gridNE != gridNW)
                {
                    AddToBucket(ref partitionGrid, gridNE, entity);
                }
                if (gridSE != gridNW && gridSE != gridNE)
                {
                    AddToBucket(ref partitionGrid, gridSE, entity);
                }
                if (gridSW != gridSE && gridSW != gridNW && gridSW != gridNE)
                {
                    AddToBucket(ref partitionGrid, gridSW, entity);
                }
            }

            foreach (List<Entity> entitiesInCell in partitionGrid)
            {
                if (entitiesInCell != null)
                {
                    foreach (Entity entityOne in entitiesInCell)
                    {
                        Collision collisionOne = ecsContainer.Collisions[entityOne.Id];
                        Position positionOne = ecsContainer.Positions[entityOne.Id];
                        Rectangle one = new Rectangle((int)positionOne.OriginPosition.X - collisionOne.CollisionRadius, (int)positionOne.OriginPosition.Y - collisionOne.CollisionRadius, collisionOne.CollisionRadius * 2, collisionOne.CollisionRadius * 2);

                        foreach (Entity entityTwo in entitiesInCell)
                        {
                            if (entityOne != entityTwo)
                            {
                                Collision collisionTwo = ecsContainer.Collisions[entityTwo.Id];
                                Position positionTwo = ecsContainer.Positions[entityTwo.Id];

                                if (!collisionOne.CheckedEntities.Contains(entityTwo.Id) && (collisionOne.CollisionType == CollisionType.REACTOR || collisionTwo.CollisionType == CollisionType.REACTOR))
                                {
                                    Rectangle two = new Rectangle((int)positionTwo.OriginPosition.X - collisionTwo.CollisionRadius, (int)positionTwo.OriginPosition.Y - collisionTwo.CollisionRadius, collisionTwo.CollisionRadius * 2, collisionTwo.CollisionRadius * 2);
                                    if (one.Intersects(two))
                                    {
                                        collisionOne.CollidedEntities.Add(entityTwo.Id);
                                        collisionTwo.CollidedEntities.Add(entityOne.Id);
                                    }
                                    collisionOne.CheckedEntities.Add(entityTwo.Id);
                                    collisionTwo.CheckedEntities.Add(entityOne.Id);
                                }
                            }
                        }
                    }
                    entitiesInCell.Clear();
                }

            }
        }

        private static Vector2 SpatialGridPosition(Vector2 position)
        {
            Vector2 positionInGrid = new Vector2((int)Math.Floor(position.X) / Constants.Systems.Collision.MapCellSize, (int)Math.Floor(position.Y) / Constants.Systems.Collision.MapCellSize);
            if (positionInGrid.X < 0)
            {
                positionInGrid.X = 0;
            }
            if (positionInGrid.Y < 0)
            {
                positionInGrid.Y = 0;
            }
            return positionInGrid;
        }

        private static void AddToBucket(ref List<Entity>[,] partitionGrid, Vector2 position, Entity entity)
        {
            if (partitionGrid[(int)position.X, (int)position.Y] == null)
            {
                partitionGrid[(int)position.X, (int)position.Y] = new List<Entity>();
            }

            partitionGrid[(int)position.X, (int)position.Y].Add(entity);
        }

        public static void HandleCollisions(ECSContainer ecsContainer)
        {
            IEnumerable<Entity> collidableEntities = ecsContainer.Entities.Where(x => x.IsCollidable());
            foreach (Entity entity in collidableEntities)
            {
                Collision collision = ecsContainer.Collisions[entity.Id];
                if (collision.CollisionType == CollisionType.REACTOR)
                {
                    foreach (Guid collidedEntity in collision.CollidedEntities)
                    {
                        Collision collided = ecsContainer.Collisions[collidedEntity];
                        switch (collided.CollisionType)
                        {
                            case CollisionType.ITEM:
                                if (entity.HasComponents(ComponentFlags.IS_PLAYER))
                                {
                                    ecsContainer.DelayedActions.Add(new Action(() =>
                                    {
                                        if (entity.HasComponents(ComponentFlags.INVENTORY))
                                        {
                                            ecsContainer.Inventories[entity.Id].EntitiesOwned.Add(collidedEntity);
                                        }
                                        Entity collidedEntityObject = ecsContainer.Entities.Where(x => x.Id == collidedEntity).First();
                                        collidedEntityObject.AddComponentFlags(ComponentFlags.MOVEMENT);
                                        ecsContainer.Movements[collidedEntity] = new Movement()
                                        {
                                            BaseVelocity = 800,
                                            MovementType = MovementType.DIRECTED,
                                            TargetReachedBehavior = TargetReachedBehavior.HIDE,
                                            Velocity = 800,
                                            TargetPosition = ecsContainer.Positions[entity.Id].OriginPosition
                                        };
                                    }));
                                }
                                break;
                        }
                    }
                }
            }
        }

        public static void ResetCollisions(ref ECSContainer ecsContainer)
        {
            foreach (Entity entity in ecsContainer.Entities.Where(x => x.IsCollidable()))
            {
                ecsContainer.Collisions[entity.Id].CollidedEntities.Clear();
                ecsContainer.Collisions[entity.Id].CheckedEntities.Clear();
            }
        }
    }
}
