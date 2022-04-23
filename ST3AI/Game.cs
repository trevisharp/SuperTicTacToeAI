using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public class Game
{
    public State State { get; set; } = State.Empty;
    public Player XPlayer { get; set; }
    public Player OPlayer { get; set; }
    public Player CurrentPlayer { get; private set; }

    public Piece CurrentPiece 
        => CurrentPlayer == XPlayer ? Piece.X : Piece.O;

    public async Task<Piece> Play()
    {
        CurrentPlayer = XPlayer;
        Piece winner = Piece.None;
        (int, int, int, int)? play = null;
        (int, int, int, int) nonnullplay = (0, 0, 0, 0);
        while (true)
        {
            play = await XPlayer.Play(Piece.X);
            if (!play.HasValue)
                throw new Exception("Player cannot make a move");
            nonnullplay = play.Value;
            State.Play(
                nonnullplay.Item1, 
                nonnullplay.Item2, 
                nonnullplay.Item3, 
                nonnullplay.Item4, 
                Piece.X);

            CurrentPlayer = OPlayer;
            winner = VerifyWinner();
            if (winner != Piece.None)
                break;
            
            play = await OPlayer.Play(Piece.O);
            if (!play.HasValue)
                throw new Exception("Player cannot make a move");
            nonnullplay = play.Value;
            State.Play(
                nonnullplay.Item1, 
                nonnullplay.Item2, 
                nonnullplay.Item3, 
                nonnullplay.Item4, 
                Piece.O);

            CurrentPlayer = XPlayer;
            winner = VerifyWinner();
            if (winner != Piece.None)
                break;
        }
        return winner;
    }

    public Piece VerifyWinner()
    {
        return Piece.None;
    }
}