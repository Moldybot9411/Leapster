using NAudio.Wave;

namespace Leapster.Audio;

public class CachedSampleProvider : ISampleProvider
{
    public WaveFormat WaveFormat { get; set; }

    private readonly float[] cachedSample;
    private int position;

    public CachedSampleProvider(ISampleProvider source)
    {
        WaveFormat = source.WaveFormat;
        cachedSample = new float[source.WaveFormat.SampleRate * source.WaveFormat.Channels * 10]; // 10 seconds of audio
        int samplesRead = source.Read(cachedSample, 0, cachedSample.Length);
        if (samplesRead < cachedSample.Length)
        {
            Array.Resize(ref cachedSample, samplesRead);
        }
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = Math.Min(count, cachedSample.Length - position);
        Array.Copy(cachedSample, position, buffer, offset, samplesRead);
        position += samplesRead;
        if (position >= cachedSample.Length)
        {
            position = 0; // Loop back to the start
        }
        return samplesRead;
    }

    public void Reset()
    {
        position = 0;
    }
}
