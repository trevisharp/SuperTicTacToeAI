using System;
using System.Threading.Tasks;

public class Game
{
    public State State { get; set; } = State.Empty;
    public Player XPlayer { get; set; }
    public Player YPlayer { get; set; }

    public async Task Play()
    {
        var play = await XPlayer.Play(Piece.X);
        if (!play.HasValue)
            throw new Exception("Player cannot make a move");
        var realplay = play.Value;
        State.Play(realplay.Item1, realplay.Item2, realplay.Item3, realplay.Item4, Piece.X);
    }
}