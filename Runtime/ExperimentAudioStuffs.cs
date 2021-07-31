

///https://stackoverflow.com/questions/35228767/noisy-audio-clip-after-decoding-from-base64
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExperimentAudioStuffs : MonoBehaviour
{
    public TextAsset m_audioInBase64AsWav;
    public AudioSource m_audioSource;


    private void Awake()
    {
        PlaySound();
    }

    void PlaySound()
    {
        string scat = m_audioInBase64AsWav.text;
        byte[] bcat = System.Convert.FromBase64String(scat);
        float[] f = ConvertByteToFloat(bcat);
        Normalize(f);

        byte[] byteArray = new byte[f.Length * 4];
        Buffer.BlockCopy(f, 0, byteArray, 0, byteArray.Length);

        AudioClip audioClip = AudioClip.Create("testSound", f.Length, 2, 44100, false, false);
        audioClip.SetData(f, 0);
        m_audioSource.PlayOneShot(audioClip);
        //System.IO.File.WriteAllBytes("Assets/final.wav", byteArray);
    }
     float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 2];

        for (int i = 0; i < floatArr.Length; i++)
        {
            floatArr[i] = ((float) BitConverter.ToInt16(array, i * 2)) / 32768f;
        }

        return floatArr;
    }

    /// <summary>
    /// Normalizes the values within this array.
    /// </summary>
    /// <param name="data">The array which holds the values to be normalized.</param>
     void Normalize( float[] data)
    {
        float max = float.MinValue;

        // Find maximum
        for (int i = 0; i < data.Length; i++)
        {
            if (Math.Abs(data[i]) > max)
            {
                max = Math.Abs(data[i]);
            }
        }

        // Divide all by max
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = data[i] / max;
        }
    }
}
