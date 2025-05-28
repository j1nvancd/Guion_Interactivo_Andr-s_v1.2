using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ConditionalText
{
    public string text;
    public string next; //ID del siguiente nodo
    public List<string> requiredFlags; //flag necesarias para que la opción aparezca
}