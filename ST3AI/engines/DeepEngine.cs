public class DeepEngine : AIEngine
{
    public MinMaxTree Tree { get; set; } = new MinMaxTree();
    public override (int, int, int, int)? Play(State state, Piece piece)
    {
        throw new System.NotImplementedException();
    }
}