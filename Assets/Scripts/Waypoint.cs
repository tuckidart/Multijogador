using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {

	public List<Transform> waypoints;
	public Transform wrongWay;

	private Transform AiCar;

	public Transform GetRandomWaypoint (Transform car, Transform cantGoToThisWaypoint)
	{
		AiCar = car;
		int index = Random.Range (0, waypoints.Count);
		do 
		{
			index = Random.Range (0, waypoints.Count);
			Debug.Log (index);
		}
		while(waypoints [index] == cantGoToThisWaypoint);

		Debug.Log ("VOU PARA O WAYPOINT: " + waypoints [index].name);

		if(index != 0 || waypoints.Count == 1)
			WrongWay ();
		
		return waypoints[index];
	}

	void WrongWay()
	{
		AiCar.GetComponent<State>().cantGoToThisWaypoint = wrongWay;
	}
}