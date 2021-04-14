using UnityEngine;

public class Seq_Spa_glass : MonoBehaviour
{
    public Seq_Spa spa;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped) return;
        spa.GetGlass(gameObject);
        Destroy(this);
    }
}
