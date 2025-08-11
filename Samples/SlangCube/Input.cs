using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SlangCube
{
    public class Input
    {
        //Used to track change in mouse movement to allow for moving of the Camera
        public Vector2 LastMousePosition;
        public IKeyboard primaryKeyboard;

        IWindow Window;
        Func<Camera> GetCamera;
        Action<Camera> SetCamera;

        public Input(IWindow window, Func<Camera> getCamera, Action<Camera> setCamera)
        {
            Window = window;
            GetCamera = getCamera;
            SetCamera = setCamera;
        }

        public void Load()
        {
            IInputContext input = Window.CreateInput();
            primaryKeyboard = input.Keyboards.FirstOrDefault();
            if (primaryKeyboard != null)
            {
                primaryKeyboard.KeyDown += KeyDown;
            }
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
            }
        }

        void OnMouseMove(IMouse mouse, Vector2 position)
        {
            var lookSensitivity = 0.1f;
            if (LastMousePosition == default)
            {
                LastMousePosition = position;
            }
            else
            {
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                Camera newValue = GetCamera();
                newValue.Yaw += xOffset;
                newValue.Pitch -= yOffset;

                //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
                newValue.Pitch = Math.Clamp(newValue.Pitch, -89.0f, 89.0f);

                newValue.Direction.X = MathF.Cos(MathHelper.DegreesToRadians(GetCamera().Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(GetCamera().Pitch));
                newValue.Direction.Y = MathF.Sin(MathHelper.DegreesToRadians(GetCamera().Pitch));
                newValue.Direction.Z = MathF.Sin(MathHelper.DegreesToRadians(GetCamera().Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(GetCamera().Pitch));
                newValue.Front = Vector3.Normalize(GetCamera().Direction);

                SetCamera(newValue);
            }
        }

        void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            //We don't want to be able to zoom in too close or too far away so clamp to these values
            Camera newValue = GetCamera();
            newValue.Zoom = Math.Clamp(GetCamera().Zoom - scrollWheel.Y, 1.0f, 45f);
            SetCamera(newValue);
        }

        void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            if (key == Key.Escape)
            {
                Window.Close();
            }
            else if (key == Key.R)
            {
                //throw new NotImplementedException();
            }
            else if (key == Key.B)
            {
                //throw new NotImplementedException();
            }
        }
    }
}
