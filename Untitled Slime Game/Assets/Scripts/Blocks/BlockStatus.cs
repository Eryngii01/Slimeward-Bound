using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BlockStatus : MonoBehaviour {
    abstract public void TakeDamage(int bulletColor);

    protected void Die() {
        MusicManager.Instance.PlayDestroyBlock();
        Destroy(this.gameObject);
    }
}
