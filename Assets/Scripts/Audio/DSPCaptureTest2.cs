using System;
using UnityEngine;
using System.Runtime.InteropServices;

class DSPCaptureTest2 : MonoBehaviour
{
    FMOD.DSP fft;

    LineRenderer lineRenderer;

    const int WindowSize = 1024;

    [SerializeField]
    private MixerController.MIXER_BUS targetBusID;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = WindowSize;
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;

        FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fft);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, WindowSize * 2);

        var targetBus = CameraReceiverListener.GetBus();
        targetBus.lockChannelGroup();
        FMODUnity.RuntimeManager.StudioSystem.flushCommands();

        FMOD.ChannelGroup channelGroup;
        var result1 = targetBus.getChannelGroup(out channelGroup);
        var result2 = channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft);
        bool test = true;
    }

    const float WIDTH = 10.0f;
    const float HEIGHT = 0.1f;

    void Update()
    {
        IntPtr unmanagedData;
        uint length;
        fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        var spectrum = fftData.spectrum;

        if (fftData.numchannels > 0)
        {
            var pos = Vector3.zero;
            pos.x = WIDTH * -0.5f;

            for (int i = 0; i < WindowSize; ++i)
            {
                pos.x += (WIDTH / WindowSize);

                float level = lin2dB(spectrum[0][i]);
                pos.y = (80 + level) * HEIGHT;

                lineRenderer.SetPosition(i, pos);
            }
        }
    }

    float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }

    public FMOD.Studio.Bus GetTargetBus()
    {
        return MixerController.GetBus(targetBusID);
    }
}