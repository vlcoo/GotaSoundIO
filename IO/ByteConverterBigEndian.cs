﻿using System.Security;

namespace GotaSoundIO.IO;

/// <summary>
///     Represents a <see cref="T:Syroot.BinaryData.ByteConverter" /> which handles big endianness.
/// </summary>
[SecuritySafeCritical]
public sealed class ByteConverterBigEndian : ByteConverter
{
    /// <summary>
    ///     Gets the <see cref="T:Syroot.BinaryData.ByteOrder" /> in which data is stored as converted by this instance.
    /// </summary>
    public override ByteOrder ByteOrder => ByteOrder.BigEndian;

    /// <summary>
    ///     Stores the specified <see cref="T:System.Double" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    [SecuritySafeCritical]
    public override unsafe void GetBytes(double value, byte[] buffer, int startIndex = 0)
    {
        var num = (ulong)*(long*)&value;
        buffer[startIndex] = (byte)(num >> 56);
        buffer[startIndex + 1] = (byte)(num >> 48);
        buffer[startIndex + 2] = (byte)(num >> 40);
        buffer[startIndex + 3] = (byte)(num >> 32);
        buffer[startIndex + 4] = (byte)(num >> 24);
        buffer[startIndex + 5] = (byte)(num >> 16);
        buffer[startIndex + 6] = (byte)(num >> 8);
        buffer[startIndex + 7] = (byte)num;
    }

    /// <summary>
    ///     Stores the specified <see cref="T:System.Int16" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    public override void GetBytes(short value, byte[] buffer, int startIndex = 0)
    {
        buffer[startIndex] = (byte)((uint)value >> 8);
        buffer[startIndex + 1] = (byte)value;
    }

    /// <summary>
    ///     Stores the specified <see cref="T:System.Int32" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    public override void GetBytes(int value, byte[] buffer, int startIndex = 0)
    {
        buffer[startIndex] = (byte)(value >> 24);
        buffer[startIndex + 1] = (byte)(value >> 16);
        buffer[startIndex + 2] = (byte)(value >> 8);
        buffer[startIndex + 3] = (byte)value;
    }

    /// <summary>
    ///     Stores the specified <see cref="T:System.Int64" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    public override void GetBytes(long value, byte[] buffer, int startIndex = 0)
    {
        buffer[startIndex] = (byte)(value >> 56);
        buffer[startIndex + 1] = (byte)(value >> 48);
        buffer[startIndex + 2] = (byte)(value >> 40);
        buffer[startIndex + 3] = (byte)(value >> 32);
        buffer[startIndex + 4] = (byte)(value >> 24);
        buffer[startIndex + 5] = (byte)(value >> 16);
        buffer[startIndex + 6] = (byte)(value >> 8);
        buffer[startIndex + 7] = (byte)value;
    }

    /// <summary>
    ///     Stores the specified <see cref="T:System.Single" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    [SecuritySafeCritical]
    public override unsafe void GetBytes(float value, byte[] buffer, int startIndex = 0)
    {
        var num = *(uint*)&value;
        buffer[startIndex] = (byte)(num >> 24);
        buffer[startIndex + 1] = (byte)(num >> 16);
        buffer[startIndex + 2] = (byte)(num >> 8);
        buffer[startIndex + 3] = (byte)num;
    }

    /// <summary>
    ///     Stores the specified <see cref="T:System.UInt16" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    public override void GetBytes(ushort value, byte[] buffer, int startIndex = 0)
    {
        buffer[startIndex] = (byte)((uint)value >> 8);
        buffer[startIndex + 1] = (byte)value;
    }

    /// <summary>
    ///     Stores the specified <see cref="T:System.UInt32" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    public override void GetBytes(uint value, byte[] buffer, int startIndex = 0)
    {
        buffer[startIndex] = (byte)(value >> 24);
        buffer[startIndex + 1] = (byte)(value >> 16);
        buffer[startIndex + 2] = (byte)(value >> 8);
        buffer[startIndex + 3] = (byte)value;
    }

    /// <summary>
    ///     Stores the specified <see cref="T:System.UInt64" /> value as bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="buffer">The byte array to store the value in.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    public override void GetBytes(ulong value, byte[] buffer, int startIndex = 0)
    {
        buffer[startIndex] = (byte)(value >> 56);
        buffer[startIndex + 1] = (byte)(value >> 48);
        buffer[startIndex + 2] = (byte)(value >> 40);
        buffer[startIndex + 3] = (byte)(value >> 32);
        buffer[startIndex + 4] = (byte)(value >> 24);
        buffer[startIndex + 5] = (byte)(value >> 16);
        buffer[startIndex + 6] = (byte)(value >> 8);
        buffer[startIndex + 7] = (byte)value;
    }

