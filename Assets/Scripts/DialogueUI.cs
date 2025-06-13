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
    ShowNode("start");

    continueButton.gameObject.SetActive(true); // Siempre visible

    continueButton.onClick.RemoveAllListeners();
    continueButton.onClick.AddListener(() =>
    {
        if (loader.LoadNextDialogue())
        {
            nodes = loader.LoadDialogue();
            ShowNode("start");
        }
        else
        {
            Debug.Log("No hay más diálogos para cargar.");
            // Opcional: aquí puedes desactivar el botón o mostrar mensaje en UI
            // continueButton.gameObject.SetActive(false);
        }
    });
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
            GameObject entryObj = Instantiate(choiceButtonPrefab, choiceContainer);

            // Buscar los componentes internos
            TextMeshProUGUI textComponent = entryObj.transform.Find("ChoiceText").GetComponent<TextMeshProUGUI>();
            Button selectButton = entryObj.transform.Find("SelectButton").GetComponent<Button>();

            textComponent.text = choice.text;

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() =>
            {
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
        
        

    }
}
