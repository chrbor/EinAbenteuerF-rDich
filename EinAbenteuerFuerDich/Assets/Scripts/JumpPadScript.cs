using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    public float strength = 10;
    private AudioSource aSrc;

    private void Awake()
    {
        aSrc = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(other.gameObject.GetComponent<Rigidbody2D>().velocity.x, strength);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Squeesh");
        GetComponent<AudioSource>().Play();
    }
}
