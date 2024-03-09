using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GemInteract : MonoBehaviour, IPointerClickHandler
{
	public GameObject blueGem;
	private GameObject gameManager;
	private GameManager gameManagerInstance;
	private GameObject uiManager;
	private UIManager uiManagerInstance;

	/// <summary>
	/// Sets all the necessary variables on script initialization
	/// </summary>
	void Start()
	{
		gameManager = GameObject.Find("GameManager");
		gameManagerInstance = gameManager.GetComponent<GameManager>();
		uiManager = GameObject.Find("UIManager");
		uiManagerInstance = uiManager.GetComponent<UIManager>();
	}

	/// <summary>
	/// Handles gem interaction
	/// </summary>
	/// <param name="eventData">Pointer's event Data</param>
	public void OnPointerClick(PointerEventData eventData)
	{
		bool gamePaused = uiManagerInstance.paused;
		Queue<GameObject> queue = gameManagerInstance.levelGems;
		Vector3 previousGem = gameManagerInstance.previousGem;

		if (!gamePaused && queue.Peek() == eventData.pointerPress.gameObject && !gameManagerInstance.animating)
		{
			uiManagerInstance.FadeOut(transform.GetChild(0).GetComponent<TextMeshProUGUI>(), 1f);
			Vector3 position = eventData.pointerCurrentRaycast.gameObject.transform.position;

			if (previousGem == Vector3.zero)
			{
				gameManagerInstance.firstGem = position;
				gameManagerInstance.previousGem = position;
				Instantiate(blueGem, transform);
			}
			if (!gameManagerInstance.morethenOneGem)
			{
				gameManager.GetComponent<GameManager>().morethenOneGem = true;
			}
			else
			{
				GameObject line = DrawLine(previousGem, position);
				//Final line drawing logic
				if (queue.Count == 1 && previousGem != gameManagerInstance.firstGem)
				{
					StartCoroutine(WaitToDrawLine(line.GetComponent<RopeAnimation>().appearanceTime, position, gameManagerInstance.firstGem));
				}
			}
			gameManager.GetComponent<GameManager>().previousGem = position;
			queue.Dequeue();
			if (queue.Count == 0)
			{
				uiManager.GetComponent<UIManager>().GameWon();
			}
		}
		else if (gamePaused)
		{
			Debug.Log("The Game is paused");
		}
		else
		{
			Debug.Log("Not the correct order");
		}
	}

	/// <summary>
	/// Creates a GameObject with the LineRenderer component and initializes the line's animation to the given points
	/// </summary>
	/// <param name="startP">Start point</param>
	/// <param name="endP">End point</param>
	/// <returns>GameObject with the LineRenderer component</returns>
	private GameObject DrawLine(Vector3 startP, Vector3 endP)
	{
		GameObject ropeGOLAST = Instantiate(Resources.Load<GameObject>("Prefabs/RopePrefab"));
		ropeGOLAST.transform.SetParent(GameObject.Find("CanvasWS").GetComponent<RectTransform>(), false);
		ropeGOLAST.tag = "Rope";
		ropeGOLAST.SetActive(false);

		ropeGOLAST.SetActive(true);
		RopeAnimation ropeanimationLAST = ropeGOLAST.GetComponent<RopeAnimation>();
		ropeanimationLAST.startPoint = startP;
		ropeanimationLAST.endPoint = endP;
		ropeanimationLAST.TriggerLineConnection();
		TurnGemBlue(transform, ropeanimationLAST.appearanceTime);
		return ropeGOLAST;
	}

	/// <summary>
	/// Waits to draw a line
	/// </summary>
	/// <param name="waitTime">Wait duration</param>
	/// <param name="startP">Start point</param>
	/// <param name="endP">End point</param>
	/// <returns>GameObject with the LineRenderer component after the period</returns>
	IEnumerator WaitToDrawLine(float waitTime, Vector3 startP, Vector3 endP)
	{
		yield return new WaitForSeconds(waitTime);
		DrawLine(startP, endP);
	}

	/// <summary>
	/// Turns the gem blue and fades out the text child object with an animation
	/// </summary>
	/// <param name="position">Position where to spawn the blue gem</param>
	/// <param name="fadeDuration">Animation duration</param>
	public void TurnGemBlue(Transform position, float fadeDuration)
	{
		StartCoroutine(TurnGemBlueIE(position, fadeDuration));
	}

	/// <summary>
	/// Spawns the text fade instantly and instantiates a blue gem after a duration
	/// </summary>
	/// <param name="position">Position where to spawn the blue gem</param>
	/// <param name="fadeDuration">Animation duration</param>
	/// <returns>Smooth animation</returns>
	IEnumerator TurnGemBlueIE(Transform position, float fadeDuration)
	{
		uiManagerInstance.FadeOut(transform.GetChild(0).GetComponent<TextMeshProUGUI>(), fadeDuration);
		yield return new WaitForSeconds(fadeDuration - 0.1f); //-0.1 because the line connects to the center of the gem not the outer position
		Instantiate(blueGem, position); //"Makes" the gem turn blue
	}
}
