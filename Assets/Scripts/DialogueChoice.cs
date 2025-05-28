using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public string next; //ID del siguiente nodo
    public List<string> requiredFlags; //flag necesarias para que la opción aparezca
    public List<string> setFlags; //Flags que se activan al elegir la opción
}