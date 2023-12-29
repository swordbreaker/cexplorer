using System.Text;

namespace console_explorer.ExtensionMethods
{
    public static class ConsoleKeyInfoExtensions
    {
        public static string ToDisplayString(this ConsoleKeyInfo keyInfo)
        {
            var sb = new StringBuilder();
            if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                sb.Append("Ctrl+");
            }
            if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
            {
                sb.Append("Alt+");
            }
            if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                sb.Append("Shift+");
            }
            sb.Append(keyInfo.Key.ToString());
            return sb.ToString();
        }
    }
}
