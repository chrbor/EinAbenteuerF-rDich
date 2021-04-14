using UnityEngine;
using static GameManager;
public class Seq_Spa_spa : MonoBehaviour
{
    public Seq_Spa spa;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || !spa.glassReceived) return;
        spa.GetBrew(gameObject);
        progress.brewDone = true;
        LoadSave.SaveProgress();
        Destroy(this);
    }
}
