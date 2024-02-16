using System.Collections.Generic;
using System.Linq;

namespace GotaSoundIO.IO.RIFF;

/// <summary>
///     List chunk.
/// </summary>
public class ListChunk : Chunk
{
    /// <summary>
    ///     Chunks.
    /// </summary>
    public List<Chunk> Chunks = new();

    /// <summary>
    ///     Get a chunk.
    /// </summary>
    /// <param name="magic">The magic.</param>
    /// <returns>The chunk.</returns>
    public Chunk GetChunk(string magic)
    {
        return Chunks.Where(x => x.Magic.Equals(magic)).FirstOrDefault();
    }
}