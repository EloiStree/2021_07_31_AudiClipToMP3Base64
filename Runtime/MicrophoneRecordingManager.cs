using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MicrophoneRecordingManager : MonoBehaviour
{
    public MicrophoneCapture m_basicMicroRecorder;
    public AudioSource m_replaySource;
    public AudioClip m_recordedAsClip;
    private byte[] m_recordedAsBytes;
    public string m_recordedAsBase64;
    public string m_recordedAsBase64WithHeader;
    public UnityEvent m_audioConvertedChange;
    public int m_mpegbitRate = 128;


    public void CopyToClipboard() {
        GUIUtility.systemCopyBuffer = m_recordedAsBase64WithHeader;
    }

    public void RequestAudioReplay() {
        if (m_replaySource != null) {
            m_replaySource.clip = m_recordedAsClip;
            m_replaySource.Play();
        }
    }

    public void TryToStartRecording()
    {
        if (m_basicMicroRecorder.GetState() == MicrophoneCapture.RecordingState.ReadyToBeUsed) { 
            m_basicMicroRecorder.StartRecording();
        }
    }

    public void GetBase64WithHeader(out string text)
    {
      text= m_recordedAsBase64WithHeader;
    }

    public void TryToStopRecording()
    {
        if (m_basicMicroRecorder.GetState() == MicrophoneCapture.RecordingState.Recording) {
       
            m_basicMicroRecorder.StopRecording();
            ConvertLastRecordAsBase64();
        }
    }

    public void DoWhatBestNext()
    {
        if (m_basicMicroRecorder.GetState() == MicrophoneCapture.RecordingState.NotWorking_NoMicrophone)
        { 
            m_basicMicroRecorder.CheckForMicrophone();
        }

        else if (m_basicMicroRecorder.GetState() == MicrophoneCapture.RecordingState.Recording)
        {
            TryToStopRecording();
        }
        else if (m_basicMicroRecorder.GetState() == MicrophoneCapture.RecordingState.ReadyToBeUsed) { 
         
            TryToStartRecording();
        }
    }

    public void ConvertLastRecordAsBase64() {
        if (m_basicMicroRecorder.HasRecordedSomething())
        {
            m_recordedAsClip = m_basicMicroRecorder.GetClip();
            AudioClipToBase64Utility.ConvertAudioToBase64(m_recordedAsClip,
                out m_recordedAsBase64,
                out m_recordedAsBase64WithHeader, m_mpegbitRate);
            m_audioConvertedChange.Invoke();

        }
        else {
            m_recordedAsClip = null;
            m_recordedAsBase64 = "";
            m_recordedAsBase64WithHeader = "";
            m_audioConvertedChange.Invoke();
        }
    }
}
