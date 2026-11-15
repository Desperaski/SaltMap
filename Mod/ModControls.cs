using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SaltMap
{
    internal static class ModControls
    {
        [DataContract]
        public class ActionBinding
        {
            [DataMember] public string Keyboard = "";
            [DataMember] public string Gamepad = "";
        }

        [DataContract]
        public class ControlConfig
        {
            [DataMember] public Dictionary<string, ActionBinding> Bindings = new Dictionary<string, ActionBinding>();
        }

        static ControlConfig config = new ControlConfig();

        public static void Load(string modDir)
        {
            string path = Path.Combine(modDir, "SaltMapControls.json");
            if (!File.Exists(path)) { CreateDefault(path); return; }

            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ControlConfig));
                    config = (ControlConfig)ser.ReadObject(fs);
                }
                Log.Info("controls: loaded from SaltMapControls.json");
            }
            catch (Exception e) { Log.ErrorOnce("controls load", e); }
        }

        static void CreateDefault(string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"Bindings\": {");
            sb.AppendLine("    \"MinimapToggle\": { \"Keyboard\": \"M\", \"Gamepad\": \"BUTTON_SELECT\" },");
            sb.AppendLine("    \"MinimapZoomIn\": { \"Keyboard\": \"OemPlus\", \"Gamepad\": \"R1\" },");
            sb.AppendLine("    \"MinimapZoomOut\": { \"Keyboard\": \"OemMinus\", \"Gamepad\": \"L1\" },");
            sb.AppendLine("    \"MapOpen\": { \"Keyboard\": \"Tab\", \"Gamepad\": \"BUTTON_START\" }");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            try
            {
                File.WriteAllText(filePath, sb.ToString());
                Log.Info("controls: created default SaltMapControls.json");
                string modDir = Path.GetDirectoryName(filePath);
                Load(modDir);
            }
            catch (Exception e) { Log.ErrorOnce("controls create default", e); }
        }

        public static string GetKeyName(string action)
        {
            if (config.Bindings.TryGetValue(action, out var binding)) return binding.Keyboard;
            return "";
        }

        public static string GetPadName(string action)
        {
            if (config.Bindings.TryGetValue(action, out var binding)) return binding.Gamepad;
            return "";
        }

        public static bool IsKeyboardPressed(string action, KeyboardState current, KeyboardState previous)
        {
            string keyStr = GetKeyName(action);
            if (string.IsNullOrEmpty(keyStr)) return false;
            try
            {
                Keys key = (Keys)Enum.Parse(typeof(Keys), keyStr, true);
                return current.IsKeyDown(key) && !previous.IsKeyDown(key);
            }
            catch { return false; }
        }

        public static bool IsGamepadPressed(string action, GamePadState current, GamePadState previous)
        {
            string btnStr = GetPadName(action);
            if (string.IsNullOrEmpty(btnStr)) return false;

            // Маппинг строго по дампу из InputMapping
            switch (btnStr.ToUpperInvariant())
            {
                case "BUTTON_A": return current.Buttons.A == ButtonState.Pressed && previous.Buttons.A != ButtonState.Pressed;
                case "BUTTON_B": return current.Buttons.B == ButtonState.Pressed && previous.Buttons.B != ButtonState.Pressed;
                case "BUTTON_X": return current.Buttons.X == ButtonState.Pressed && previous.Buttons.X != ButtonState.Pressed;
                case "BUTTON_Y": return current.Buttons.Y == ButtonState.Pressed && previous.Buttons.Y != ButtonState.Pressed;
                case "DPAD_UP": return current.DPad.Up == ButtonState.Pressed && previous.DPad.Up != ButtonState.Pressed;
                case "DPAD_DOWN": return current.DPad.Down == ButtonState.Pressed && previous.DPad.Down != ButtonState.Pressed;
                case "DPAD_LEFT": return current.DPad.Left == ButtonState.Pressed && previous.DPad.Left != ButtonState.Pressed;
                case "DPAD_RIGHT": return current.DPad.Right == ButtonState.Pressed && previous.DPad.Right != ButtonState.Pressed;
                case "BUTTON_START": return current.Buttons.Start == ButtonState.Pressed && previous.Buttons.Start != ButtonState.Pressed;
                case "BUTTON_SELECT": return current.Buttons.Back == ButtonState.Pressed && previous.Buttons.Back != ButtonState.Pressed;
                case "L1": return current.Buttons.LeftShoulder == ButtonState.Pressed && previous.Buttons.LeftShoulder != ButtonState.Pressed;
                case "R1": return current.Buttons.RightShoulder == ButtonState.Pressed && previous.Buttons.RightShoulder != ButtonState.Pressed;
                case "L2": return current.Triggers.Left > 0.5f && previous.Triggers.Left <= 0.5f;
                case "R2": return current.Triggers.Right > 0.5f && previous.Triggers.Right <= 0.5f;
                case "BUTTON_L3": return current.Buttons.LeftStick == ButtonState.Pressed && previous.Buttons.LeftStick != ButtonState.Pressed;
                case "BUTTON_R3": return current.Buttons.RightStick == ButtonState.Pressed && previous.Buttons.RightStick != ButtonState.Pressed;
                default: return false;
            }
        }
    }
}