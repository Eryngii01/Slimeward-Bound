using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBlockStatus : BlockStatus {
    override public void TakeDamage(int bulletColor) {
        if (bulletColor == 0) {
            Die();
        }
    }
}
