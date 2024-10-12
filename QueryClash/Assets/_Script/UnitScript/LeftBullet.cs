using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBullet : MonoBehaviour
{
    public float bulletspeed;
    public float deadZone;
    public float Damage;

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

        if (collision.gameObject.CompareTag("RightTeam"))
        {
            CubeUnit u = collision.gameObject.GetComponent<CubeUnit>();
            u.ReduceHp(Damage);
            Destroy(gameObject);
        }
    }

  
}
