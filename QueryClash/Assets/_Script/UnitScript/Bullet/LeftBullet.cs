using UnityEngine;

public class LeftBullet : Bullet
{

  

    public override void Move()
    {
        transform.position += Vector3.right * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        Soldier soldier = collision.gameObject.GetComponent<Soldier>();
        if (collision.gameObject.CompareTag("RightTeam") && soldier != null && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk.Value);
            Destroy(gameObject);
        }
    }
}
