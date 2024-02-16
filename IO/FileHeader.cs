namespace GotaSoundIO.IO;

/// <summary>
///     File header.
/// </summary>
public abstract class FileHeader : IReadable, IWriteable
{
    /// <summary>
    ///     Block offsets.
    /// </summary>
    public long[] BlockOffsets;

    /// <summary>
    ///     Block sizes.
    /// </summary>
    public long[] BlockSizes;

    /// <summary>
    ///     Block types.
    /// </summary>
    public long[] BlockTypes;

    /// <summary>
    ///     Byte order.
    /// </summary>
    public ByteOrder ByteOrder;

    /// <summary>
    ///     File size.
    /// </summary>
    public long FileSize;

    /// <summary>
    ///     Header size.
    /// </summary>
    public long HeaderSize;

    /// <summary>
    ///     Magic.
    /// </summary>
    public string Magic;

    /// <summary>
    ///     Version.
    /// </summary>
    public Version Version;

    /// <summary>
    ///     Read a header.
    /// </summary>
    /// <param name="r">The reader.</param>
    public abstract void Read(FileReader r);

    /// <summary>
    ///     Write a header.
    /// </summary>
    /// <param name="w">The writer.</param>
    public abstract void Write(FileWriter w);
}