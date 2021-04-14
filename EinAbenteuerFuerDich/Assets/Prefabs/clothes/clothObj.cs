using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new cloth", menuName = "cloth")]
public class clothObj : ScriptableObject
{
    public Sprite sprite;
    public Vector2 position;
    public float rotation;
    public float scale = 1;
    public bool isHat;

    public void CreateCloth_Instant(Transform parent = null)
    {
        if (!sprite) return;

        GameObject obj_ref = new GameObject("cloth_" + parent.childCount);
        SpriteRenderer spriteRenderer = obj_ref.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerID = parent.GetComponent<SpriteRenderer>().sortingLayerID;
        spriteRenderer.sortingOrder = 3;

        obj_ref.transform.parent = parent;

        obj_ref.transform.localPosition = position;
        obj_ref.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        obj_ref.transform.localScale = Vector3.one * scale;
    }

    public static void DestroyCloths_Instant(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--) Destroy(parent.GetChild(i));
    }

    public IEnumerator CreateCloth(Transform parent = null)
    {
        if (!sprite) yield break;

        GameObject obj_ref = new GameObject("cloth_" + parent.childCount);
        SpriteRenderer spriteRenderer = obj_ref.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerID = parent.GetComponent<SpriteRenderer>().sortingLayerID;
        spriteRenderer.sortingOrder = 20 + parent.childCount;

        obj_ref.transform.parent = parent;

        Vector2 startPos = position + (isHat? Vector2.up : Vector2.zero);
        obj_ref.transform.localPosition = isHat? startPos : position;
        obj_ref.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        obj_ref.transform.localScale = Vector3.one * scale * .5f;
        spriteRenderer.color = new Color(1, 1, 1, 0);

        //Zoom and fadeIn:
        Color stepColor = Color.black * Time.fixedDeltaTime * 2;
        Vector3 scalestep = Vector3.one * scale * Time.fixedDeltaTime * 2;
        for (float count = 0; count < .5f; count += Time.fixedDeltaTime)
        {
            spriteRenderer.color += stepColor;
            obj_ref.transform.localScale += scalestep;
            yield return new WaitForFixedUpdate();
        }
        scalestep /= 2;
        for (float count = 0; count < .5f; count += Time.fixedDeltaTime)
        {
            obj_ref.transform.localScale -= scalestep;
            yield return new WaitForFixedUpdate();
        }

        if (!isHat) yield break;

        //Flip and Fall:
        float rot = 360 * Time.fixedDeltaTime;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            obj_ref.transform.Rotate(Vector3.forward, rot);
            obj_ref.transform.localPosition = startPos + (-1.89f * (count - .23f) * (count - .23f) + .1f) * Vector2.up;//Parabel mit Scheitelpunkt bei .1f und P(1|-1)
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public static IEnumerator DestroyCloths(Transform parent)
    {
        if (parent == null) yield break;
        SpriteRenderer[] spriteRenderer = new SpriteRenderer[parent.childCount];
        for(int i = 0; i < parent.childCount; i++) spriteRenderer[i] = parent.GetChild(i).GetComponent<SpriteRenderer>();

        Color stepColor = Color.black * Time.fixedDeltaTime;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            foreach(var sr in spriteRenderer) sr.color -= stepColor;
            yield return new WaitForFixedUpdate();
        }
        for (int i = spriteRenderer.Length - 1; i >= 0; i--) Destroy(parent.GetChild(i).gameObject);
        yield break;
    }
}
