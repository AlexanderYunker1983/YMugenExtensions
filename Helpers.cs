namespace YMugenExtensions
{
    public static class Helpers
    {
        internal static bool HasFlagEx(this byte b, byte value)
        {
            return (b & value) == value;
        }

    }
}