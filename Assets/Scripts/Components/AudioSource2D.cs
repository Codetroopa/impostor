﻿using UnityEngine;

/// <summary>
/// An audio source that has its volume managed by the inverse square law on the X and Y axis ONLY.
/// </summary>
public class AudioSource2D : MonoBehaviour {

    public AudioClip audioClip;
    public bool playOnAwake = true;

    // Anything as close as this distance can be heard at full volume
    public int minDistance = 1;

    // The max distance in which this audio source can no longer be heard
    public int maxDistance = 24;

    AudioSource _src;

	void Start () {
        _src = gameObject.AddComponent<AudioSource>();
        _src.clip = audioClip;
        _src.spatialBlend = 0;

        if (playOnAwake) {
            Play();
        }
	}

    // Play the audio source with 2D spatial blending
    public void Play() {
        float distance = GetListenerDistance();
        float scale = CalculateAudioScale(distance);

        Debug.Log("Playing at %" + (scale * 100).ToString() + " volume");
        PlayAt(scale);
    }

    // Play the audio source with no spatial blending (max volume no matter where the listener is)
    public void PlayMax() {
        _src.Play();
    }

    // Play at a given scale in the range [0, 1]
    public void PlayAt(float scale) {
        scale = Mathf.Clamp01(scale);
        _src.PlayOneShot(audioClip, scale);
    }

    // Play given audio clip, overriding the default one on this component. Does not affect future Sounds.
	public void PlayOverride(string clip) {
		_src.clip = Resources.Load(clip) as AudioClip;
		Debug.Log (_src.clip);
        Play();
		_src.clip = audioClip;
    }

    // Returns the distance between the LocalPlayer's audio listener and this audio source
    float GetListenerDistance() {
        GameObject player = GameManager.Instance.GetLocalPlayer();
        return Vector3.Distance(player.transform.position, transform.position);
    }

    // Calculates audio intensity based off of distance from target (X and Y axis)
    float CalculateAudioScale(float distance) {
        Debug.Log(distance);
        if (distance >= maxDistance) {
            return 0f;
        }
        if (distance <= minDistance) {
            return 1f;
        }

        float ratio = 10f / (Mathf.Pow(distance - minDistance, 2) + 1f);
        return Mathf.Clamp01(ratio);
    }
}
