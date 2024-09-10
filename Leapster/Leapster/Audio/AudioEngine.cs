using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Reflection;
using System.Xml.Linq;

namespace Leapster.Audio;

public class AudioEngine
{
    public static AudioEngine Instance { get; private set; } = new();

    private MixingSampleProvider mixer;
    private IWavePlayer outputDevice;

    public AudioEngine()
    {
        if (Instance != null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        Instance = this;

        MMDevice device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        WaveFormat mixFormat = device.AudioClient.MixFormat;
        mixer = new(WaveFormat.CreateIeeeFloatWaveFormat(mixFormat.SampleRate, mixFormat.Channels))
        {
            ReadFully = true
        };

        WasapiOut output = new WasapiOut(device, AudioClientShareMode.Shared, true, 100);

        output.Init(mixer);
        output.Play();

        outputDevice = output;
    }

    private Dictionary<string, CachedSampleProvider> sampleCache = [];

    public void PlayResourceSound(string name)
    {
        name = typeof(Program).Namespace + ".Assets.Sounds." + name;

        if (!sampleCache.TryGetValue(name, out CachedSampleProvider cached))
        {
            Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            ISampleProvider sample = new WaveToSampleProvider(new MediaFoundationResampler(new WaveFileReader(resource), mixer.WaveFormat));
            cached = new CachedSampleProvider(sample);
            sampleCache.Add(name, cached);
        }

        AddMixerInput(cached);
    }

    public void PlaySound(string fileName)
    {
        if (!sampleCache.TryGetValue(fileName, out CachedSampleProvider cached))
        {
            ISampleProvider sample = new AudioFileReader(fileName);
            cached = new CachedSampleProvider(sample);
            sampleCache.Add(fileName, cached);
        }

        AddMixerInput(cached);
    }

    private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
    {
        if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
        {
            return input;
        }

        if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
        {
            return new MonoToStereoSampleProvider(input);
        }

        throw new NotImplementedException("Not yet implemented this channel count conversion");
    }

    private void AddMixerInput(ISampleProvider input)
    {
        WdlResamplingSampleProvider resampled = new(input, mixer.WaveFormat.SampleRate);

        mixer.AddMixerInput(ConvertToRightChannelCount(resampled));
    }

}
