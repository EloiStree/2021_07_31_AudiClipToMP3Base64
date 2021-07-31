/*                    GNU GENERAL PUBLIC LICENSE
                       Version 3, 29 June 2007

 Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.

*/

/*---------------------- BeatUp (C) 2016-------------------- */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using NAudio.Wave;
using NAudio.Lame;

public static class EncodeMP3
{


	public static void SaveAudioClipAsMP3 (AudioClip clip, string path, int bitRate)
	{
		if (!path.EndsWith (".mp3"))
			path = path + ".mp3";
		ConvertAndWrite (clip, path, bitRate);
	}




	public static void ConvertAudioClipToBase64(AudioClip clip, int bitRate, out byte[] mp3AsBytes , out string base64, out string base64WithHeader) {

		ConvertAudioClipToMP3Bytes(clip, bitRate, out mp3AsBytes);
		base64 = Convert.ToBase64String(mp3AsBytes);
		base64WithHeader = "data:audio/mpeg;base64," + base64;
	}

	public static void ConvertAudioClipToMP3Bytes(AudioClip clip, int bitRate, out byte [] bytesAsMp3File)
	{
		var samples = new float[clip.samples * clip.channels];

		clip.GetData(samples, 0);

		Int16[] intData = new Int16[samples.Length];
		//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

		Byte[] bytesData = new Byte[samples.Length * 2];
		//bytesData array is twice the size of
		//dataSource array because a float converted in Int16 is 2 bytes.

		float rescaleFactor = 32767; //to convert float to Int16

		for (int i = 0; i < samples.Length; i++)
		{
			intData[i] = (short)(samples[i] * rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(intData[i]);
			byteArr.CopyTo(bytesData, i * 2);
		}

		bytesAsMp3File = ConvertWavToMp3(bytesData, bitRate);
	}




	//  derived from Gregorio Zanon's script
	private static void ConvertAndWrite (AudioClip clip, string path, int bitRate)
	{
		var samples = new float[clip.samples * clip.channels];

		clip.GetData (samples, 0);

		Int16[] intData = new Int16[samples.Length];
		//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

		Byte[] bytesData = new Byte[samples.Length * 2];
		//bytesData array is twice the size of
		//dataSource array because a float converted in Int16 is 2 bytes.

		float rescaleFactor = 32767; //to convert float to Int16

		for (int i = 0; i < samples.Length; i++) {
			intData [i] = (short)(samples [i] * rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes (intData [i]);
			byteArr.CopyTo (bytesData, i * 2);
		}

		File.WriteAllBytes (path, ConvertWavToMp3 (bytesData,bitRate));
	}



	private static byte[] ConvertWavToMp3 (byte[] wavFile, int bitRate)
	{

		var retMs = new MemoryStream ();
		var ms = new MemoryStream (wavFile);
		var rdr = new RawSourceWaveStream (ms, new WaveFormat ());
		var wtr = new LameMP3FileWriter (retMs, rdr.WaveFormat, bitRate);

		rdr.CopyTo (wtr);
		return retMs.ToArray ();



	}
	
	



}