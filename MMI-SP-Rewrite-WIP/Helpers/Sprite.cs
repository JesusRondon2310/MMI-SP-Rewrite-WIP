using System.Drawing;
using GTA;

namespace MMI_SP.Helpers
{
    internal static class Sprite
    {
        // ==========================================
        // BLOQUE: Funciones
        // ==========================================
        public static void InsuranceStatus(string fileName, float x, float y, Color color)
        {
            Image sprite = Image.FromFile(fileName);
            GTA.UI.CustomSprite a = new GTA.UI.CustomSprite(fileName, new Size(sprite.Width, sprite.Height), new PointF(x, y), color, 0.0f);
            a.Draw();
        }
    }
}