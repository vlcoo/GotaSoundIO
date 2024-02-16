using System.Collections.Generic;
using System.IO;
using GotaSoundIO.Sound;

namespace GotaSoundIO.IO.RIFF;

/// <summary>
///     RIFF Writer.
/// </summary>
public class RiffWriter : FileWriter
{
    /// <summary>
    ///     Backup offsets.
    /// </summary>
    private Stack<long> BakOffs = new();

    /// <summary>
    ///     Block offset.
    /// </summary>
    private readonly Stack<long> BlockOffs = new();

    /// <summary>
    ///     Current offset.
    /// </summary>
    public long CurrOffset;

    //Constructors.

    #region Constructors

    public RiffWriter(Stream output) : base(output)
    {
    }

    #endregion

    /// <summary>
    ///     Init file.
    /// </summary>
    public void InitFile(string magic)
    {
        //Prepare file.
        BakOffs = new Stack<long>();
        Write("RIFF".ToCharArray());
        Write((uint)0);
        CurrOffset = CurrentOffset = FileOffset = BaseStream.Position;
        Write(magic.ToCharArray());
    }

    /// <summary>
    ///     Close the file.
    /// </summary>
    public new void CloseFile()
    {
        //Write the offset.
        WriteOffset(FileOffset);
    }

    /// <summary>
    ///     Start a chunk.
    /// </summary>
    /// <param name="blockName">The block name.</param>
    public void StartChunk(string blockName)
    {
        //Prepare block.
        BakOffs.Push(CurrOffset);
        Write(blockName.ToCharArray());
        Write((uint)0);
        CurrentOffset = CurrOffset = BaseStream.Position;
        BlockOffs.Push(CurrentOffset);
    }

    /// <summary>
    ///     Start a list chunk.
    /// </summary>
    /// <param name="blockName">The block name.</param>
    public void StartListChunk(string blockName)
    {
        //Prepare block.
        BakOffs.Push(CurrOffset);
        Write("LIST".ToCharArray());
        Write((uint)0);
        CurrentOffset = CurrOffset = BaseStream.Position;
        BlockOffs.Push(CurrentOffset);
        Write(blockName.ToCharArray());
    }

    /// <summary>
    ///     End a chunk.
    /// </summary>
    public void EndChunk()
    {
        //Write the offset.
        WriteOffset(BlockOffs.Pop());
        CurrOffset = CurrentOffset = BakOffs.Pop();
    }

    /// <summary>
    ///     Write an offset.
    /// </summary>
    /// <param name="basePos">The base position.</param>
    public void WriteOffset(long basePos)
    {
        var bak = Position;
        Position = basePos;
        var size = (uint)(bak - basePos);
        Position -= 4;
        Write(size);
        Position = bak;
    }

    /// <summary>
    ///     Write a wave file.
    /// </summary>
    /// <param name="r">The riff wave.</param>
    public void WriteWave(RiffWave r)
    {
        //Make it so the sample block is not written.
        r.Loops = false;

        //Write the file and fix chunks.
        var bak = BaseStream.Position;
        Write(r.Write());
        var bak2 = BaseStream.Position;
        BaseStream.Position = bak;
        Write("LIST".ToCharArray());
        BaseStream.Position += 4;
        Write("wave".ToCharArray());
        BaseStream.Position = bak2;
    }
}