    /// <summary>
    ///     Returns an <see cref="T:System.Double" /> instance converted from the bytes in the given <paramref name="buffer" />
    ///     .
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    [SecuritySafeCritical]
    public override unsafe double ToDouble(byte[] buffer, int startIndex = 0)
    {
        var i = ((long)buffer[startIndex] << 56) | ((long)buffer[startIndex + 1] << 48) |
                ((long)buffer[startIndex + 2] << 40) | ((long)buffer[startIndex + 3] << 32) |
                ((long)buffer[startIndex + 4] << 24) | ((long)buffer[startIndex + 5] << 16) |
                ((long)buffer[startIndex + 6] << 8) | buffer[startIndex + 7];
        return *(double*)&i;
    }

    /// <summary>
    ///     Returns an <see cref="T:System.Int16" /> instance converted from the bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    public override short ToInt16(byte[] buffer, int startIndex = 0)
    {
        return (short)((buffer[startIndex] << 8) | buffer[startIndex + 1]);
    }

    /// <summary>
    ///     Returns an <see cref="T:System.Int32" /> instance converted from the bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    public override int ToInt32(byte[] buffer, int startIndex = 0)
    {
        return (buffer[startIndex] << 24) | (buffer[startIndex + 1] << 16) | (buffer[startIndex + 2] << 8) |
               buffer[startIndex + 3];
    }

    /// <summary>
    ///     Returns an <see cref="T:System.Int64" /> instance converted from the bytes in the given <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    public override long ToInt64(byte[] buffer, int startIndex = 0)
    {
        return ((long)buffer[startIndex] << 56) | ((long)buffer[startIndex + 1] << 48) |
               ((long)buffer[startIndex + 2] << 40) | ((long)buffer[startIndex + 3] << 32) |
               ((long)buffer[startIndex + 4] << 24) | ((long)buffer[startIndex + 5] << 16) |
               ((long)buffer[startIndex + 6] << 8) | buffer[startIndex + 7];
    }

    /// <summary>
    ///     Returns an <see cref="T:System.Single" /> instance converted from the bytes in the given <paramref name="buffer" />
    ///     .
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    [SecuritySafeCritical]
    public override unsafe float ToSingle(byte[] buffer, int startIndex = 0)
    {
        var i = (buffer[startIndex] << 24) | (buffer[startIndex + 1] << 16) | (buffer[startIndex + 2] << 8) |
                buffer[startIndex + 3];
        return *(float*)&i;
    }

    /// <summary>
    ///     Returns an <see cref="T:System.UInt16" /> instance converted from the bytes in the given <paramref name="buffer" />
    ///     .
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    public override ushort ToUInt16(byte[] buffer, int startIndex = 0)
    {
        return (ushort)(((uint)buffer[startIndex] << 8) | buffer[startIndex + 1]);
    }

    /// <summary>
    ///     Returns an <see cref="T:System.UInt32" /> instance converted from the bytes in the given <paramref name="buffer" />
    ///     .
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    public override uint ToUInt32(byte[] buffer, int startIndex = 0)
    {
        return (uint)((buffer[startIndex] << 24) | (buffer[startIndex + 1] << 16) | (buffer[startIndex + 2] << 8)) |
               buffer[startIndex + 3];
    }

    /// <summary>
    ///     Returns an <see cref="T:System.UInt64" /> instance converted from the bytes in the given <paramref name="buffer" />
    ///     .
    /// </summary>
    /// <param name="buffer">The byte array storing the raw data.</param>
    /// <param name="startIndex">The index at which to start writing values into the buffer.</param>
    /// <returns>The converted value.</returns>
    public override ulong ToUInt64(byte[] buffer, int startIndex = 0)
    {
        return (ulong)(((long)buffer[startIndex] << 56) | ((long)buffer[startIndex + 1] << 48) |
                       ((long)buffer[startIndex + 2] << 40) | ((long)buffer[startIndex + 3] << 32) |
                       ((long)buffer[startIndex + 4] << 24) | ((long)buffer[startIndex + 5] << 16) |
                       ((long)buffer[startIndex + 6] << 8)) | buffer[startIndex + 7];
    }
}