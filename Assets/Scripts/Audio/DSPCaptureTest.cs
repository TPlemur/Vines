using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.InteropServices;

//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to capture
// the DSP with FMOD.DSP_READ_CALLBACK and how to create and add a DSP
// to master channel group ready for capturing.
//  
// For the description of the channel counts. See
// https://fmod.com/docs/2.02/api/core-api-common.html#fmod_speakermode
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
//--------------------------------------------------------------------


class DSPCaptureTest : MonoBehaviour
{
    private FMOD.DSP_READ_CALLBACK mReadCallback;
    private FMOD.DSP mCaptureDSP;
    private float[] mDataBuffer;
    private GCHandle mObjHandle;
    private uint mBufferLength;
    private int mChannels = 0;

    [AOT.MonoPInvokeCallback(typeof(FMOD.DSP_READ_CALLBACK))]
    static FMOD.RESULT CaptureDSPReadCallback(ref FMOD.DSP_STATE dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels)
    {
        FMOD.DSP_STATE_FUNCTIONS functions = (FMOD.DSP_STATE_FUNCTIONS)Marshal.PtrToStructure(dsp_state.functions, typeof(FMOD.DSP_STATE_FUNCTIONS));

        IntPtr userData;
        functions.getuserdata(ref dsp_state, out userData);

        GCHandle objHandle = GCHandle.FromIntPtr(userData);
        DSPCaptureTest obj = objHandle.Target as DSPCaptureTest;

        // Save the channel count out for the update function
        obj.mChannels = inchannels;

        // Copy the incoming buffer to process later
        int lengthElements = (int)length * inchannels;
        Marshal.Copy(inbuffer, obj.mDataBuffer, 0, lengthElements);

        // Copy the inbuffer to the outbuffer so we can still hear it
        Marshal.Copy(obj.mDataBuffer, 0, outbuffer, lengthElements);

        return FMOD.RESULT.OK;
    }

    void Start()
    {
        // Assign the callback to a member variable to avoid garbage collection
        mReadCallback = CaptureDSPReadCallback;

        // Allocate a data buffer large enough for 8 channels, pin the memory to avoid garbage collection
        uint bufferLength;
        int numBuffers;
        FMODUnity.RuntimeManager.CoreSystem.getDSPBufferSize(out bufferLength, out numBuffers);
        mDataBuffer = new float[bufferLength * 8];
        mBufferLength = bufferLength;

        // Get a handle to this object to pass into the callback
        mObjHandle = GCHandle.Alloc(this);
        if (mObjHandle != null)
        {
            // Define a basic DSP that receives a callback each mix to capture audio
            FMOD.DSP_DESCRIPTION desc = new FMOD.DSP_DESCRIPTION();
            desc.numinputbuffers = 1;
            desc.numoutputbuffers = 1;
            desc.read = mReadCallback;
            desc.userdata = GCHandle.ToIntPtr(mObjHandle);

            // Create an instance of the capture DSP and attach it to the master channel group to capture all audio
            FMOD.ChannelGroup channelGroup;

            var targetBus = CameraReceiverListener.GetBus();
            targetBus.lockChannelGroup();
            FMODUnity.RuntimeManager.StudioSystem.flushCommands();

            // FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterCG)
            if (targetBus.getChannelGroup(out channelGroup) == FMOD.RESULT.OK)
            {
                if (FMODUnity.RuntimeManager.CoreSystem.createDSP(ref desc, out mCaptureDSP) == FMOD.RESULT.OK)
                {
                    if (channelGroup.addDSP(0, mCaptureDSP) != FMOD.RESULT.OK)
                    {
                        Debug.LogWarningFormat("FMOD: Unable to add mCaptureDSP to the master channel group");
                    }
                }
                else
                {
                    Debug.LogWarningFormat("FMOD: Unable to create a DSP: mCaptureDSP");
                }
            }
            else
            {
                Debug.LogWarningFormat("FMOD: Unable to create a master channel group: masterCG");
            }
        }
        else
        {
            Debug.LogWarningFormat("FMOD: Unable to create a GCHandle: mObjHandle");
        }
    }

    void OnDestroy()
    {
        if (mObjHandle != null)
        {
            var targetBus = CameraReceiverListener.GetBus();

            // Remove the capture DSP from the master channel group
            FMOD.ChannelGroup channelGroup;
            if (targetBus.getChannelGroup(out channelGroup) == FMOD.RESULT.OK)
            {
                if (mCaptureDSP.hasHandle())
                {
                    channelGroup.removeDSP(mCaptureDSP);

                    // Release the DSP and free the object handle
                    mCaptureDSP.release();
                }
            }
            mObjHandle.Free();

            targetBus.unlockChannelGroup(); // might cause problems if there is more than one of these...
        }
    }

    const float WIDTH = 10.0f;
    const float HEIGHT = 1.0f;

    void Update()
    {
        Camera cam = Camera.main;

        transform.position = cam.transform.position + (cam.transform.forward * 4.0f);
        transform.rotation = cam.transform.rotation;

        // Do what you want with the captured data

        if (mChannels != 0)
        {
            float yOffset = 5.7f;

            for (int j = 0; j < mChannels; j++)
            {
                var pos = Vector3.zero;
                pos.x = WIDTH * -0.5f;
                for (int i = 0; i < mBufferLength; ++i)
                {
                    pos.x += (WIDTH / mBufferLength);
                    pos.y = mDataBuffer[i + j * mBufferLength] * HEIGHT;
                    // Make sure Gizmos is enabled in the Unity Editor to show debug line draw for the captured channel data
                    Debug.DrawLine(transform.position + new Vector3(pos.x, yOffset + pos.y, 0), transform.position + new Vector3(pos.x, yOffset - pos.y, 0), Color.green);
                }
                yOffset -= 1.9f;
            }
        }
    }
}
