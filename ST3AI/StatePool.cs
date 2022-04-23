using System;
using System.Buffers;

public class StatePool
{
    #region Singleton Implementation

    public static StatePool Current => current;
    private static readonly StatePool current = null;
    static StatePool() => current = new StatePool();

    #endregion

    private ArrayPool<ushort> pool;
    private StatePool()
    {
        this.pool = ArrayPool<ushort>.Create(10 * 1024 * 1024, 10);
    }

    public State RentState()
    {
        var bytes = this.pool.Rent(10);
        var state = new State();
        state.data = bytes;
        return state;
    }

    public State CopyAndRentState(State original)
    {
        var state = RentState();
        Buffer.BlockCopy(original.data, 0, state.data, 0, state.data.Length);
        return state;
    }
}