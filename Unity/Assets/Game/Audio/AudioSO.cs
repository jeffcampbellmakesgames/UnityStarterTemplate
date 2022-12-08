using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Track List")]
public class AudioSO : ScriptableObject
{
	[SerializeField]
	private Dictionary<string, AudioClip> _bgmDictionary;
	[SerializeField]
	private Dictionary<string, AudioClip> _sfxDictionary;

	
}
