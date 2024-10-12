using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUnit : MonoBehaviour
{
    public float MaxHp;
    public float CurrentHp;

    void Start()
    {
        CurrentHp = MaxHp;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReduceHp(float Damage)
    {
        CurrentHp -= Damage;
        if (CurrentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
