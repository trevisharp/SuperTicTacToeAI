public class State
{
    internal byte[] data;
    internal State() { }

    public Piece this[int collumn, int row, int i, int j]
        => (Piece)(data[27 * row + 9 * collumn + 3 * j + i] % 4);

    public void Play(int column, int row, int i, int j, Piece piece)
    {
        var index = 27 * row + 9 * column;
        var valcoef = (byte)(4 * (byte)(10 - piece));
        data[index + 3 * j + i] += (byte)piece;
        data[index + i] += valcoef;
        data[index + j + 3] += valcoef;
        if (i == j)
            data[index + 6] += valcoef;
        if (i == 2 - j)
            data[index + 7] += valcoef;
        // TODO: add a major validation
    }

    public Piece VerifyMinorWinner(int column, int row)
    {
        var index = 27 * row + 9 * column;
        for (int i = 0; i < 9; i++)
        {
            var value = data[index + i] / 4;
            if (value == 24)
                return Piece.O;
            else if (value == 27)
                return Piece.X;
        }
        return Piece.None;
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