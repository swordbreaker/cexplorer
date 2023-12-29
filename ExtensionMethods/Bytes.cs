namespace console_explorer.ExtensionMethods
{
    public record Bytes(long Value)
    {
        public static implicit operator Bytes(long value) => new(value);
        public static implicit operator long(Bytes bytes) => bytes.Value;

        public static Bytes operator +(Bytes a, Bytes b) => new(a.Value + b.Value);
        public static Bytes operator -(Bytes a, Bytes b) => new(a.Value - b.Value);
        public static Bytes operator *(Bytes a, Bytes b) => new(a.Value * b.Value);
        public static Bytes operator /(Bytes a, Bytes b) => new(a.Value / b.Value);
        public static Bytes operator %(Bytes a, Bytes b) => new(a.Value % b.Value);
        public static bool operator <(Bytes a, Bytes b) => a.Value < b.Value;
        public static bool operator >(Bytes a, Bytes b) => a.Value > b.Value;

        public string ToDisplayString()
        {
            float bytes = Value;
            var units = new[] { "B", "KB", "MB", "GB", "TB", "PB" };
            var unitIndex = 0;
            while (bytes >= 1024)
            {
                bytes /= 1024;
                unitIndex++;
            }
            return $"{bytes:f2} {units[unitIndex]} ({Value:N0} bytes)";
        }
    }
}
