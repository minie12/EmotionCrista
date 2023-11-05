using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternEye : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int index = this.gameObject.transform.GetSiblingIndex();
        int index2 = collision.gameObject.transform.GetSiblingIndex();
        PatternPurple scripts = GameObject.Find("MiniManager").GetComponent<PatternPurple>();

        //if (collision.tag == "Gimmick" && index > index2)
        //{
        //    Debug.Log("collision eye!");
        //    scripts.DeleteEye(index);
        //    scripts.AddEye();
        //    Destroy(this.gameObject);
        //}
    }
}
