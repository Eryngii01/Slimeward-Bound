using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelector : MonoBehaviour {
    [SerializeField]
    private int _color;
    [SerializeField]
    private bool _isPlayer;

    private Animator _anim;

    void Awake() {
        _anim = GetComponent<Animator>();

        if (_isPlayer) {
            switch (_color) {
                case 0:
                    _anim.Play("Blue Idle");
                    break;
                case 1:
                    _anim.Play("Green Idle");
                    break;
                case 2:
                    _anim.Play("Red Idle");
                    break;
                case 3:
                    _anim.Play("Yellow Idle");
                    break;
            }
        }
    }

    public void SetColor(int color) {
        if (!_isPlayer) {
            switch (color) {
                case 0:
                    _anim.Play("Blue Fall");
                    break;
                case 1:
                    _anim.Play("Green Fall");
                    break;
                case 2:
                    _anim.Play("Red Fall");
                    break;
                case 3:
                    _anim.Play("Yellow Fall");
                    break;
                case 4:
                    _anim.Play("Water Fall");
                    break;
            }
        }
        
    }
}
