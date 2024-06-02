using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{

    #region State Machine States
    public PlayerStateMachine _StateMachine { get; set; }

    #endregion

    #region IDamageable
    public int _health { get; set; }
    [field: SerializeField, Header("IDamageable")] public int _maxHealth { get; set; }
    #endregion



    void Awake()
    {
        _StateMachine = new PlayerStateMachine();

        //child class should create individual states
    }
    // Start is called before the first frame update
    void Start()
    {
        //child class should initialize state machine
        _health = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region IMoveable
    public void Damage(int damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        GameObject.Destroy(gameObject);
    }

    #endregion
}
