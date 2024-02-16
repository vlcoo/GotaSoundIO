using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GotaSoundIO.IO.RIFF;

/// <summary>
///     Riff Reader.
/// </summary>
public class RiffReader : FileReader
{
    /// <summary>
    ///     Chunks.
    /// </summary>
    public List<Chunk> Chunks = new();

    //Constructors.

    #region Constructors

    public RiffReader(Stream input) : base(input)
    {
        ReadData();
    }

    #endregion

    /// <summary>
    ///     Magic.
    /// </summary>
    public string Magic { get; private set; }

    /// <summary>
    ///     Read data.
    /// </summary>
    private void ReadData()
    {
        //RIFF.
        if (!new string(ReadChars(4)).Equals("RIFF"))
        {
            //throw new Exception("Not a valid RIFF file!");
        }

        //Size and magic.
        ReadUInt32();
        Magic = new string(ReadChars(4));

        //Read chunks.
        while (BaseStream.Position < BaseStream.Length) Chunks.Add(ReadChunk());
    }

    /// <summary>
    ///     Read a chunk.
    /// </summary>
    private Chunk ReadChunk()
    {
        //Get chunk.
        var magic = new string(ReadChars(4));
        var size = ReadUInt32();
        var bak = BaseStream.Position;
        if (size == 0) size = (uint)(Length - bak);

        //List chunk.
        if (magic.Equals("LIST"))
        {
            var l = new ListChunk();
            l.Magic = new string(ReadChars(4));
            l.Pos = BaseStream.Position;
            l.Size = size - 4;
            while (BaseStream.Position < bak + size) l.Chunks.Add(ReadChunk());
            return l;
        }

        var c = new Chunk { Magic = magic, Pos = BaseStream.Position, Size = size };
        ReadBytes((int)size);
        return c;
    }

    /// <summary>
    ///     Get a chunk.
    /// </summary>
    /// <param name="magic">The magic.</param>
    /// <returns>The chunk.</returns>
    public Chunk GetChunk(string magic)
    {
        return Chunks.Where(x => x.Magic.Equals(magic)).FirstOrDefault();
    }

    /// <summary>
    ///     Open a chunk.
    /// </summary>
    /// <param name="c">The chunk to open.</param>
    public void OpenChunk(Chunk c)
    {
        BaseStream.Position = c.Pos;
    }
}