using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeakController : MonoBehaviour {
    [SerializeField]
    private GameObject _water;
    [SerializeField]
    private Transform _waterLeakSource;
    [SerializeField]
    private float _timeBetweenDrop;
    private float _dropTimer = -1, _musicTimer = 1.15f;

    private AudioSource _audio;

    void Awake() {
        _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (_dropTimer < 0) {
            GameObject bulletInstance = Instantiate(_water, _waterLeakSource.position, _water.transform.rotation);
            bulletInstance.GetComponent<BulletController>().SetColor(4);
            bulletInstance.GetComponent<BulletController>().SetDirection(Vector3.down);

            _dropTimer = _timeBetweenDrop;
        } else {
            _dropTimer -= Time.deltaTime;
        }

        if (_musicTimer < 0) {
            _audio.Play();

            _musicTimer = _timeBetweenDrop;
        } else {
            _musicTimer -= Time.deltaTime;
        }
    }
}
