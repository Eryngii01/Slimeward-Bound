using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    private static MusicManager _instance;
    public static MusicManager Instance {
        get { return _instance; }
    }

    [SerializeField]
    private float _jumpVol, _shootVol;

    private AudioSource _audio;

    // Player SFX
    [SerializeField]
    private AudioClip _jump, _shoot, _duplicate, _absorb, _getPowerUp, _die;
    // UI SFX
    [SerializeField]
    private AudioClip _openMenu, _clickMenu, _deniedMenu, _type;
    // Damaging SFX
    [SerializeField]
    private AudioClip _sizzle, _hurt, _destroyBlock;

    [SerializeField]
    private AudioSource _backgroundMusic;

    [SerializeField]
    private AudioClip _win, _lose, _bossBattle, _levelBGM;

    void Awake() {
        _instance = this;
        _audio = GetComponent<AudioSource>();
    }

    public void PlayJump() {
        _audio.PlayOneShot(_jump, _jumpVol);
    }

    public void PlayShoot() {
        _audio.PlayOneShot(_shoot, _shootVol);
    }

    public void PlayDuplicate() {
        _audio.PlayOneShot(_duplicate, _shootVol);
    }

    public void PlayAbsorb() {
        _audio.PlayOneShot(_absorb, _shootVol);
    }

    public void PlayPowerUp() {
        _audio.PlayOneShot(_getPowerUp, 1);
    }

    public void PlayDie() {
        _audio.PlayOneShot(_die, 1);
    }

    public void PlayOpenMenu() {
        _audio.PlayOneShot(_openMenu, 1);
    }

    public void PlayClick() {
        _audio.PlayOneShot(_clickMenu, 1);
    }

    public void PlayDeniedMenu() {
        _audio.PlayOneShot(_deniedMenu, 1);
    }

    public void PlayType() {
        _audio.PlayOneShot(_type, 1);
    }

    public void PlaySizzle() {
        _audio.PlayOneShot(_sizzle, 1);
    }

    public void PlayHurt() {
        _audio.PlayOneShot(_hurt, 1);
    }

    public void PlayDestroyBlock() {
        _audio.PlayOneShot(_destroyBlock, 1);
    }

    public void PlayWin() {
        _backgroundMusic.clip = _win;
        _backgroundMusic.loop = true;
        _backgroundMusic.Play();
    }

    public void PlayLose() {
        _backgroundMusic.clip = _lose;
        _backgroundMusic.loop = false;
        _backgroundMusic.Play();
    }

    public void PlayBattle() {
        _backgroundMusic.clip = _bossBattle;
        _backgroundMusic.loop = true;
        _backgroundMusic.Play();
    }

    public void PlayBGM() {
        _backgroundMusic.clip = _levelBGM;
        _backgroundMusic.loop = true;
        _backgroundMusic.Play();
    }
}
