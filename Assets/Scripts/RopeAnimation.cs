using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class RopeAnimation : MonoBehaviour
{
	public Vector3 startPoint;
	public Vector3 endPoint;
	public float appearanceTime = 1f;

	private LineRenderer lineRenderer;

	/// <summary>
	/// Trigers the whole line appearance animation
	/// </summary>
	public void TriggerLineConnection()
	{
		GameObject.Find("GameManager").GetComponent<GameManager>().animating = true;
		Initiation();
		StartCoroutine(AnimateLineAppearance());
	}

	/// <summary>
	/// IEnumerator for the line animation
	/// </summary>
	/// <returns>Smooth animation</returns>
	IEnumerator AnimateLineAppearance()
	{
		float elapsedTime = 0f;

		while (elapsedTime < appearanceTime)
		{
			float t = elapsedTime / appearanceTime;
			Vector3 lerpedPosition = Vector3.Lerp(startPoint, endPoint, t);

			lineRenderer.positionCount = 2;
			lineRenderer.SetPosition(0, startPoint);
			lineRenderer.SetPosition(1, lerpedPosition);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		lineRenderer.SetPosition(1, endPoint);
		GameObject.Find("GameManager").GetComponent<GameManager>().animating = false;
	}

	/// <summary>
	/// Initiates the basic variables needed for the animation
	/// </summary>
	private void Initiation()
	{
		lineRenderer = this.GetComponent<LineRenderer>();
		float scaleFactor = GameObject.Find("GameManager").GetComponent<GameManager>().scaleFactor.x;
		lineRenderer.startWidth = 3f * scaleFactor;
		lineRenderer.endWidth = 3f * scaleFactor;

		lineRenderer.positionCount = 2;

		startPoint = new Vector3(startPoint.x, startPoint.y, 89);
		endPoint = new Vector3(endPoint.x, endPoint.y, 89);

		//Obscure time scale bug workaround
		Time.timeScale = 1f;
	}
}


