using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Texture2DList")]
public class Texture2DListVariable : ScriptableObject
{
    public List<Texture2D> list = new List<Texture2D>();
}
