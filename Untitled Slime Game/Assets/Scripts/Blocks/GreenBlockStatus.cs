using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBlockStatus : BlockStatus {

    override public void TakeDamage(int bulletColor) {
        if (bulletColor == 1) {
            Die();
        }
    }
}
