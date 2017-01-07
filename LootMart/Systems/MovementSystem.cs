using LootMart.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart.Systems
{
    public static class MovementSystem
    {
        public static void InputMovement(KeyboardState currentKey, KeyboardState prevKey, GameTime gameTime, Position positionInfo, Movement movementInfo)
        {
            Vector2 newPosition = positionInfo.OriginPosition;

            if (currentKey.IsKeyDown(Keys.W))
            {
                newPosition.Y -= (float)movementInfo.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (currentKey.IsKeyDown(Keys.A))
            {
                newPosition.X -= (float)movementInfo.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (currentKey.IsKeyDown(Keys.S))
            {
                newPosition.Y += (float)movementInfo.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (currentKey.IsKeyDown(Keys.D))
            {
                newPosition.X += (float)movementInfo.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            positionInfo.OriginPosition = newPosition;
        }

        public static void UpdateMovingEntities(Movement movement, Position position, GameTime gameTime, ECSContainer ecsContainer, Entity entity)
        {
            Vector2 Direction = Vector2.Normalize(movement.TargetPosition - position.OriginPosition);
            float distance = Math.Abs(Vector2.Distance(position.OriginPosition, movement.TargetPosition));
            if (distance > 10)
            {
                position.OriginPosition += Direction * new Vector2((float)movement.Velocity) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                ecsContainer.DelayedActions.Add(new Action(() =>
                {
                    entity.RemoveComponentFlags(ComponentFlags.MOVEMENT);
                    switch (movement.TargetReachedBehavior)
                    {
                        case TargetReachedBehavior.HIDE:
                            entity.RemoveComponentFlags(ComponentFlags.DISPLAY);
                            entity.RemoveComponentFlags(ComponentFlags.POSITION);
                            break;
                        case TargetReachedBehavior.DESTROY:
                            ecsContainer.DestroyEntity(entity);
                            break;
                    }
                }));
            }
        }
    }
}
