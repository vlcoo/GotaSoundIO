﻿using System;
using System.Collections.Generic;
using System.Linq;
using GotaSoundIO.IO;
using GotaSoundIO.Sound.Encoding;

namespace GotaSoundIO.Sound;

/// <summary>
///     Signed 32-bit PCM audio.
/// </summary>
public class PCM32Signed : IAudioEncoding
{
    /// <summary>
    ///     Data.
    /// </summary>
    private int[] Data;

    /// <summary>
    ///     Number of samples contained.
    /// </summary>
    /// <returns>Number of samples.</returns>
    public int SampleCount()
    {
        return Data.Length;
    }

    /// <summary>
    ///     Data size contained.
    /// </summary>
    /// <returns>Data size.</returns>
    public int DataSize()
    {
        return SampleCount() * 4;
    }

    /// <summary>
    ///     Get the number of samples from a block size.
    /// </summary>
    /// <param name="blockSize">Block size to get the number of samples from.</param>
    /// <returns>Number of samples.</returns>
    public int SamplesFromBlockSize(int blockSize)
    {
        return blockSize / 4;
    }

    /// <summary>
    ///     Raw data.
    /// </summary>
    /// <returns>Raw data.</returns>
    public object RawData()
    {
        return Data;
    }

    /// <summary>
    ///     Read the raw data.
    /// </summary>
    /// <param name="r">File reader.</param>
    /// <param name="numSamples">Number of samples.</param>
    /// <param name="dataSize">Data size.</param>
    public void ReadRaw(FileReader r, uint numSamples, uint dataSize)
    {
        Data = r.ReadInt32s((int)(dataSize / 4));
    }

    /// <summary>
    ///     Write the raw data.
    /// </summary>
    /// <param name="w">File writer.</param>
    public void WriteRaw(FileWriter w)
    {
        w.Write(Data);
    }

    /// <summary>
    ///     Convert from floating point PCM to the data.
    /// </summary>
    /// <param name="pcm">PCM data.</param>
    /// <param name="encodingData">Encoding data.</param>
    /// <param name="loopStart">Loop start.</param>
    /// <param name="loopEnd">Loop end.</param>
    public void FromFloatPCM(float[] pcm, object encodingData = null, int loopStart = -1, int loopEnd = -1)
    {
        Data = pcm.Select(x => (int)(x * int.MaxValue)).ToArray();
    }

    /// <summary>
    ///     Convert the data to floating point PCM.
    /// </summary>
    /// <param name="decodingData">Decoding data.</param>
    /// <returns>Floating point PCM data.</returns>
    public float[] ToFloatPCM(object decodingData = null)
    {
        return Data.Select(x => (float)x / int.MaxValue).ToArray();
    }

    /// <summary>
    ///     Trim audio data.
    /// </summary>
    /// <param name="totalSamples">Total number of samples to have in the end.</param>
    public void Trim(int totalSamples)
    {
        Data = Data.SubArray(0, totalSamples);
    }

    /// <summary>
    ///     Change block size.
    /// </summary>
    /// <param name="blocks">Audio blocks.</param>
    /// <param name="newBlockSize">New block size.</param>
    /// <returns>New blocks.</returns>
    public List<IAudioEncoding> ChangeBlockSize(List<IAudioEncoding> blocks, int newBlockSize)
    {
        //New blocks.
        var newData = new List<IAudioEncoding>();

        //Get all samples.
        var samples = new List<int>();
        foreach (var b in blocks) samples.AddRange((int[])b.RawData());
        var s = samples.ToArray();

        //Block size is -1.
        if (newBlockSize == -1)
        {
            newData.Add(new PCM32Signed { Data = s });
        }

        //Other.
        else
        {
            var samplesPerBlock = newBlockSize / 4;
            var currSample = 0;
            while (currSample < samples.Count)
            {
                var numToCopy = Math.Min(samples.Count - currSample, samplesPerBlock);
                newData.Add(new PCM32Signed { Data = s.SubArray(currSample, numToCopy) });
                currSample += numToCopy;
            }
        }

        //Return data.
        return newData;
    }

    /// <summary>
    ///     Get a property.
    /// </summary>
    /// <typeparam name="T">Property type.</typeparam>
    /// <param name="propertyName">Property name.</param>
    /// <returns>Retrieved property.</returns>
    public T GetProperty<T>(string propertyName)
    {
        return default;
    }

    /// <summary>
    ///     Set a property.
    /// </summary>
    /// <typeparam name="T">Property type to set.</typeparam>
    /// <param name="value">Value to set.</param>
    /// <param name="propertyName">Name of the property to set.</param>
    public void SetProperty<T>(T value, string propertyName)
    {
    }

    /// <summary>
    ///     Duplicate the audio data.
    /// </summary>
    /// <returns>A copy of the audio data.</returns>
    public IAudioEncoding Duplicate()
    {
        var ret = new PCM32Signed { Data = new int[Data.Length] };
        Array.Copy(Data, ret.Data, Data.Length);
        return ret;
    }
}