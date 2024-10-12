using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightBullet : MonoBehaviour
{
    public float bulletspeed;
    public float deadZone;
    public float Damage;

    void Update()
    {
        transform.position = transform.position + (Vector3.left * bulletspeed) * Time.deltaTime;
        if (transform.position.x < deadZone)
        {
            //Debug.Log("Bullet Deleted");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("LeftTeam"))
        {
            CapsuleUnit u = collision.gameObject.GetComponent<CapsuleUnit>();
            u.ReduceHp(Damage);
            Destroy(gameObject);
        }
    }

}
