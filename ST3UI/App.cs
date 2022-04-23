public class App
{
    public App()
    {
        PlayerX = new HumanPlayer();
        PlayerO = new HumanPlayer();
    }
    public Player PlayerX { get; set; }
    public Player PlayerO { get; set; }
}