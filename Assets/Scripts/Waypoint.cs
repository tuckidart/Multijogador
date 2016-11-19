using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {

	public List<Transform> waypoints;
	public bool isRight;

	public Transform GetRandomWaypoint ()
	{
		int index = Random.Range (0, waypoints.Count);
		if (index == waypoints.Count-1)
			isRight = true;
		else
			isRight = false;
		
		return waypoints[index];
	}
}