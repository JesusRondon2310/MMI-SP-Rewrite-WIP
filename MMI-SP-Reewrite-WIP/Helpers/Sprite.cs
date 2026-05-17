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
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
            GTA.UI.CustomSprite sprite = new GTA.UI.CustomSprite(
                fileName, new Size(img.Width, img.Height), new PointF(x, y), color, 0.0f);
            sprite.Draw();
        }
    }
}
