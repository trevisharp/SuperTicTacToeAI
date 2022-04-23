using System.Threading.Tasks;

public abstract class Player
{
    public abstract Task<(int, int, int, int)?> Play(Piece piece);
}