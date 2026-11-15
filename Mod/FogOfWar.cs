using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectTower;
using ProjectTower.config;

namespace SaltMap
{
    internal static class FogOfWar
    {
        // Уменьшенная сетка для производительности и мягких краев
        public const int GridSize = 128;
        public const float MapSize = 32768f;
        public const float CellSize = MapSize / GridSize;

        // Множитель для радиуса проверки текста (2.5 * 800 = 2000)
        const float TextRadiusMultiplier = 2.5f;

        static Color[] pixels = new Color[GridSize * GridSize];
        static Texture2D texture;
        static bool dirty = true;
        static bool enabled;

        static Vector2 lastPlayerMapPos = new Vector2(-1, -1);
        static readonly HashSet<string> revealedBosses = new HashSet<string>();

        static float revealRadius = 800f;
        static float bossRevealRadius = 3000f;
        static double saveTimer = 0;

        // Отслеживаем текущий слот сохранения
        static int lastSaveId = -2;

        // Публичное свойство, чтобы MenuMap мог узнать текущий радиус текста
        public static float TextRevealRadius => revealRadius * TextRadiusMultiplier;

        public static void Init(string modDir)
        {
            enabled = Settings.GetBool("fogOfWar", true);
            if (!enabled) return;

            if (float.TryParse(Settings.Get("fogRadius", "800"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float r))
                revealRadius = Math.Max(100f, r);

            if (float.TryParse(Settings.Get("fogBossRadius", "3000"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float br))
                bossRevealRadius = Math.Max(100f, br);

            // Загрузка файла теперь происходит в Update(), когда мы знаем номер слота
        }

        static void Reset()
        {
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.Black;
            dirty = true;
        }

        public static void Save(string modDir)
        {
            if (!enabled || !dirty) return;

            // Если мы в главном меню (нет активного сейва), не сохраняем
            if (lastSaveId < 0) return;

            byte[] data = new byte[GridSize * GridSize];
            for (int i = 0; i < data.Length; i++)
                data[i] = (pixels[i].A < 128) ? (byte)0 : (byte)1;

            string path = Path.Combine(modDir, $"fog_{lastSaveId}.dat");
            try { File.WriteAllBytes(path, data); dirty = false; }
            catch (Exception e) { Log.ErrorOnce("fog save", e); }
        }

        public static bool IsVisited(Vector2 mapPos)
        {
            if (!enabled) return true;
            int cx = (int)(mapPos.X / CellSize);
            int cy = (int)(mapPos.Y / CellSize);
            if (cx < 0 || cy < 0 || cx >= GridSize || cy >= GridSize) return false;
            return pixels[cy * GridSize + cx].A < 128;
        }

        public static bool IsAreaVisited(Vector2 center, float radius)
        {
            if (!enabled) return true;
            int rCells = (int)(radius / CellSize);
            int cx = (int)(center.X / CellSize);
            int cy = (int)(center.Y / CellSize);

            for (int y = -rCells; y <= rCells; y++)
            {
                for (int x = -rCells; x <= rCells; x++)
                {
                    if (x * x + y * y <= rCells * rCells)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && py >= 0 && px < GridSize && py < GridSize)
                        {
                            if (pixels[py * GridSize + px].A < 128) return true;
                        }
                    }
                }
            }
            return false;
        }

        public static void Update()
        {
            if (!enabled) return;

            int currentSave = ConfigMgr.lastSave;
            if (currentSave != lastSaveId)
            {
                Log.Info($"fog: save slot changed from {lastSaveId} to {currentSave}");

                if (lastSaveId >= 0) Save(Entry.ModDir);

                lastSaveId = currentSave;

                if (lastSaveId == -1)
                {
                    Reset();
                    Log.Info("fog: new character detected, resetting fog");
                }
                else if (lastSaveId >= 0)
                {
                    string path = Path.Combine(Entry.ModDir, $"fog_{lastSaveId}.dat");
                    if (File.Exists(path))
                    {
                        byte[] data = File.ReadAllBytes(path);
                        if (data.Length == GridSize * GridSize)
                        {
                            for (int i = 0; i < data.Length; i++)
                                pixels[i] = data[i] == 0 ? Color.Transparent : Color.Black;
                            dirty = true;
                            Log.Info($"fog: loaded fog for slot {lastSaveId}");
                        }
                        else { Reset(); }
                    }
                    else
                    {
                        Reset();
                        Log.Info($"fog: no fog file for slot {lastSaveId}, starting fresh");
                    }
                }
            }

            // ИСПРАВЛЕНО: убрали блокировку, если lastSaveId < 0
            if (!PlayerTracker.HasPosition) return;

            Vector2 mapPos = MapSpace.FromWorld(PlayerTracker.Position);
            if (Vector2.DistanceSquared(mapPos, lastPlayerMapPos) > 2500f)
            {
                lastPlayerMapPos = mapPos;
                Reveal(mapPos, revealRadius);
            }

            if (PlayerTracker.MainPlayer != null)
            {
                foreach (Marker m in Markers.All)
                {
                    if (m.Kind == Show.Bosses)
                    {
                        string flag = Progress.LinkedFlag(m);
                        if (flag != null && !revealedBosses.Contains(flag) && Progress.Has(flag))
                        {
                            revealedBosses.Add(flag);
                            Reveal(m.Map, bossRevealRadius);
                        }
                    }
                }
            }

            saveTimer += Game1.frameTime;
            if (saveTimer > 30.0)
            {
                saveTimer = 0;
                Save(Entry.ModDir);
            }
        }

        static void Reveal(Vector2 center, float radius)
        {
            int rCells = (int)(radius / CellSize);
            int cx = (int)(center.X / CellSize);
            int cy = (int)(center.Y / CellSize);

            bool changed = false;
            for (int y = -rCells; y <= rCells; y++)
            {
                for (int x = -rCells; x <= rCells; x++)
                {
                    float dist = (float)Math.Sqrt(x * x + y * y);
                    if (dist <= rCells)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && py >= 0 && px < GridSize && py < GridSize)
                        {
                            int idx = py * GridSize + px;

                            // Более плавный градиент (квадратичный)
                            float normalizedDist = dist / rCells;
                            float alphaFalloff = 1f - (normalizedDist * normalizedDist);

                            if (pixels[idx].A > 0)
                            {
                                int newA = (int)(pixels[idx].A - alphaFalloff * 255);
                                newA = Math.Max(0, newA);
                                pixels[idx] = new Color((byte)0, (byte)0, (byte)0, (byte)newA);
                                changed = true;
                            }
                        }
                    }
                }
            }
            if (changed) dirty = true;
        }

        public static void Draw(GraphicsDevice device, SpriteBatch batch, Rectangle rect, Vector2 focus, float scale)
        {
            if (!enabled) return;

            if (texture == null || texture.IsDisposed || texture.GraphicsDevice != device)
            {
                texture = new Texture2D(device, GridSize, GridSize, false, SurfaceFormat.Color);
                dirty = true;
            }
            if (dirty)
            {
                texture.SetData(pixels);
                dirty = false;
            }

            Vector2 topLeft = MapView.ToScreen(rect, focus, scale, Vector2.Zero);
            Vector2 bottomRight = MapView.ToScreen(rect, focus, scale, new Vector2(MapSize, MapSize));

            Rectangle dest = new Rectangle((int)topLeft.X, (int)topLeft.Y,
                (int)(bottomRight.X - topLeft.X), (int)(bottomRight.Y - topLeft.Y));

            batch.Draw(texture, dest, Color.White);
        }

        public static void EndFrame() { }
    }
}