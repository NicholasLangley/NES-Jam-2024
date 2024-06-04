using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int _health { get; set; }
    int _maxHealth { get; set; }

    bool _damageBounceDirectionIsRight { get; set; }
    float _bounceYVelocity { get; set; }
    float _bounceSpeed { get; set; }

    // Start is called before the first frame update
    void Damage(int damage);

    void Kill();
}
