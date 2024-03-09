using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public GameObject levelButton;
	public GameObject panelPaused;

	public GameObject gameManager;
	private GameManager gameManagerInstance;

	public bool paused = false;
	public bool buttonsVisible = false;
	private bool buttonsCreated = false;

	private List<GameObject> levelButtonList;


	/// <summary>
	/// Sets all the necessary variables on script initialization
	/// </summary>
	void Start()
	{
		levelButtonList = new List<GameObject>();
		gameManager = GameObject.Find("GameManager");
		if (gameManager != null)
		{
			gameManagerInstance = gameManager.GetComponent<GameManager>();
		}
		else Debug.LogWarning("No GameManager found");
		panelPaused.SetActive(false);
	}

	/// <summary>
	/// Checks if the game is paused every frame
	/// </summary>
	void Update()
	{
		PauseGame();
	}

	/// <summary>
	/// Pauses the game
	/// </summary>
	public void PauseGame()
	{
		if (!paused && Input.GetButtonDown("Cancel") && !buttonsVisible)
		{
			paused = true;
			Time.timeScale = 0f;
			panelPaused.SetActive(true);
			panelPaused.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "GAME PAUSED";
			panelPaused.transform.GetChild(2).gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Resume the game
	/// </summary>
	public void ResumeGame()
	{
		if (paused)
		{
			paused = false;
			Time.timeScale = 1f;
			panelPaused.SetActive(false);
		}
	}

	/// <summary>
	/// Controls the UI buttons' visibility
	/// </summary>
	/// <param name="visible">True if you need to set them active or false if you want to disable them</param>
	public void ButtonVisibility(bool visible)
	{
		bool foundButton = true;
		while (foundButton)
		{
			GameObject buttonObject = GameObject.FindObjectsOfType<GameObject>()
			.FirstOrDefault(go => go.name.Contains("Button") || go.name.Contains("Button ("));

			if (buttonObject != null)
			{
				buttonObject.SetActive(visible);
				foundButton = true;
			}
			else foundButton = false;
		}
		buttonsVisible = visible;
	}

	/// <summary>
	/// Makes all button for different levels visible
	/// </summary>
	public void LevelButtonVisibility()
	{
		for (int i = 0; levelButtonList.Count > i; i++)
		{
			levelButtonList[i].SetActive(true);
		}
		buttonsVisible = true;
	}

	/// <summary>
	/// Removes all gems from the scene
	/// </summary>
	public void RemoveAllObjectsWithTag(string tag)
	{
		GameObject[] objectsToRemove = GameObject.FindGameObjectsWithTag(tag);
		foreach (GameObject obj in objectsToRemove)
		{
			Destroy(obj);
		}
		if (tag == "Gem" && gameManagerInstance.levelGems != null) gameManagerInstance.levelGems.Clear();
	}

	/// <summary>
	/// Loads the level selection buttons pased on the amount of levels
	/// in level data json file
	/// </summary>
	public void LoadLevelUI()
	{
		RemoveAllObjectsWithTag("Rope");
		if (panelPaused.activeSelf == true)
		{
			RemoveAllObjectsWithTag("Gem");
			panelPaused.SetActive(false);
			paused = false;
		}
		//If there are no created level buttons, it creates them
		if (!buttonsCreated)
		{
			ButtonVisibility(false);
			float uiSpawnX;
			float uiSpawnY = 150;
			for (int i = 0; i < gameManagerInstance.levelCount; i++)
			{
				if (i == 0)
				{
					uiSpawnX = -400;
				}
				else if (i % 2 == 0)
				{
					uiSpawnX = -400;
					uiSpawnY -= 200;
				}
				else uiSpawnX = 400;

				GameObject newButton = Instantiate(levelButton, Vector3.zero, Quaternion.identity);
				int levelIndex = i; //creating a local variable to use inside a lambda expression for the delagate 
				newButton.GetComponent<Button>().onClick.AddListener(() => gameManagerInstance.LoadLevel(levelIndex));

				RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
				buttonRectTransform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>(), false);
				buttonRectTransform.anchoredPosition = new Vector2(uiSpawnX, uiSpawnY);

				TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
				buttonText.text = "Level " + (i + 1).ToString();
				buttonsVisible = true;
				buttonsCreated = true;

				levelButtonList.Add(newButton);
			}
		}
		else LevelButtonVisibility(); //if there exists created button it shows them
	}

	/// <summary>
	/// Exits the game
	/// </summary>
	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	/// <summary>
	/// Enables the pause panel and tweaks it so it would fit
	/// </summary>
	public void GameWon()
	{
		paused = true;
		panelPaused.SetActive(true);
		panelPaused.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "VICTORY!";
		panelPaused.transform.GetChild(2).gameObject.SetActive(false);
	}

	/// <summary>
	/// Fades out the gem's number
	/// </summary>
	public void FadeOut(TextMeshProUGUI text, float fadeDuration)
	{
		StartCoroutine(FadeOutCoroutine(text, fadeDuration));
	}

	/// <summary>
	/// Coroutine to gradually fade the text number
	/// </summary>
	/// <returns></returns>
	IEnumerator FadeOutCoroutine(TextMeshProUGUI text, float fadeDuration)
	{
		float elapsedTime = 0f;
		float startAlpha = text.color.a;

		while (elapsedTime < fadeDuration)
		{
			Color newColor = text.color;
			newColor.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
			text.color = newColor;
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		Color finalColor = text.color;
		finalColor.a = 0f;
		text.color = finalColor;
	}
}
