using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage menu buttons sounds
/// Improve and adapt this file from : https://github.com/atmosgames/unityMenuTutorial
/// 
/// Author : Alexander Wohlfahrt
/// Version : 0.1
/// </summary>
public class AnimatorFunctions : MonoBehaviour
{
	[SerializeField] private MenuButtonController menuButtonController;
	public bool disableOnce;

	private void PlaySound(AudioClip whichSound)
    {
		if(!disableOnce)
        {
			menuButtonController.ButtonAudioSource.PlayOneShot (whichSound);
		}
        else
        {
			disableOnce = false;
		}
	}
}	
