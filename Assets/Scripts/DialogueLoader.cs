using UnityEngine;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour 
{
    public TextAsset dialogueJSON;

    private Dictionary<string, DialogueNode> dialogueNodes;
    public HashSet<string> activeFlags = new HashSet<string>();

    public Dictionary<string, DialogueNode> LoadDialogue()
    {
        DialogueNode[] nodes = JsonHelper.FromJson<DialogueNode> (dialogueJSON.text);
        dialogueNodes = new Dictionary<string, DialogueNode>();

        foreach (DialogueNode node in nodes)
        {
            dialogueNodes[node.id] = node;
        }
        return dialogueNodes;
    }

    public DialogueNode GetNode(string id)
    {
        var node = dialogueNodes[id];

        //Activa las flags del nodo si las tiene
        if (node.setFlags != null)
        {
            foreach (var flag in node.setFlags)
            {
                activeFlags.Add(flag);
            }
        }
        return node;
    }
}
