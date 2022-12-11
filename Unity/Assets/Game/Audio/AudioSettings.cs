using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using SuperUnityBuild.BuildTool;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Track List")]
public class AudioSettings : ScriptableObject
{
	
	public SerializableDictionaryBase<string, AudioClip> bgmDictionary;
	public SerializableDictionaryBase<string, AudioClip> sfxDictionary;
	public float crossfadeTime;
}
