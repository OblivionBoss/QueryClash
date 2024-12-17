using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBullet : MonoBehaviour
{
    public float bulletspeed = 1;
    public float deadZone;
    public float Atk = 10;

    void Update() // Update positions of certain bullets
    {
        transform.position = transform.position + (Vector3.right * bulletspeed) * Time.deltaTime;
        if (transform.position.x > deadZone)
        {
            //Debug.Log("Bullet Deleted");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision) // Check when bullet hit a target and check if target is an enemy or not
    {

        Soldier s = collision.gameObject.GetComponent<Soldier>();
        if (collision.gameObject.CompareTag("RightTeam") && s.isPlaced)
        {
            if (s != null)
            {
                //Debug.Log($"Bullet Atk: {Atk}");
                //Debug.Log($"Soldier CurrentHp before: {s.CurrentHp}");

                // Reduce the HP of the collided unit
                s.ReduceHp(Atk);

                //Debug.Log($"Soldier CurrentHp after: {s.CurrentHp}");
            }
            Destroy(gameObject);
        }
    }



}
