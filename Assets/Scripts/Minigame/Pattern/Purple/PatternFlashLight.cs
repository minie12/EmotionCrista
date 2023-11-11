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

            // collision destroy
            scripts.DeleteEye(index);
            StartCoroutine(CollisionDestroy(collision.gameObject));
            
            // collision recreate
            Invoke(nameof(AddEye), 5f);
        }
    }

    private IEnumerator CollisionDestroy(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);

        // collision fade out
        obj.gameObject.GetComponent<PatternEye>().AllChildrenFadeOut();

        yield return new WaitForSeconds(1.0f);

        Destroy(obj);
    }

    private void AddEye()
    {
        GameObject.Find("MiniManager").GetComponent<PatternPurple>().AddEye();
    }
}
