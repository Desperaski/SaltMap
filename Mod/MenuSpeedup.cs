using System;
using HarmonyLib;
using ProjectTower.menu;

namespace SaltMap
{
    [HarmonyPatch(typeof(MenuMgr), "Update")]
    internal static class MenuSpeedup
    {
        // Безопасный доступ к приватным/защищенным полям через Harmony
        static readonly AccessTools.FieldRef<MenuMgr, int> transModeRef = AccessTools.FieldRefAccess<MenuMgr, int>("transMode");
        static readonly AccessTools.FieldRef<MenuMgr, float> transFrameRef = AccessTools.FieldRefAccess<MenuMgr, float>("transFrame");

        static void Prefix(MenuMgr __instance)
        {
            try
            {
                int mode = transModeRef(__instance);

                // Если меню находится в процессе какого-либо перехода (фейда)
                if (mode != 0)
                {
                    // Принудительно ставим таймер в значение больше 1 (например, 2f).
                    // В оригинальном коде игра проверяет if (transFrame > 1f) и мгновенно
                    // завершает переход, выставляя transMode = 0.
                    transFrameRef(__instance) = 2f;
                }
            }
            catch (Exception e)
            {
                Log.ErrorOnce("MenuSpeedup", e);
            }
        }
    }
}