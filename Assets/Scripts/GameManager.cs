using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject uiManager;
	private UIManager uiManagerInstance;
	public GameObject spawnable;
	public Queue<GameObject> levelGems = new Queue<GameObject>();

	public int levelCount = 0;
	public bool morethenOneGem = false;
	public Vector3 previousGem;
	public Vector3 firstGem;
	public bool animating = false;
	public Vector3 scaleFactor;

	private LevelDatas allLevels;

	/// <summary>
	/// Sets all the necessary variables on script initialization
	/// </summary>
	void Start()
	{
		uiManager = GameObject.Find("UIManager");
		uiManagerInstance = uiManager.GetComponent<UIManager>();

		allLevels = JsonUtility.FromJson<LevelDatas>(LoadJson().text);
		levelCount = allLevels.levels.Count;
	}

	/// <summary>
	/// Loads the json asset containing the levels' information
	/// </summary>
	public TextAsset LoadJson()
	{
		return Resources.Load<TextAsset>("LevelDatas/level_data");
	}


	/// <summary>
	/// Loads a level by it's index
	/// </summary>
	/// <param name="levelIndex">Index of the level</param>
	/// TO DO:
	/// Add logic for text addment so that it would always be visible
	public void LoadLevel(int levelIndex)
	{
		animating = false;
		firstGem = Vector3.zero;
		previousGem = Vector3.zero;
		morethenOneGem = false;
		uiManagerInstance.ButtonVisibility(false); //Sets all the buttons active=false
		Level currentLevel = allLevels.levels[levelIndex]; ;
		RectTransform canvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();

		scaleFactor = Vector3.one;
		float gemCount = currentLevel.level_data.Count;
		if (gemCount > 10)
		{
			scaleFactor = new Vector3(0.5f, 0.5f, 0.5f); //scales the gems if there are more gems - if the diagram is more complex
		}

		for (int i = 0; i < currentLevel.level_data.Count; i += 2)
		{
			float x = currentLevel.level_data[i];
			float y = 1000 - currentLevel.level_data[i + 1];
			Vector3 unityPosition = new Vector3((x - 500) * (Screen.width - 200) / 1000, (y - 500) * (Screen.height - 200) / 1000, -10);

			GameObject newSpawned = Instantiate(spawnable, unityPosition, Quaternion.identity);
			newSpawned.GetComponent<RectTransform>().SetParent(canvasTransform, false); //Setting the parent so that it would spawn in the right place
			newSpawned.GetComponent<RectTransform>().localScale = scaleFactor;

			levelGems.Enqueue(newSpawned);

			GameObject textObject = new GameObject("ChildText");
			textObject.AddComponent<RectTransform>();
			float width = 70; //two number width
			float height = 0;

			textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
			textObject.transform.position = textObject.transform.position + new Vector3(80, 0, 0);
			textObject.GetComponent<RectTransform>().SetParent(newSpawned.GetComponent<RectTransform>(), false);
			TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();

			textComponent.font = Resources.Load<TMP_FontAsset>("Fonts/Copyduck SDF");
			textComponent.fontSize = 48;
			textComponent.color = Color.black;
			textComponent.alignment = TextAlignmentOptions.Center;
			textComponent.alignment = TextAlignmentOptions.Midline;
			textObject.GetComponent<TextMeshProUGUI>().text = (i / 2 + 1).ToString();
		}
	}
}
