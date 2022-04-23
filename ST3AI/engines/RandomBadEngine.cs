using System;

public class RandomBadEngine : AIEngine
{
    public override (int, int, int, int)? Play(State state, Piece piece)
    {
        Random rnd = new Random();
        int seed = rnd.Next();
        int patience = 1000;
        while (patience-- > 0)
        {
            int col = seed % 3;
            int row = (seed / 2) % 3;
            int i = (seed / 4) % 3;
            int j = (seed / 8) % 3;
            if (state[col, row, i, j] == Piece.None)
                return (col, row, i, j);
            seed = rnd.Next();
        }
        return null;
    }
}