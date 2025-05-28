using UnityEngine;
using System.Collections.Generic;

    [System.Serializable]
    public class DialogueNode
    {
        public string id;
        public string speaker;
        public string text;
        public List<ConditionalText> conditionalTexts;
        public List<DialogueChoice> choices;
        public List<string> setFlags; //Las flags que activa el nodo
    }