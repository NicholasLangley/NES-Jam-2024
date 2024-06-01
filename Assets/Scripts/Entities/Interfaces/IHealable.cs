using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable
{
    int _health { get; set; }
    int _maxHealth { get; set; }

    // Start is called before the first frame update
    void Heal(int heal);
}
