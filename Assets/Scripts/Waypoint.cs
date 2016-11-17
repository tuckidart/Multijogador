using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {

	public List<Transform> waypoints;

	public Transform GetRandomWaypoint ()
	{
		int index = Random.Range (0, waypoints.Count);

		return waypoints[index];
	}
}
