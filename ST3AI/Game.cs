using System;
using System.Threading.Tasks;
using System.Linq;

public class Game
{
    public State State { get; set; } = State.Empty;
    public Player XPlayer { get; set; }
    public Player OPlayer { get; set; }
    public Player CurrentPlayer { get; private set; }

    public Piece CurrentPiece 
        => CurrentPlayer == XPlayer ? Piece.X : Piece.O;

    public (int, int) ActiveBoard { get; private set; } = (-1, -1);

    public async Task<Piece> Play()
    {
        CurrentPlayer = XPlayer;
        Piece winner = Piece.None;
        (int, int, int, int)? play = null;
        (int, int, int, int) nonnullplay = (0, 0, 0, 0);
        while (true)
        {
            play = await XPlayer.Play(State, Piece.X);
            if (!play.HasValue)
                throw new Exception("Player cannot make a move");
            nonnullplay = play.Value;
            State.Play(
                nonnullplay.Item1, 
                nonnullplay.Item2, 
                nonnullplay.Item3, 
                nonnullplay.Item4, 
                Piece.X);
            if (VerifyMinorWinner(nonnullplay.Item3, nonnullplay.Item4) != Piece.None)
                ActiveBoard = (-1, -1);
            else ActiveBoard = (nonnullplay.Item3, nonnullplay.Item4);

            CurrentPlayer = OPlayer;
            winner = VerifyMajorWinner();
            if (winner != Piece.None)
                break;
            
            play = await OPlayer.Play(State, Piece.O);
            if (!play.HasValue)
                throw new Exception("Player cannot make a move");
            nonnullplay = play.Value;
            State.Play(
                nonnullplay.Item1, 
                nonnullplay.Item2, 
                nonnullplay.Item3, 
                nonnullplay.Item4, 
                Piece.O);
            if (VerifyMinorWinner(nonnullplay.Item3, nonnullplay.Item4) != Piece.None)
                ActiveBoard = (-1, -1);
            else ActiveBoard = (nonnullplay.Item3, nonnullplay.Item4);

            CurrentPlayer = XPlayer;
            winner = VerifyMajorWinner();
            if (winner != Piece.None)
                break;
        }
        return winner;
    }

    public bool IsActive(int collum, int row)
        => ((ActiveBoard.Item1 == -1 && ActiveBoard.Item2 == -1)
        || (ActiveBoard.Item1 == collum && ActiveBoard.Item2 == row))
        && VerifyMinorWinner(collum, row) == Piece.None;
    public Piece VerifyMajorWinner()
    {
        int[] points = new int[8];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var minor = (int)VerifyMinorWinner(i, j);
                if (minor == 0)
                    continue;
                minor = 2 * minor - 3;
                points[i] += minor;
                points[j + 3] += minor;
                if (i == j)
                    points[6] += minor;
                if (i == 2 - j)
                    points[7] += minor;
            }
        }
        if (points.Contains(3))
            return Piece.O;
        else if (points.Contains(-3))
            return Piece.X;
        return Piece.None;
    }

    public Piece VerifyMinorWinner(int i, int j)
    {
        if (i < 0 || i > 2 || j < 0 || j > 2)
            return Piece.X;
        return State.VerifyMinorWinner(i, j);
    }
}