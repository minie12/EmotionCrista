using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogExtra : MonoBehaviour
{
    public void SetCharacterName(string nameText_, Fungus.Character player){        
        player.SetStandardText(nameText_);
    }
}

