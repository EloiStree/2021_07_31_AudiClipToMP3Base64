using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MicrophoneRecordingManager : MonoBehaviour
{
    public MicrophoneCapture m_microphoneState;
    public MicrophoneRecordingManager m_recorderState;


    public Text m_buttonLabel;
    public Text m_recordingMicName;
    public Image m_recordingLed;
    public InputField m_base64WithHeader;

    public Color m_recordingColor= Color.green;
    public Color m_notRecordingColor = Color.red;
    public Color m_hasNoMicrophoneColor = Color.cyan;
    public void UpdateUIFromData()
    {

        MicrophoneCapture.RecordingState micState = m_microphoneState.GetState();
        if (micState == MicrophoneCapture.RecordingState.NotWorking_NoMicrophone) {
            m_recordingLed.color = m_hasNoMicrophoneColor;
            m_buttonLabel.text = "Connect";
        }
        else if (micState == MicrophoneCapture.RecordingState.Recording)
        {
            m_recordingLed.color = m_recordingColor;
            m_buttonLabel.text = "Recording...";
        }
        else if (micState == MicrophoneCapture.RecordingState.ReadyToBeUsed)
        {
            m_recordingLed.color = m_notRecordingColor;
            m_buttonLabel.text = "Start Recording";

        }
        else
        {
            m_buttonLabel.text = "?";
            m_recordingLed.color = Color.white;
        }
        m_recorderState.GetBase64WithHeader(out string b64WithHeader);
        m_base64WithHeader.text = b64WithHeader;

        if (m_recordingMicName != null) { 
            m_microphoneState.GetMicrophoneName(out string micName);
            m_recordingMicName.text = micName;
        }


    }
}
