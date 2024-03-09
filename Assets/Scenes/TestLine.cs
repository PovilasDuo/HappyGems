using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    public Transform pos1;
    public Transform pos2;
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();  
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, pos1.position);
		lineRenderer.SetPosition(1, pos2.position);
	}
}
