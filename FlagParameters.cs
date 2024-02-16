using GotaSoundIO.IO;

namespace GotaSoundIO;

/// <summary>
///     Has optional parameters that are enabled by bit flags.
/// </summary>
public class FlagParameters : IReadable, IWriteable
{
    /// <summary>
    ///     Parameters.
    /// </summary>
    private readonly uint?[] Parameters = new uint?[32];

    /// <summary>
    ///     Get parameter.
    /// </summary>
    /// <param name="bit">Bit index.</param>
    /// <returns>Parameter at bit index.</returns>
    public uint? this[int bit]
    {
        get => Parameters[bit];
        set => Parameters[bit] = value;
    }

    /// <summary>
    ///     Read the item.
    /// </summary>
    /// <param name="r">The reader.</param>
    public void Read(FileReader r)
    {
        var mask = r.ReadUInt32();
        for (var i = 0; i < 32; i++)
            if ((mask & (0b1 << i)) > 0)
                Parameters[i] = r.ReadUInt32();
            else
                Parameters[i] = null;
    }

    /// <summary>
    ///     Write the item.
    /// </summary>
    /// <param name="w">The writer.</param>
    public void Write(FileWriter w)
    {
        uint mask = 0;
        for (var i = 0; i < 32; i++)
            if (Parameters[i] != null)
                mask |= (uint)(0b1 << i);
        w.Write(mask);
        foreach (var p in Parameters)
            if (p != null)
                w.Write(p.Value);
    }
}