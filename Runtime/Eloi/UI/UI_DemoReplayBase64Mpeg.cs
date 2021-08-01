using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UI_DemoReplayBase64Mpeg : MonoBehaviour
{
    public InputField m_base64Text;
    public InputField m_uriBase64;
    public AudioSource m_replayer;
    public Text m_debugText;
    [Header("Debug")]
    public AudioClip m_loaded;
    public string m_errorLoading;


    public void TryToPlay()
    {
        m_replayer.clip = m_loaded;
        m_replayer.Play();
    }

    private void AudioLoaded(AudioClipCallBack source)
    {
        m_errorLoading = source.GetError();
        m_loaded = source.GetClip();
        if (m_debugText != null)
            m_debugText.text = m_errorLoading;
    }

    public void LoadAudioClipFromURI()
    {
        string uri = m_uriBase64.text;
        AudioClipCallBack callback = new AudioClipCallBack();
        callback.AddListener(AudioLoaded);
        StartCoroutine(AudioClipToBase64Utility.LoadAudioBase64FromWebpageOrFile(m_uriBase64.text, callback));
    }
    public void LoadAudioClipFromInputField()
    {
        string text = m_base64Text.text;
        LoadFromText(text);


    }

    private void LoadFromText(string text)
    {
        AudioClipCallBack callback = new AudioClipCallBack();
        callback.AddListener(AudioLoaded);
        StartCoroutine(AudioClipToBase64Utility.ImportAudioFromBase64WithWebRequest(text, callback));
    }

    public void LoadAudioClipFromClipboard()
    {
        string clipboard = GUIUtility.systemCopyBuffer;
        LoadFromText(clipboard);


    }

}
