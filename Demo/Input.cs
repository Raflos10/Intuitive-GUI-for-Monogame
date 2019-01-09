using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Demo
{
    public static class Input
    {
        #region Names
        public enum Actions
        {
            Up,
            Down,
            Left,
            Right,
            OK,
            Cancel,
            Pause
        }

        private static readonly string[] actionNames =
        {
            "Up",
            "Down",
            "Left",
            "Right",
            "OK",
            "Cancel",
            "Pause"
        };

        public static string GetActionName(Actions action)
        {
            int index = (int)action;

            if ((index < 0) || (index > actionNames.Length))
            {
                throw new ArgumentException("action");
            }

            return actionNames[index];
        }

        #endregion

        #region Keyboard

        public static KeyboardState KeyboardState { get; private set; }
        public static KeyboardState KeyboardStateLast { get; private set; }

        public static bool IsKeyPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyTriggered(Keys key)
        {
            return (KeyboardState.IsKeyDown(key)) && (!KeyboardStateLast.IsKeyDown(key));
        }

        #endregion

        #region GamePad

        public static GamePadState GamePadState { get; private set; }
        public static GamePadState GamePadStateLast { get; private set; }
        const float analogLimit = 0.5f;

        public enum GamePadButtons
        {
            Start,
            Back,
            A,
            B,
            X,
            Y,
            Up,
            Down,
            Left,
            Right,
            LeftShoulder,
            RightShoulder,
            LeftTrigger,
            RightTrigger,
        }

        #region GamePadButton Pressed Queries

        public static bool IsGamePadStartPressed()
        {
            return (GamePadState.Buttons.Start == ButtonState.Pressed);
        }

        public static bool IsGamePadBackPressed()
        {
            return (GamePadState.Buttons.Back == ButtonState.Pressed);
        }

        public static bool IsGamePadAPressed()
        {
            return (GamePadState.Buttons.A == ButtonState.Pressed);
        }

        public static bool IsGamePadBPressed()
        {
            return (GamePadState.Buttons.B == ButtonState.Pressed);
        }

        public static bool IsGamePadXPressed()
        {
            return (GamePadState.Buttons.X == ButtonState.Pressed);
        }

        public static bool IsGamePadYPressed()
        {
            return (GamePadState.Buttons.Y == ButtonState.Pressed);
        }

        public static bool IsGamePadLeftShoulderPressed()
        {
            return (GamePadState.Buttons.LeftShoulder == ButtonState.Pressed);
        }

        public static bool IsGamePadRightShoulderPressed()
        {
            return (GamePadState.Buttons.RightShoulder == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadUpPressed()
        {
            return (GamePadState.DPad.Up == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadDownPressed()
        {
            return (GamePadState.DPad.Down == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadLeftPressed()
        {
            return (GamePadState.DPad.Left == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadRightPressed()
        {
            return (GamePadState.DPad.Right == ButtonState.Pressed);
        }

        public static bool IsGamePadLeftTriggerPressed()
        {
            return (GamePadState.Triggers.Left > analogLimit);
        }

        public static bool IsGamePadRightTriggerPressed()
        {
            return (GamePadState.Triggers.Right > analogLimit);
        }

        public static bool IsGamePadLeftStickUpPressed()
        {
            return (GamePadState.ThumbSticks.Left.Y > analogLimit);
        }

        public static bool IsGamePadLeftStickDownPressed()
        {
            return (-1f * GamePadState.ThumbSticks.Left.Y > analogLimit);
        }

        public static bool IsGamePadLeftStickLeftPressed()
        {
            return (-1f * GamePadState.ThumbSticks.Left.X > analogLimit);
        }

        public static bool IsGamePadLeftStickRightPressed()
        {
            return (GamePadState.ThumbSticks.Left.X > analogLimit);
        }

        public static bool IsGamePadButtonPressed(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartPressed();

                case GamePadButtons.Back:
                    return IsGamePadBackPressed();

                case GamePadButtons.A:
                    return IsGamePadAPressed();

                case GamePadButtons.B:
                    return IsGamePadBPressed();

                case GamePadButtons.X:
                    return IsGamePadXPressed();

                case GamePadButtons.Y:
                    return IsGamePadYPressed();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderPressed();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderPressed();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerPressed();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerPressed();

                case GamePadButtons.Up:
                    return IsGamePadDPadUpPressed();

                case GamePadButtons.Down:
                    return IsGamePadDPadDownPressed();

                case GamePadButtons.Left:
                    return IsGamePadDPadLeftPressed();

                case GamePadButtons.Right:
                    return IsGamePadDPadRightPressed();
            }

            return false;
        }


        #endregion

        #region GamePadButton Triggered Queries

        public static bool IsGamePadStartTriggered()
        {
            return ((GamePadState.Buttons.Start == ButtonState.Pressed) &&
              (GamePadStateLast.Buttons.Start == ButtonState.Released));
        }

        public static bool IsGamePadBackTriggered()
        {
            return ((GamePadState.Buttons.Back == ButtonState.Pressed) &&
              (GamePadStateLast.Buttons.Back == ButtonState.Released));
        }

        public static bool IsGamePadATriggered()
        {
            return ((GamePadState.Buttons.A == ButtonState.Pressed) &&
              (GamePadStateLast.Buttons.A == ButtonState.Released));
        }

        public static bool IsGamePadBTriggered()
        {
            return ((GamePadState.Buttons.B == ButtonState.Pressed) &&
              (GamePadStateLast.Buttons.B == ButtonState.Released));
        }

        public static bool IsGamePadXTriggered()
        {
            return ((GamePadState.Buttons.X == ButtonState.Pressed) &&
              (GamePadStateLast.Buttons.X == ButtonState.Released));
        }

        public static bool IsGamePadYTriggered()
        {
            return ((GamePadState.Buttons.Y == ButtonState.Pressed) &&
              (GamePadStateLast.Buttons.Y == ButtonState.Released));
        }

        public static bool IsGamePadLeftShoulderTriggered()
        {
            return (
                (GamePadState.Buttons.LeftShoulder == ButtonState.Pressed) &&
                (GamePadStateLast.Buttons.LeftShoulder == ButtonState.Released));
        }

        public static bool IsGamePadRightShoulderTriggered()
        {
            return (
                (GamePadState.Buttons.RightShoulder == ButtonState.Pressed) &&
                (GamePadStateLast.Buttons.RightShoulder == ButtonState.Released));
        }

        public static bool IsGamePadDPadUpTriggered()
        {
            return ((GamePadState.DPad.Up == ButtonState.Pressed) &&
              (GamePadStateLast.DPad.Up == ButtonState.Released));
        }

        public static bool IsGamePadDPadDownTriggered()
        {
            return ((GamePadState.DPad.Down == ButtonState.Pressed) &&
              (GamePadStateLast.DPad.Down == ButtonState.Released));
        }

        public static bool IsGamePadDPadLeftTriggered()
        {
            return ((GamePadState.DPad.Left == ButtonState.Pressed) &&
              (GamePadStateLast.DPad.Left == ButtonState.Released));
        }

        public static bool IsGamePadDPadRightTriggered()
        {
            return ((GamePadState.DPad.Right == ButtonState.Pressed) &&
              (GamePadStateLast.DPad.Right == ButtonState.Released));
        }

        public static bool IsGamePadLeftTriggerTriggered()
        {
            return ((GamePadState.Triggers.Left > analogLimit) &&
                (GamePadStateLast.Triggers.Left < analogLimit));
        }

        public static bool IsGamePadRightTriggerTriggered()
        {
            return ((GamePadState.Triggers.Right > analogLimit) &&
                (GamePadStateLast.Triggers.Right < analogLimit));
        }

        public static bool IsGamePadLeftStickUpTriggered()
        {
            return ((GamePadState.ThumbSticks.Left.Y > analogLimit) &&
                (GamePadStateLast.ThumbSticks.Left.Y < analogLimit));
        }

        public static bool IsGamePadLeftStickDownTriggered()
        {
            return ((-1f * GamePadState.ThumbSticks.Left.Y > analogLimit) &&
                (-1f * GamePadStateLast.ThumbSticks.Left.Y < analogLimit));
        }

        public static bool IsGamePadLeftStickLeftTriggered()
        {
            return ((-1f * GamePadState.ThumbSticks.Left.X > analogLimit) &&
                (-1f * GamePadStateLast.ThumbSticks.Left.X < analogLimit));
        }

        public static bool IsGamePadLeftStickRightTriggered()
        {
            return ((GamePadState.ThumbSticks.Left.X > analogLimit) &&
                (GamePadStateLast.ThumbSticks.Left.X < analogLimit));
        }

        public static bool IsGamePadButtonTriggered(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartTriggered();

                case GamePadButtons.Back:
                    return IsGamePadBackTriggered();

                case GamePadButtons.A:
                    return IsGamePadATriggered();

                case GamePadButtons.B:
                    return IsGamePadBTriggered();

                case GamePadButtons.X:
                    return IsGamePadXTriggered();

                case GamePadButtons.Y:
                    return IsGamePadYTriggered();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderTriggered();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderTriggered();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerTriggered();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerTriggered();

                case GamePadButtons.Up:
                    return IsGamePadDPadUpTriggered() ||
                        IsGamePadLeftStickUpTriggered();

                case GamePadButtons.Down:
                    return IsGamePadDPadDownTriggered() ||
                        IsGamePadLeftStickDownTriggered();

                case GamePadButtons.Left:
                    return IsGamePadDPadLeftTriggered() ||
                        IsGamePadLeftStickLeftTriggered();

                case GamePadButtons.Right:
                    return IsGamePadDPadRightTriggered() ||
                        IsGamePadLeftStickRightTriggered();
            }

            return false;
        }


        #endregion

        #endregion

        #region Mapping

        public class ActionMap
        {
            //these were lists before
            public Keys key;
            public GamePadButtons gamePadButton;
        }

        public static ActionMap[] ActionMaps { get; set; }

        public static ActionMap[] ResetActionsToDefault()
        {
            ActionMap[] actionMaps = new ActionMap[Enum.GetNames(typeof(Actions)).Length];

            actionMaps[(int)Actions.OK] = new ActionMap();
            actionMaps[(int)Actions.OK].key = Keys.X;
            actionMaps[(int)Actions.OK].gamePadButton = GamePadButtons.A;

            actionMaps[(int)Actions.Cancel] = new ActionMap();
            actionMaps[(int)Actions.Cancel].key = Keys.C;
            actionMaps[(int)Actions.Cancel].gamePadButton = GamePadButtons.B;

            actionMaps[(int)Actions.Up] = new ActionMap();
            actionMaps[(int)Actions.Up].key = Keys.Up;
            actionMaps[(int)Actions.Up].gamePadButton = GamePadButtons.Up;

            actionMaps[(int)Actions.Down] = new ActionMap();
            actionMaps[(int)Actions.Down].key = Keys.Down;
            actionMaps[(int)Actions.Down].gamePadButton = GamePadButtons.Down;

            actionMaps[(int)Actions.Left] = new ActionMap();
            actionMaps[(int)Actions.Left].key = Keys.Left;
            actionMaps[(int)Actions.Left].gamePadButton = GamePadButtons.Left;

            actionMaps[(int)Actions.Right] = new ActionMap();
            actionMaps[(int)Actions.Right].key = Keys.Right;
            actionMaps[(int)Actions.Right].gamePadButton = GamePadButtons.Right;

            actionMaps[(int)Actions.Pause] = new ActionMap();
            actionMaps[(int)Actions.Pause].key = Keys.Space;
            actionMaps[(int)Actions.Pause].gamePadButton = GamePadButtons.Start;

            return actionMaps;
        }

        public static bool IsActionPressed(Actions action)
        {
            return IsActionMapPressed(ActionMaps[(int)action]);
        }

        public static bool IsActionTriggered(Actions action)
        {
            return IsActionMapTriggered(ActionMaps[(int)action]);
        }

        private static bool IsActionMapPressed(ActionMap actionMap)
        {
            //used to be for loops to check multiple keys/buttons

            if (IsKeyPressed(actionMap.key))
                return true;
            if (GamePadState.IsConnected)
            {
                if (IsGamePadButtonPressed(actionMap.gamePadButton))
                    return true;
            }
            return false;
        }

        private static bool IsActionMapTriggered(ActionMap actionMap)
        {
            //this was also for loops

            if (IsKeyTriggered(actionMap.key))
                return true;
            if (GamePadState.IsConnected)
            {
                if (IsGamePadButtonTriggered(actionMap.gamePadButton))
                    return true;
            }
            return false;
        }

        #endregion

        #region Mouse

        public static MouseState MouseState { get; private set; }
        public static MouseState MouseStateLast { get; private set; }

        #region MouseButton Pressed Queries

        public static bool IsLeftMouseButtonDown()
        {
            return (MouseState.LeftButton == ButtonState.Pressed);
        }

        public static bool IsRightMouseButtonDown()
        {
            return (MouseState.RightButton == ButtonState.Pressed);
        }

        #endregion

        #region MouseButton Triggered Queries


        public static bool IsLeftMouseButtonTriggered()
        {
            return ((MouseState.LeftButton == ButtonState.Pressed) &&
              (MouseStateLast.LeftButton == ButtonState.Released));
        }

        public static bool IsRightMouseButtonTriggered()
        {
            return ((MouseState.RightButton == ButtonState.Pressed) &&
              (MouseStateLast.RightButton == ButtonState.Released));
        }

        #endregion

        #region MouseButton Released Queries

        public static bool IsLeftMouseButtonReleased()
        {
            return ((MouseState.LeftButton == ButtonState.Released) &&
              (MouseStateLast.LeftButton == ButtonState.Pressed));
        }

        public static bool IsRightMouseButtonReleased()
        {
            return ((MouseState.RightButton == ButtonState.Released) &&
              (MouseStateLast.RightButton == ButtonState.Pressed));
        }

        #endregion

        #endregion

        public static void Initialize()
        {
            ActionMaps = ResetActionsToDefault();
        }

        public static void Update()
        {
            KeyboardStateLast = KeyboardState;
            KeyboardState = Keyboard.GetState();

            GamePadStateLast = GamePadState;
            GamePadState = GamePad.GetState(PlayerIndex.One);

            MouseStateLast = MouseState;
            MouseState = Mouse.GetState();
        }
    }
}
