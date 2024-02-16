using System;
using NAudio.Wave;

namespace GotaSoundIO.Sound.Playback;

/// <summary>
///     Null wave provider.
/// </summary>
public class NullWavePlayer : IWavePlayer
{
    private PlaybackState m_PlaybackState;

    /// <summary>
    ///     Volume.
    /// </summary>
    public float Volume { get; set; } = 1f;

    /// <summary>
    ///     Playback state.
    /// </summary>
    public PlaybackState PlaybackState => throw new NotImplementedException();

    /// <summary>
    ///     Playback stopped.
    /// </summary>
    public event EventHandler<StoppedEventArgs> PlaybackStopped;

    /// <summary>
    ///     Play.
    /// </summary>
    public void Play()
    {
        m_PlaybackState = PlaybackState.Playing;
    }

    /// <summary>
    ///     Stop.
    /// </summary>
    public void Stop()
    {
        m_PlaybackState = PlaybackState.Stopped;
    }

    /// <summary>
    ///     Pause.
    /// </summary>
    public void Pause()
    {
        if (m_PlaybackState == PlaybackState.Paused)
            m_PlaybackState = PlaybackState.Playing;
        else
            m_PlaybackState = PlaybackState.Paused;
    }

    /// <summary>
    ///     Init.
    /// </summary>
    public void Init(IWaveProvider waveProvider)
    {
    }

    /// <summary>
    ///     There's nothing to dispose.
    /// </summary>
    public void Dispose()
    {
    }
}