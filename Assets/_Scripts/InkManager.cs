using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class InkManager : MonoBehaviour
{
    private Story inkStory;
    public Text inkTextUI;

    void Start()
    {
        TextAsset inkJSONAsset = Resources.Load<TextAsset>("GameStory");
        inkStory = new Story(inkJSONAsset.text);

        // Display the initial text
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ContinueStory();
        }
    }

    void ContinueStory()
    {
        if (inkStory.canContinue)
        {
            string text = inkStory.Continue();
            UpdateUI();

            // Check for specific story points and repeat encounters
            if ((int)inkStory.variablesState["EncounteredBandits"] == 1 && inkStory.currentChoices.Count == 0)
            {
                RepeatEncounter();
            }
        }
    }

    void UpdateUI()
    {
        inkTextUI.text = inkStory.currentText;
    }

    void RepeatEncounter()
    {
        // Handle logic for repeating encounters
        Debug.Log("Repeating Bandit Encounter");
    }
}
