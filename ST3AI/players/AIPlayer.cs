using System.Threading.Tasks;

public class AIPlayer : Player
{
    public AIEngine Engine { get; set; }
    public override Task<(int, int, int, int)?> Play(State state, Piece piece)
    {
        return Task.Factory.StartNew(() => Engine.Play(state, piece));
    }
}