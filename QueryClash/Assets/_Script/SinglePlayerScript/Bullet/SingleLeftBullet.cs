using UnityEngine;

public class SingleLeftBullet : SingleBullet
{

  

    public override void Move()
    {
        transform.position += Vector3.right * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        SingleSoldier soldier = collision.gameObject.GetComponent<SingleSoldier>();
        if (collision.gameObject.CompareTag("RightTeam") && soldier != null && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk);
            Destroy(gameObject);
        }
    }
}
