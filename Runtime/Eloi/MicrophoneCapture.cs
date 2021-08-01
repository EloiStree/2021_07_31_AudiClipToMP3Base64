using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class MicrophoneCapture : MonoBehaviour
{
    //A handle to the attached AudioSource  
    [SerializeField] AudioSource m_linkedAudioSourceToReplay;
    // Boolean flags shows if the microphone is connected 
    private bool m_hasMicrophone = false;

    //The maximum and minimum available recording frequencies  
    private int m_minFrequence;
    private int m_maxFrequence;
    [SerializeField] RecordingState m_recordState;
    public enum RecordingState:int
    {
        NotWorking_NoMicrophone,
        Recording,
        ReadyToBeUsed
    }


    public bool HasMicrophone(bool recheck=true)
    {
        if (recheck)
            CheckForMicrophone();
        return m_hasMicrophone;
    }

   

    public RecordingState GetState()
    {
        return m_recordState;
    }


    public bool HasRecordedSomething()
    {
      return m_linkedAudioSourceToReplay.clip != null;
    }

    public UnityEvent m_uiChanged;
    public UnityEvent m_startRecording;
    public UnityEvent m_stopRecording;

    void Awake()
    {
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
        CheckForMicrophone();
    }

    public AudioClip GetClip()
    {
        return m_linkedAudioSourceToReplay.clip;
    }

    public void CheckForMicrophone() {

        RecordingState previous = m_recordState;
        if (Microphone.devices.Length <= 0)
        {
            m_recordState = RecordingState.NotWorking_NoMicrophone;
            NotifyStateChange();
        }
        else if (m_recordState== RecordingState.NotWorking_NoMicrophone )
        {
            m_hasMicrophone = true;
            Microphone.GetDeviceCaps(null, out m_minFrequence, out m_maxFrequence);
            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (m_minFrequence == 0 && m_maxFrequence == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate  
                m_maxFrequence = 44100;
            }
            m_recordState = RecordingState.ReadyToBeUsed;

        }
        if (m_recordState != previous)
            NotifyStateChange();
    }

    public void GetMicrophoneName(out string microphoneName)
    {
        if (Microphone.devices.Length == 0)
            microphoneName = "";
        else {
            microphoneName = string.Join(" | ",Microphone.devices);
        }
    }

    public bool IsMicrophoneBeingUsed() { return Microphone.IsRecording(null); }

    [ContextMenu("Start Recording")]
    public void StartRecording() {

        CheckForMicrophone();
        if (!m_hasMicrophone)
            return;
        //Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource  
        m_recordState = RecordingState.Recording;
        m_linkedAudioSourceToReplay.clip = Microphone.Start(null, true, 20, m_maxFrequence);
        NotifyStateChange();
        m_startRecording.Invoke();
    }
    [ContextMenu("Stop Recording")]
    public void StopRecording() {


        Microphone.End(null); //Stop the audio recording  
        m_recordState = RecordingState.ReadyToBeUsed;
        NotifyStateChange();
        m_stopRecording.Invoke();

    }


    public void PlayBackCurrentRecord() {

        if(m_linkedAudioSourceToReplay.clip!=null)
            m_linkedAudioSourceToReplay.Play();  
    }

    public void NotifyStateChange() {
        m_uiChanged.Invoke();
    }

    private void Reset()
    {
        m_linkedAudioSourceToReplay = GetComponent<AudioSource>();
    }
}