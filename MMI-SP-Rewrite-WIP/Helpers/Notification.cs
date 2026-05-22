namespace MMI_SP.Helpers
{
    internal static class Notification
    {
        public static void Show(string title, string message, string description)
        {
            string text = string.IsNullOrEmpty(description)
                ? $"{title}: {message}"
                : $"{title}: {message} {description}";

            GTA.UI.Notification.PostTicker(text, isImportant: false);
        }
    }
}