using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootMart
{
    public class Camera
    {
        public float Rotation;
        public float Scale;
        public Vector2 Position;
        public Rectangle Bounds;
        public Vector2 TargetPosition;
        public Guid TargetEntity;
        public Viewport FullViewport;
        public Matrix CurrentMatrix;
        public Matrix CurrentInverseMatrix;

        public static readonly Vector2 Velocity = new Vector2(1300, 1300);

        public Camera(Viewport viewport, Vector2 position, float rotation, float scale)
        {
            FullViewport = viewport;
            Rotation = rotation;
            Scale = scale;
            Position = position;
            Bounds = FullViewport.Bounds; // Used to set a non-UI Bounds if needed.
        }

        public void ResetCamera()
        {
            this.Scale = 1f;
            this.Position = FullViewport.Bounds.Center.ToVector2();
            this.Bounds = FullViewport.Bounds;
            this.Rotation = 0f;
            this.TargetPosition = this.Position;
        }

        public Rectangle TranslateToGrid(int rows, int cols, int tileSize, int buffer = 0)
        {
            int cameraOffsetX = (int)this.Position.X / tileSize;
            int cameraOffsetY = (int)this.Position.Y / tileSize;
            int cameraWidth = this.Bounds.Width / tileSize;
            int cameraHeight = this.Bounds.Height / tileSize;


            int x = (cameraOffsetX - cameraWidth / 2) - buffer;
            x = x < 0 ? 0 : x;
            int y = (cameraOffsetY - cameraHeight / 2) - buffer;
            y = y < 0 ? 0 : y;
            int width = cameraOffsetX + (int)Math.Ceiling((double)cameraWidth / 2) + buffer;
            width = width >= cols ? cols : width;
            int height = cameraOffsetY + (int)Math.Ceiling((double)cameraHeight / 2) + buffer;
            height = height >= rows ? rows : height;

            return new Rectangle(x, y, width, height);
        }

        public Matrix GetMatrix()
        {
            Matrix transform =
                Matrix.CreateTranslation(new Vector3((int)-Position.X, (int)-Position.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale) *
                Matrix.CreateTranslation(new Vector3((int)(Bounds.Width * 0.5f), (int)(Bounds.Height * 0.5f), 0));
            //M41 and M42 are for translation, cast them to ints to avoid jitteryness
            transform.M41 = (int)transform.M41;
            transform.M42 = (int)transform.M42;
            return transform;
        }

        public Matrix GetInverseMatrix()
        {
            return
                Matrix.Invert(
                    Matrix.CreateTranslation(new Vector3((int)-Position.X, (int)-Position.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(Scale) *
                    Matrix.CreateTranslation(new Vector3((int)(Bounds.Width * 0.5f), (int)(Bounds.Height * 0.5f), 0)));
        }

        public Vector2 ScreenToWorld(Point point)
        {
            return Vector2.Transform(point.ToVector2(), this.GetInverseMatrix());
        }

        public Vector2 WorldToScreen(Point point)
        {
            return Vector2.Transform(point.ToVector2(), this.GetMatrix());
        }

        public bool IsInView(Rectangle item)
        {
            return !Rectangle.Intersect(this.Bounds, item).IsEmpty;
        }
    }
}
