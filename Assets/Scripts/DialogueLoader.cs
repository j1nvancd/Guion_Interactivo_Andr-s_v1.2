using UnityEngine;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour
{
    public List<TextAsset> dialogueJSONs;
    private int currentDialogueIndex = 0;

    private Dictionary<string, DialogueNode> dialogueNodes;
    public HashSet<string> activeFlags = new HashSet<string>();

    public Dictionary<string, DialogueNode> LoadDialogue()
    {
        return LoadDialogueFromIndex(currentDialogueIndex);
    }

    public Dictionary<string, DialogueNode> LoadDialogueFromIndex(int index)
    {
        if (index >= dialogueJSONs.Count) return null;

        DialogueNode[] nodes = JsonHelper.FromJson<DialogueNode>(dialogueJSONs[index].text);
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
    public bool LoadNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex >= dialogueJSONs.Count)
            return false;

        LoadDialogueFromIndex(currentDialogueIndex);
        return true;
    }
}
