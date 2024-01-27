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

<<<<<<< Updated upstream
    void RepeatEncounter()
    {
        // Handle logic for repeating encounters
        Debug.Log("Repeating Bandit Encounter");
    }
=======
	void OnClickChoiceButton(Choice choice)
	{
		if (choice.text == "ContinueOnPath")
		{
			DeductPlayerHealth();
			RemoveBandits();
		}

		story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}

	void DeductPlayerHealth()
	{
		player.health -= 40;
		playerHealthBar.SetHealth(player.health - 20);
	}
void CreateContentView(string text)
{
    Text storyText = Instantiate(textPrefab) as Text;
    storyText.text = text;

    RectTransform rt = storyText.GetComponent<RectTransform>();
    rt.SetParent(canvas.transform, false);

    float yOffset = -50 * textCount;
    rt.anchoredPosition = new Vector2(0, yOffset);
    rt.sizeDelta = new Vector2(400, rt.sizeDelta.y);
    storyText.color = Color.black;

    textCount++;
}



	// Creates a button showing the choice text
	Button CreateChoiceView(string text)
	{
		// Creates the button from a prefab
		Button choice = Instantiate(buttonPrefab) as Button;
		choice.transform.SetParent(canvas.transform, false);

		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text>();
		choiceText.text = text;

		// Make the button expand to fit the text
		HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
		layoutGroup.childForceExpandHeight = false;

		// Set the y-coordinate to create space between buttons
		float yOffset = -30 * textCount;  // Adjust the vertical spacing (e.g., -30 pixels)
		RectTransform rt = choice.GetComponent<RectTransform>();
		rt.anchoredPosition = new Vector2(0, yOffset);

		// Increase textCount for the next text element
		textCount++;

		return choice;
	}


	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren()
	{
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i)
		{
			Destroy(canvas.transform.GetChild(i).gameObject);
		}
	}

	void RemoveBandits()
	{
		// Find all Bandit GameObjects in the scene
		Bandit[] allBandits = FindObjectsOfType<Bandit>();

		// Sort the bandits based on their distance to the player
		List<Bandit> sortedBandits = new List<Bandit>(allBandits);
		sortedBandits.Sort((a, b) => Vector3.Distance(player.transform.position, a.transform.position)
									  .CompareTo(Vector3.Distance(player.transform.position, b.transform.position)));

		// Destroy the three closest bandits
		int banditsToRemoveCount = Mathf.Min(3, sortedBandits.Count);
		for (int i = 0; i < banditsToRemoveCount; i++)
		{
			Destroy(sortedBandits[i].gameObject);
		}
	}


	[SerializeField]
	private TextAsset inkJSONAsset = null;
	public Story story;

	[SerializeField]
	private Canvas canvas = null;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab = null;
	[SerializeField]
	private Button buttonPrefab = null;
>>>>>>> Stashed changes
}
