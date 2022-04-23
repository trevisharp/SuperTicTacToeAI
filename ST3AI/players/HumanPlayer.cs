using System.Threading;
using System.Threading.Tasks;

public class HumanPlayer : Player
{
    private int[] lastplay = null;
    SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public override async Task<(int, int, int, int)?> Play(State state, Piece piece)
    {
        bool running = true;
        while (running)
        {
            await Task.Delay(50);
            await semaphore.WaitAsync();
            running = lastplay == null;
            semaphore.Release();
        }
        var play = (lastplay[0], lastplay[1], lastplay[2], lastplay[3]);
        lastplay = null;
        return play;
    }

    public async Task RegisterPlay(int collumn, int row, int i, int j)
    {
        await semaphore.WaitAsync();
        lastplay = new int[] { collumn, row, i, j };
        semaphore.Release();
    }
}