using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternFlashLight : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int index = collision.gameObject.transform.GetSiblingIndex();
        PatternPurple scripts = GameObject.Find("MiniManager").GetComponent<PatternPurple>();
        if (collision.tag == "Gimmick" && scripts.IsMatchGimmick(index, collision.gameObject))
        {
            Debug.Log("Ãæµ¹!" + index);

            // collision fade out
            StartCoroutine(FadeOut(collision.gameObject.GetComponent<SpriteRenderer>(), 1f));

            // collision destroy
            scripts.DeleteEye(index);
            StartCoroutine(CollisionDestroy(collision.gameObject));
            

            // collision recreate
            Invoke(nameof(AddEye), 5f);
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sr, float time)
    {
        while (sr.color.a >= 0f)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime / time);
            yield return null;
        }
    }

    private IEnumerator CollisionDestroy(GameObject obj)
    {
        yield return new WaitForSeconds(1f);

        Destroy(obj);
    }

    private void AddEye()
    {
        GameObject.Find("MiniManager").GetComponent<PatternPurple>().AddEye();
    }
}
