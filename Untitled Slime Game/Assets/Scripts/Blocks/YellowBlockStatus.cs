using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBlockStatus : BlockStatus {
    override public void TakeDamage(int bulletColor) {
        if (bulletColor == 3) {
            Die();
        }
    }
}
