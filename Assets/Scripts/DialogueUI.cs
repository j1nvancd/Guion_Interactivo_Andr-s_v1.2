using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class DialogueUI : MonoBehaviour 
{
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    private Dictionary<string, DialogueNode> nodes;
    private DialogueNode currentNode;

    public DialogueLoader loader;

    public Button continueButton; 


    //accede a las flaags activas desde el DialogueLoader
    private HashSet<string> activeFlags => loader.activeFlags;

    void Start()
    {
        nodes = loader.LoadDialogue();
        ShowNode("start"); //Es el nodo de inicio, recuerda que debe de existir en el JSON
    }

    public void ShowNode(string nodeID)
    {
        if (!nodes.ContainsKey(nodeID))
        {
            Debug.LogError("Nodo no encontrado: " + nodeID);
            return;
        }

        currentNode = loader.GetNode(nodeID); //un cambio del anterior, esto también contiene flags, antes estaba esto: currentNode = nodes[nodeID];

        speakerText.text = currentNode.speaker;

        string selectedText = currentNode.text; // obtiene el texto condicional si lo aplica

        if (currentNode.conditionalTexts != null && currentNode.conditionalTexts.Count > 0)
        {
            foreach (var ct in currentNode.conditionalTexts)
            {
                if (ct.requiredFlags == null || ct.requiredFlags.Any(flag => activeFlags.Contains(flag)))
                {
                    selectedText = ct.text;
                    break;
                }
            }
        }


        dialogueText.text = selectedText;

        //limpiar los botones de antes
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        //muestra las elecciones válidas
        foreach (DialogueChoice choice in currentNode.choices)
        {
            //si hay requiredFlags, comprueba que todas estén activas
            if (choice.requiredFlags != null && choice.requiredFlags.Count > 0)
            {
                bool anyActive = choice.requiredFlags.Any(flag => activeFlags.Contains(flag));
                if (!anyActive)
                    continue; //no mostrar si falta alguna flag requerida
            }
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                //añade flags si las hay
                if (choice.setFlags != null)
                {
                    foreach (string flag in choice.setFlags)
                    {
                        activeFlags.Add(flag);
                    }
                }
                ShowNode(choice.next);
            });
        }
        
        continueButton.gameObject.SetActive(false); // Ocultarlo por defecto

        if (currentNode.choices == null || currentNode.choices.Count == 0)
        {
            // Si es nodo final
            if (currentNode.end)
            {
                if (loader.LoadNextDialogue())
                {
                    continueButton.gameObject.SetActive(true);
                    continueButton.onClick.RemoveAllListeners();
                    continueButton.onClick.AddListener(() =>
                    {
                        nodes = loader.LoadDialogue();
                        ShowNode("start");
                        continueButton.gameObject.SetActive(false);
                    });
                }
                else
                {
                    // No hay más diálogos
                    Debug.Log("Fin de todos los diálogos.");
                }
            }
        }

    }
}
