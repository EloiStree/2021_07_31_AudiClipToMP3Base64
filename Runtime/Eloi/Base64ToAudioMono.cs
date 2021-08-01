using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base64ToAudioMono : MonoBehaviour
{
   // "data:audio/mpeg;base64,"
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class Base64AudioUtility {

    public void BaseToSound(string base64, out AudioClip clip )
    {
        throw new NotImplementedException();
    }
    public void SoundToBase(AudioClip clip, out string base64, out string base64WithHeader)
    {
        //"data:audio/mpeg;base64,"
        //"data:audio/x-wav;base64,"
        throw new NotImplementedException();

    }


}
