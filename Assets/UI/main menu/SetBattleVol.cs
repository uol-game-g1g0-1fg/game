using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetBattleVol : MonoBehaviour
{

	public AudioMixer mixer;

	public void SetLevel(float sliderVal)
	{
		mixer.SetFloat("BattleVol", Mathf.Log10(sliderVal) * 20);
	}

}
