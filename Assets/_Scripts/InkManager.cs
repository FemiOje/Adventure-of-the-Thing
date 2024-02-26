using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using System;
using System.Collections.Generic;

public class InkManager : MonoBehaviour
{

	private int textCount = -7;
	[SerializeField] HealthBar playerHealthBar;
	[SerializeField] HeroKnight player;
	Image image;
	public static event Action<Story> OnCreateStory;

	void Awake()
	{
		// Remove the default messag
		Time.timeScale = 0.0f;
		RemoveChildren();
		StartStory();
	}
	private void OnEnable() {
		image = canvas.GetComponent<Image>();
		image.enabled = true;
	}

	private void OnDisable() {
		image.enabled = false;
	}

	// Creates a new Story object with the compiled story which we can then play!
	public void StartStory()
	{
		story = new Story(inkJSONAsset.text);
		if (OnCreateStory != null) OnCreateStory(story);
		RefreshView();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView()
	{
		// Remove all the UI on screen
		RemoveChildren();

		// Read all the content until we can't continue any more
		while (story.canContinue)
		{
			// Continue gets the next line of the story
			string text = story.Continue();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			CreateContentView(text);
		}

		// Display all the choices, if there are any!
		if (story.currentChoices.Count > 0)
		{
			for (int i = 0; i < story.currentChoices.Count; i++)
			{
				Choice choice = story.currentChoices[i];
				Button button = CreateChoiceView(choice.text.Trim());
				button.onClick.AddListener(delegate
				{
					OnClickChoiceButton(choice);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else
		{
			Time.timeScale = 1.0f;
			RemoveChildren();
			Destroy(gameObject);
		}
	}

	void OnClickChoiceButton(Choice choice)
	{
		if (choice.text == "ContinueOnPath")
		{
			RemoveBandits();
		}

		story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}

void CreateContentView(string text)
{
    Text storyText = Instantiate(textPrefab) as Text;
    storyText.text = text;

    RectTransform rt = storyText.GetComponent<RectTransform>();
    rt.SetParent(canvas.transform, false);

    float yOffset = -50 * textCount;
    rt.anchoredPosition = new Vector2(0, yOffset);
    storyText.color = Color.white;

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
		float yOffset = -50 * textCount;  // Adjust the vertical spacing (e.g., -30 pixels)
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
}
