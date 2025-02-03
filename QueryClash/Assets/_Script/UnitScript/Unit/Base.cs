using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class Base : Soldier
{
    //[SyncVar] public float tempHpForNetwork;

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    this.GiveOwnership(base.Owner);
    //    if (!base.IsOwner) GetComponent<Base>().enabled = false;
    //}

    void Start()
    {
        OnPlaced();
        CurrentHp = MaxHp;
        isBase = true;
    }

    public override void ReduceHp(float damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Base : Soldier
//{


//    // Start is called before the first frame update
//    void Start()
//    {
//        OnPlaced();
//        CurrentHp = MaxHp;
//        isBase = true;
//    }

//    public override void ReduceHp(float damage)
//    {
//        CurrentHp -= damage;
//        if (CurrentHp <= 0)
//        {
//            Destroy(gameObject);
//        }
//    }
//}
