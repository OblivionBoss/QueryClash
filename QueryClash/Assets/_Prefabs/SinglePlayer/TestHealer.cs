using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHealer : SingleSoldier
{

   
    private void OnTriggerEnter(Collider other)
    {
        SingleSoldier singleSoldier = other.GetComponent<SingleSoldier>();

        if (singleSoldier != null) // Check if the collided object has a SingleSoldier component
        {
            Debug.Log($"Hit {singleSoldier.name} at {singleSoldier.transform.position}");
        }
    }
}
