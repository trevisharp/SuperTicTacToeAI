using System.Collections.Generic;

public static class StateExtension
{
    public static IEnumerable<State> Next(this State main)
    {
        yield break;
    }
}