public class State
{
    internal ushort[] data;
    internal State() { }

    public Piece this[int collumn, int row, int i, int j]
        => (Piece)data[3 * row + collumn].GetPiece(i, j);

    public void Play(int collumn, int row, int i, int j, Piece piece)
    {
        ushort move = piece == Piece.Default ?
            (ushort)(data[9] % 4) : (ushort)piece;
        var section = data[3 * row + collumn];
        data[3 * row + collumn] = section.SetPiece(i, j, move);
    }

    public static State Empty
    {
        get
        {
            State state = StatePool.Current.RentState();
            return state;
        }
    }
}