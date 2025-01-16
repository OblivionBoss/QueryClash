using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}