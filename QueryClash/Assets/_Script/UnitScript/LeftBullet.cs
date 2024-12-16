using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBullet : MonoBehaviour
{
    public float bulletspeed = 1;
    public float deadZone;
    public float Atk = 10;

    void Update()
    {   
        transform.position = transform.position + (Vector3.right * bulletspeed) * Time.deltaTime;
        if (transform.position.x > deadZone)
        {
            //Debug.Log("Bullet Deleted");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
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
