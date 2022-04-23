public static class StateExtension
{
    private static ushort[] powbuffer = { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683 };
    
    public static int GetPiece(this ushort section, int i, int j)
    {
        int index = 3 * j + i;
        return (section / powbuffer[index]) % powbuffer[index + 1];
    }

    public static ushort SetPiece(this ushort section, int i, int j, ushort value)
        => (ushort)(section + value * powbuffer[3 * j + i]);
}