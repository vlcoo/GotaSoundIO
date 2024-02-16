﻿using System;
using GotaSoundIO.IO;

namespace GotaSoundIO.Sound;

/// <summary>
///     DSP Audio.
/// </summary>
public class DSP : SoundFile
{
    /// <summary>
    ///     Block size if extended.
    /// </summary>
    public uint BlockSize = 0x2000;

    /// <summary>
    ///     Extended mode allows for more than 1 channel.
    /// </summary>
    public bool Extended;

    /// <summary>
    ///     Blank constructor.
    /// </summary>
    public DSP()
    {
    }

    /// <summary>
    ///     Create a new dsp file from a file.
    /// </summary>
    /// <param name="filePath">The file.</param>
    public DSP(string filePath) : base(filePath)
    {
    }

    /// <summary>
    ///     Get the supported encodings.
    /// </summary>
    /// <returns>The supported encodings.</returns>
    public override Type[] SupportedEncodings()
    {
        return new[] { typeof(DspAdpcm) };
    }

    /// <summary>
    ///     Name.
    /// </summary>
    /// <returns>The name.</returns>
    public override string Name()
    {
        return "DSP";
    }

    /// <summary>
    ///     Extensions.
    /// </summary>
    /// <returns>The extensions.</returns>
    public override string[] Extensions()
    {
        return new[] { "DSP", "MDSP" };
    }

    /// <summary>
    ///     Description.
    /// </summary>
    /// <returns>The description.</returns>
    public override string Description()
    {
        return "DSP-ADPCM mono file.";
    }

    /// <summary>
    ///     If the file supports tracks.
    /// </summary>
    /// <returns>No, it doesn't.</returns>
    public override bool SupportsTracks()
    {
        return false;
    }

    /// <summary>
    ///     The preferred encoding.
    /// </summary>
    /// <returns>Preferred encoding for the file.</returns>
    public override Type PreferredEncoding()
    {
        return typeof(DspAdpcm);
    }

    /// <summary>
    ///     Read the file.
    /// </summary>
    /// <param name="r">The reader.</param>
    public override void Read(FileReader r)
    {
        //Read data.
        r.ByteOrder = ByteOrder.BigEndian;
        var numSamples = r.ReadUInt32();
        r.ReadUInt32(); //Num nibbles.
        SampleRate = r.ReadUInt32();
        Loops = r.ReadUInt16() > 0;
        r.ReadUInt16(); //DSP-ADPCM is 0.
        LoopStart = r.ReadUInt32();
        LoopEnd = r.ReadUInt32();
        r.ReadUInt32(); //Always 2???
        var context = r.Read<DspAdpcmContext>();
        var numChannels = r.ReadUInt16();
        BlockSize = (uint)(r.ReadUInt16() * 8);
        Extended = numChannels > 0;

        //Add channel 0.
        //Channels = new List<AudioEncoding>();
        //Channels.Add(new DspAdpcmOLD() { Context = context });
        r.Align(0x60);

        //Extra channels.
        for (var i = 1; i < numChannels; i++)
        {
            r.ReadBytes(0x1C);
            //Channels.Add(new DspAdpcmOLD() { Context = r.Read<DspAdpcmContext>() });
            r.Align(0x60);
        }

        //Do block reading logic.
        if (numChannels == 0) numChannels = 1;
        var dataLen = r.Length - r.Position;
        var channelLen = dataLen / numChannels;
        if (!Extended) BlockSize = (uint)channelLen;
        var lastBlockSize = (uint)(channelLen % BlockSize);
        var blockCarry = false;
        if (lastBlockSize == 0)
            lastBlockSize = BlockSize;
        else
            blockCarry = true;
        var numBlocks = (uint)(channelLen / BlockSize + (blockCarry ? 1 : 0));

        //Read data.
        //Audio.Read(r, encodingType, numChannels, numBlocks, BlockSize, blockSamples, lastBlockSize, lastBlockSamples, 0);
    }

    /// <summary>
    ///     Write the file.
    /// </summary>
    /// <param name="w">The writer.</param>
    public override void Write(FileWriter w)
    {
        //Remove channels if not extended.
        if (!Extended)
            for (var i = Audio.Channels.Count - 1; i >= 1; i--)
                Audio.Channels.RemoveAt(i);

        //Write data.
        w.ByteOrder = ByteOrder.BigEndian;
        for (var i = 0; i < Audio.Channels.Count; i++)
        {
            w.Write(Audio.NumSamples);
            //w.Write(DspAdpcmMath.CalcNumNibbles((uint)Audio.NumSamples));
            w.Write(SampleRate);
            w.Write((ushort)(Loops ? 1 : 0));
            w.Write((ushort)0);
            w.Write(LoopStart);
            w.Write(LoopEnd);
            w.Write((uint)2);
            w.Write((Audio.Channels[i][0] as DspAdpcm).Context);
            if (Extended)
            {
                w.Write((ushort)Audio.Channels.Count);
                w.Write((ushort)(BlockSize / 8));
            }

            w.Align(0x60);
        }

        //Write data.
        Audio.ChangeBlockSize((int)BlockSize);
        Audio.Write(w);
    }
}