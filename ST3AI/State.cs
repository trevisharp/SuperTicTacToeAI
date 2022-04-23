public class State
{
    internal ushort[] data;
    internal State() { }

    public void Play(int collumn, int row, int i, int j, Piece piece)
    {
        ushort move = piece == Piece.Default ?
            (ushort)(data[0] % 4) : (ushort)piece;
        var section = data[3 * row + collumn];

    }
}