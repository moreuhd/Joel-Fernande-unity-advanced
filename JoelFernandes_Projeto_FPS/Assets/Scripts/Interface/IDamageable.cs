using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage); 

    void Knockback(Vector3 direction, float force);
}
