using DT;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DT.Game {
  public class SoundManager : Singleton<SoundManager> {
    public void PlaySoundFile(int soundClipIndex) {
      AudioClip clip = this._soundClips[soundClipIndex];
      this._audioSource.PlayOneShot(clip);
    }

    public void RestartBGMusic() {
      this._bgAudioSource.Play();
    }

    [SerializeField]
    private AudioClip[] _soundClips;

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioSource _bgAudioSource;
  }
}
