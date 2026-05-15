using System;
using GTA;
using MMI_SP.Agency.MainMenu;
using MMI_SP.PatternMatching;

namespace MMI_SP.Agency.Office.Entry
{
    internal static class CancelSequence
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static Result<bool> Execute(UI menu, CutsceneManager cutscene, bool showMenu)
        {
            cutscene.Exit();
            Game.Player.Character.Position = Agency.Reception.Position;
            Game.Player.Character.IsPositionFrozen = false;
            GTA.UI.Screen.FadeIn(1000);
            ScriptCameraDirector.StopRendering();

            if (showMenu)
            {
                menu.RebuildMenu();
                menu.Show();
            }

            return new Ok<bool>(true);
        }
    }
}