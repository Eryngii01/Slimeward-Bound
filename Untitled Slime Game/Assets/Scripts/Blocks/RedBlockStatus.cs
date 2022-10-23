using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBlockStatus : BlockStatus {
    override public void TakeDamage(int bulletColor) {
        if (bulletColor == 2) {
            Die();
        }
    }
}
