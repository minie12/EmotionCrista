using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator anim;

    [SerializeField]
    private string parameterName;

    public void OnClickButtonToggle()
    {
        if (anim.GetBool(parameterName))
        {
            anim.SetBool(parameterName, false);
        }
        else
        {
            anim.SetBool(parameterName, true);
        }
    }
}
