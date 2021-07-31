All credit to  'BeatUpir' that used the code of  'Gregorio Zanon'.

My code is just:
- adaptation that allows you to use the code as a unity package with the new package manager.
- The addition of the ability to record the microphone and import export the sound as base64 string.

Hope this code help the community,
Big thanks to ['BeatUpir'](https://github.com/BeatUpir/Unity3D-save-audioClip-to-MP3) !

Kind regards,
Eloi

--------------------------------------
Original Read me >
--------------------------------------
# Unity3D-save-audioClip-to-MP3
with this package you can save an audioclip to mp3 in unity3d
It works with both Windows and Android


Usage:

```c#
EncodeMP3.convert (AudioClip clip, string path, int BitRate);
```

If any errors occured you need to change your .NET API level in unity build settings.

For example for Android:
File => Build Settings => Player Settings => (in Inspector) Other Settings => Optimization => Api Compatibility Level : .Net 2.0 
