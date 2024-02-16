namespace GotaSoundIO.IO;

/// <summary>
///     A writeable item.
/// </summary>
public interface IWriteable
{
    /// <summary>
    ///     Write the item.
    /// </summary>
    /// <param name="w">The file writer.</param>
    void Write(FileWriter w);
}