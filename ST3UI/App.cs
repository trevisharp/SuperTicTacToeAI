public class App
{
    public App()
    {
        PlayerX = new HumanPlayer();
        PlayerO = new AIPlayer()
        {
            Engine = new RandomBadEngine()
        };
    }
    public Player PlayerX { get; set; }
    public Player PlayerO { get; set; }
}