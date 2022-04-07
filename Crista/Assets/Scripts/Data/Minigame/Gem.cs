using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gem", menuName = "Gem")]
public class Gem : ScriptableObject
{
    public int gem_color;
    public Sprite gem_image;
}
