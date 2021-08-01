using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;

public class ExampleConvert : MonoBehaviour {
	public AudioClip m_yourClip;
	public string m_base64OfYourClip;
	public string m_base64WithHeaderOfYourClip;
	public AudioClip m_reimportOfYourClip;
	public string m_reimportError;

	void Start () {

		AudioClipToBase64Utility.ConvertAudioToBase64(m_yourClip, out m_base64OfYourClip, out m_base64WithHeaderOfYourClip);
		AudioClipCallBack callbackLoading = new AudioClipCallBack();
		callbackLoading.AddListener(ClipRecovered);
		StartCoroutine( AudioClipToBase64Utility.ImportAudioFromBase64WithWebRequest(m_base64OfYourClip, callbackLoading));

		//EncodeMP3.SaveAudioClipAsMP3 (m_yourClip, Application.dataPath + "/convertedMp3.mp3", 128);
		//Application.OpenURL(Application.dataPath + "/convertedMp3.mp3");
		//Application.OpenURL(Application.dataPath );
		//EncodeMP3.ConvertAudioClipToBase64(m_yourClip, 128, out byte [] b,  out  m_base64OfYourClip, out m_base64WithHeaderOfYourClip);
		//File.WriteAllText(Application.dataPath + "/convertedMp3.base64.txt", m_base64WithHeaderOfYourClip);

	}

    private void ClipRecovered(AudioClipCallBack source)
    {
		m_reimportError = source.GetError();
		if (string.IsNullOrEmpty(m_reimportError))
			m_reimportOfYourClip = source.GetClip();
		else m_reimportOfYourClip = null;

	}


}

public class AudioClipToBase64Utility {

	public static void ConvertAudioToBase64(AudioClip clip, out string base64, out string base64WithHeader, int bitRate = 128)
	{
		EncodeMP3.ConvertAudioClipToBase64(clip, bitRate, out byte[] b, out base64, out base64WithHeader);
	}

	public static void SaveAudioClipToMP3File(string filePath, AudioClip clip, int bitRate = 128)
	{
		EncodeMP3.SaveAudioClipAsMP3(clip, filePath, bitRate);
	}
	public static void SaveAudioClipToBase64File(string filePath, AudioClip clip, int bitRate = 128)
	{
		EncodeMP3.ConvertAudioClipToBase64(clip, bitRate, out byte[] b, out string b64, out string b64withheader);
		File.WriteAllText(filePath, b64withheader);
	}

	public static IEnumerator ImportAudioFromBase64WithWebRequest(string base64Text, AudioClipCallBack clipCallBack)
	{
		if (base64Text == null || base64Text.Length <= 0) {
			clipCallBack.SetAsErrorHappen("Base 64 should not be null or empty");
			clipCallBack.NotifyListenerOfChange();
			yield break;
		}

		int indexRemoveHeader = base64Text.IndexOf("base64,");
		if (indexRemoveHeader > 0) {
			base64Text = base64Text.Substring(indexRemoveHeader + "base64,".Length);
		}
		string filePath = Application.temporaryCachePath + "/CreateInstanceOfReimportingBase64.mp3";
		try
		{
			File.WriteAllBytes(filePath, Convert.FromBase64String(base64Text));
		}
		catch (Exception e) {
			clipCallBack.SetAsErrorHappen("Had some difficulty to creat the mp3 file to be read from:\n "+e.StackTrace);
			clipCallBack.NotifyListenerOfChange();
			yield break;
		}

		// should I add "file://"
		using (UnityWebRequest web = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
		{
			yield return web.SendWebRequest();
			if (web.result== UnityWebRequest.Result.Success)
			{
				clipCallBack.SetAsDownloaded(DownloadHandlerAudioClip.GetContent(web));
			}
			else {

				clipCallBack.SetAsErrorHappen("Error happen when loading the file.");
			}
		}
		clipCallBack.NotifyListenerOfChange();
	}
	public void ImportAudioFromBase64(string base64Text, AudioClip clip)
	{
		throw new NotImplementedException("Need to study the Tool to go in base64 to audioclip way");
	}

    public static IEnumerator LoadAudioBase64FromWebpageOrFile(string uri, AudioClipCallBack callback)
	{
		string textThatSouldBeBase64 = "";
		string path = uri;
		if (File.Exists(path))
			path = "file://" + path;
		using (UnityWebRequest web = UnityWebRequest.Get(path))
		{
			yield return web.SendWebRequest();
			if (web.result == UnityWebRequest.Result.Success)
			{
				textThatSouldBeBase64 = web.downloadHandler.text ;
			}
			else
			{

				callback.SetAsErrorHappen("Error found at the uri:"+uri);
				yield break;
			}
		}
		yield return ImportAudioFromBase64WithWebRequest(textThatSouldBeBase64, callback);
	}

}

public class AudioClipCallBack {

	private AudioClip m_clip;
	private string m_errorMessage;
	private AudioClipLoadingEvent m_finishLoading;
	public delegate void AudioClipLoadingEvent(AudioClipCallBack source);


	public string GetError() { return m_errorMessage; }
	public AudioClip GetClip() { return m_clip; }
	public void SetAsDownloaded(AudioClip audioClip)
    {
		m_clip = audioClip;
	}

	public void SetAsErrorHappen(string errorDebugMessage)
    {
		m_errorMessage = errorDebugMessage;
    }

	public void AddListener(AudioClipLoadingEvent listener) { m_finishLoading += listener; }
	public void RemoveListener(AudioClipLoadingEvent listener) { m_finishLoading -= listener; }

	public void NotifyListenerOfChange() { 
		if(m_finishLoading!=null)
		m_finishLoading(this); 
	}
}