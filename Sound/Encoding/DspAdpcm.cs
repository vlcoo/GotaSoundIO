using System;
using System.Collections.Generic;
using System.Linq;
using GotaSoundIO.IO;
using GotaSoundIO.Sound.Encoding;
using VGAudio.Codecs.GcAdpcm;
using VGAudio.Utilities;

namespace GotaSoundIO.Sound;

/// <summary>
///     DSP-ADPCM encoding.
/// </summary>
public class DspAdpcm : IAudioEncoding
{
    /// <summary>
    ///     Encoding context.
    /// </summary>
    public DspAdpcmContext Context;

    /// <summary>
    ///     Data.
    /// </summary>
    private byte[] Data;

    /// <summary>
    ///     Number of samples contained.
    /// </summary>
    /// <returns>Number of samples.</returns>
    public int SampleCount()
    {
        return DspAdpcmMath.ByteCountToSampleCount(Data.Length);
    }

    /// <summary>
    ///     Data size contained.
    /// </summary>
    /// <returns>Data size.</returns>
    public int DataSize()
    {
        return Data.Length;
    }

    /// <summary>
    ///     Get the number of samples from a block size.
    /// </summary>
    /// <param name="blockSize">Block size to get the number of samples from.</param>
    /// <returns>Number of samples.</returns>
    public int SamplesFromBlockSize(int blockSize)
    {
        return DspAdpcmMath.ByteCountToSampleCount(blockSize);
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
        Data = r.ReadBytes((int)dataSize);
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
        //Convert data.
        var s = pcm.Select(x => ConvertFloat(x)).ToArray();

        //Get context.
        DspAdpcmContext context = null;
        if (encodingData != null) context = encodingData as DspAdpcmContext;
        if (context == null)
        {
            context = new DspAdpcmContext();
            context.LoadCoeffs(GcAdpcmCoefficients.CalculateCoefficients(s));
        }

        //Encode data.
        Data = DspAdpcmEncoder.EncodeSamples(s, context, loopStart);

        //Set pointers.
        encodingData = Context = context;
    }

    /// <summary>
    ///     Convert the data to floating point PCM.
    /// </summary>
    /// <param name="decodingData">Decoding data.</param>
    /// <returns>Floating point PCM data.</returns>
    public float[] ToFloatPCM(object decodingData = null)
    {
        //Get context.
        DspAdpcmContext context = null;
        if (decodingData != null) context = decodingData as DspAdpcmContext;
        if (context == null) context = Context;

        //Decode data.
        var pcm = new short[SampleCount()];
        DspAdpcmDecoder.Decode(Data, ref pcm, ref Context, (uint)pcm.Length);
        var ret = pcm.Select(x => (float)x / short.MaxValue).ToArray();

        //Set pointers.
        decodingData = Context = context;

        //Return data.
        return ret;
    }

    /// <summary>
    ///     Trim audio data.
    /// </summary>
    /// <param name="totalSamples">Total number of samples to have in the end.</param>
    public void Trim(int totalSamples)
    {
        Data = Data.SubArray(0, DspAdpcmMath.SampleCountToByteCount(totalSamples));
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
        var samples = new List<short>();
        foreach (var b in blocks) samples.AddRange((short[])b.RawData());
        var s = samples.ToArray();

        //Block size is -1.
        if (newBlockSize == -1)
        {
            //newData.Add(new PCM16() { Data = s });
        }

        //Other.
        else
        {
            var samplesPerBlock = newBlockSize / 2;
            var currSample = 0;
            while (currSample < samples.Count)
            {
                var numToCopy = Math.Min(samples.Count - currSample, samplesPerBlock);
                //newData.Add(new PCM16() { Data = s.SubArray(currSample, numToCopy) });
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
        if (propertyName.ToLower().Equals("context")) return (T)(object)Context;
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
        if (propertyName.ToLower().Equals("context")) Context = (DspAdpcmContext)(object)value;
    }

    /// <summary>
    ///     Duplicate the audio data.
    /// </summary>
    /// <returns>A copy of the audio data.</returns>
    public IAudioEncoding Duplicate()
    {
        var ret = new DspAdpcm { Data = new byte[Data.Length] };
        Array.Copy(Data, ret.Data, Data.Length);
        ret.Context = new DspAdpcmContext
        {
            coefs = Context.coefs, gain = Context.gain, loop_pred_scale = Context.loop_pred_scale,
            loop_yn1 = Context.loop_yn1, loop_yn2 = Context.loop_yn2, pred_scale = Context.pred_scale,
            yn1 = Context.yn1, yn2 = Context.yn2
        };
        return ret;
    }

    /// <summary>
    ///     Convert a float to a short sample.
    /// </summary>
    /// <param name="sample">Sample to convert.</param>
    /// <returns>Converted sample.</returns>
    private short ConvertFloat(float sample)
    {
        return (short)(sample * short.MaxValue);
    }

    /// <summary>
    ///     Get the context.
    /// </summary>
    /// <param name="blocks">Blocks to get the context from.</param>
    /// <param name="loopStart">Loop start.</param>
    /// <returns></returns>
    public static DspAdpcmContext GetContext(List<IAudioEncoding> blocks, int loopStart = -1)
    {
        //New context.
        var ret = new DspAdpcmContext();

        //Test.
        if (blocks.Count == 0) return ret;

        //Get data.
        ret.coefs = (blocks[0] as DspAdpcm).Context.coefs;
        ret.yn1 = (blocks[0] as DspAdpcm).Context.yn1;
        ret.yn2 = (blocks[0] as DspAdpcm).Context.yn2;
        ret.pred_scale = (blocks[0] as DspAdpcm).Context.pred_scale;
        ret.gain = (blocks[0] as DspAdpcm).Context.gain;

        //See if data is stored in its proper block.
        if (loopStart != -1)
        {
            var samplesPerBlock = blocks[0].SampleCount();
            var blockNum = loopStart / samplesPerBlock;
            ret.loop_yn1 = (blocks[blockNum] as DspAdpcm).Context.loop_yn1;
            ret.loop_yn2 = (blocks[blockNum] as DspAdpcm).Context.loop_yn2;
            ret.loop_pred_scale = (blocks[blockNum] as DspAdpcm).Context.loop_pred_scale;
        }

        //Default.
        if (ret.loop_yn1 == 0 && ret.loop_yn2 == 0)
        {
            ret.loop_yn1 = (blocks[0] as DspAdpcm).Context.loop_yn1;
            ret.loop_yn2 = (blocks[0] as DspAdpcm).Context.loop_yn2;
            ret.loop_pred_scale = (blocks[0] as DspAdpcm).Context.loop_pred_scale;
        }

        //Return the context.
        return ret;
    }
}

/// <summary>
///     DSP-ADPCM math.
/// </summary>
public static class DspAdpcmMath
{
    public static readonly int BytesPerFrame = 8;
    public static readonly int SamplesPerFrame = 14;
    public static readonly int NibblesPerFrame = 16;

    public static int NibbleCountToSampleCount(int nibbleCount)
    {
        var frames = nibbleCount / NibblesPerFrame;
        var extraNibbles = nibbleCount % NibblesPerFrame;
        var extraSamples = extraNibbles < 2 ? 0 : extraNibbles - 2;

        return SamplesPerFrame * frames + extraSamples;
    }

    public static int SampleCountToNibbleCount(int sampleCount)
    {
        var frames = sampleCount / SamplesPerFrame;
        var extraSamples = sampleCount % SamplesPerFrame;
        var extraNibbles = extraSamples == 0 ? 0 : extraSamples + 2;

        return NibblesPerFrame * frames + extraNibbles;
    }

    public static int NibbleToSample(int nibble)
    {
        var frames = nibble / NibblesPerFrame;
        var extraNibbles = nibble % NibblesPerFrame;
        var samples = SamplesPerFrame * frames;

        return samples + extraNibbles - 2;
    }

    public static int SampleToNibble(int sample)
    {
        var frames = sample / SamplesPerFrame;
        var extraSamples = sample % SamplesPerFrame;

        return NibblesPerFrame * frames + extraSamples + 2;
    }

    public static int SampleCountToByteCount(int sampleCount)
    {
        return SampleCountToNibbleCount(sampleCount).DivideBy2RoundUp();
    }

    public static int ByteCountToSampleCount(int byteCount)
    {
        return NibbleCountToSampleCount(byteCount * 2);
    }
}

/// <summary>
///     DSP-ADPCM Decoder.
/// </summary>
public static class DspAdpcmDecoder
{
    private static readonly sbyte[] NibbleToSbyte = { 0, 1, 2, 3, 4, 5, 6, 7, -8, -7, -6, -5, -4, -3, -2, -1 };

    private static uint DivideByRoundUp(uint dividend, uint divisor)
    {
        return (dividend + divisor - 1) / divisor;
    }

    private static sbyte GetHighNibble(byte value)
    {
        return NibbleToSbyte[(value >> 4) & 0xF];
    }

    private static sbyte GetLowNibble(byte value)
    {
        return NibbleToSbyte[value & 0xF];
    }

    private static short Clamp16(int value)
    {
        if (value > 32767) return 32767;
        if (value < -32678) return -32678;
        return (short)value;
    }


    /// <summary>
    ///     Decode DSP-ADPCM data.
    /// </summary>
    /// <param name="src">DSP-ADPCM source.</param>
    /// <param name="dst">Destination array of samples.</param>
    /// <param name="cxt">DSP-APCM context.</param>
    /// <param name="samples">Number of samples.</param>
    public static void Decode(byte[] src, ref short[] dst, ref DspAdpcmContext cxt, uint samples)
    {
        //Each DSP-ADPCM frame is 8 bytes long. It contains 1 header byte, and 7 sample bytes.

        //Set initial values.
        var hist1 = cxt.yn1;
        var hist2 = cxt.yn2;
        var dstIndex = 0;
        var srcIndex = 0;

        //Until all samples decoded.
        while (dstIndex < samples)
        {
            //Get the header.
            var header = src[srcIndex++];

            //Get scale and co-efficient index.
            var scale = (ushort)(1 << (header & 0xF));
            var coef_index = (byte)(header >> 4);
            var coef1 = cxt.coefs[coef_index][0];
            var coef2 = cxt.coefs[coef_index][1];

            //7 sample bytes per frame.
            for (uint b = 0; b < 7; b++)
            {
                //Get byte.
                var byt = src[srcIndex++];

                //2 samples per byte.
                for (uint s = 0; s < 2; s++)
                {
                    var adpcm_nibble = s == 0 ? GetHighNibble(byt) : GetLowNibble(byt);
                    var sample = Clamp16((((adpcm_nibble * scale) << 11) + 1024 + coef1 * hist1 + coef2 * hist2) >> 11);
                    hist2 = hist1;
                    hist1 = sample;
                    dst[dstIndex++] = sample;

                    if (dstIndex >= samples) break;
                }

                if (dstIndex >= samples) break;
            }
        }

        //Set context.
        cxt.yn1 = hist1;
        cxt.yn2 = hist2;
    }
}

/// <summary>
///     The encoder.
/// </summary>
public class DspAdpcmEncoder
{
    /// <summary>
    ///     Encodes the samples.
    /// </summary>
    /// <returns>The samples.</returns>
    /// <param name="samples">Samples.</param>
    public static byte[] EncodeSamples(short[] samples, DspAdpcmContext info, int loopStart)
    {
        //Encode data.
        var dspAdpcm = GcAdpcmEncoder.Encode(samples, info.GetCoeffs(),
            new GcAdpcmParameters { History1 = info.yn1, History2 = info.yn2, SampleCount = samples.Length });

        //Loop stuff.
        if (loopStart > 0) info.loop_yn1 = samples[loopStart - 1];
        if (loopStart > 1) info.loop_yn2 = samples[loopStart - 2];

        //Return data.
        return dspAdpcm;
    }
}

/// <summary>
///     DspAdpcm context
/// </summary>
public class DspAdpcmContext : IReadable, IWriteable
{
    /// <summary>
    ///     [8][2] array of coefficients.
    /// </summary>
    public short[][] coefs;

    /// <summary>
    ///     Gain.
    /// </summary>
    public ushort gain;

    /// <summary>
    ///     Loop predictor scale.
    /// </summary>
    public ushort loop_pred_scale;

    /// <summary>
    ///     Loop history 1.
    /// </summary>
    public short loop_yn1;

    /// <summary>
    ///     Loop history 2.
    /// </summary>
    public short loop_yn2;

    /// <summary>
    ///     Predictor scale.
    /// </summary>
    public ushort pred_scale;

    /// <summary>
    ///     History 1.
    /// </summary>
    public short yn1;

    /// <summary>
    ///     History 2.
    /// </summary>
    public short yn2;

    /// <summary>
    ///     Read the info.
    /// </summary>
    /// <param name="r">The reader.</param>
    public void Read(FileReader r)
    {
        LoadCoeffs(r.ReadInt16s(16));
        gain = r.ReadUInt16();
        pred_scale = r.ReadUInt16();
        yn1 = r.ReadInt16();
        yn2 = r.ReadInt16();
        loop_pred_scale = r.ReadUInt16();
        loop_yn1 = r.ReadInt16();
        loop_yn2 = r.ReadInt16();
    }

    /// <summary>
    ///     Write the info.
    /// </summary>
    /// <param name="w">The writer.</param>
    public void Write(FileWriter w)
    {
        w.Write(GetCoeffs());
        w.Write(gain);
        w.Write(pred_scale);
        w.Write(yn1);
        w.Write(yn2);
        w.Write(loop_pred_scale);
        w.Write(loop_yn1);
        w.Write(loop_yn2);
    }

    /// <summary>
    ///     Get the coeffecients.
    /// </summary>
    /// <returns>The coefficients.</returns>
    public short[] GetCoeffs()
    {
        var c = new List<short>();
        foreach (var a in coefs) c.AddRange(a);
        return c.ToArray();
    }

    /// <summary>
    ///     Load the coefficients.
    /// </summary>
    /// <param name="c">The coefficients.</param>
    public void LoadCoeffs(short[] c)
    {
        coefs = new short[8][];
        coefs[0] = new[] { c[0], c[1] };
        coefs[1] = new[] { c[2], c[3] };
        coefs[2] = new[] { c[4], c[5] };
        coefs[3] = new[] { c[6], c[7] };
        coefs[4] = new[] { c[8], c[9] };
        coefs[5] = new[] { c[10], c[11] };
        coefs[6] = new[] { c[12], c[13] };
        coefs[7] = new[] { c[14], c[15] };
    }
}