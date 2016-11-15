using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State : MonoBehaviour {

	public bool button;
	private bool isRight;

	//=================STATES
	private bool turnedOn;				//Level 1
	//Level 1
	private bool moving;				//Level 2

	//Level 2
	private float acceleration;			// 1 | -1
	private float turning;				// 1 | -1

	private bool parked;				//Used on initialize
	//=================STATES


	//=================SENSORS
	private bool hasObstacle;
	public float obstacleDistance;
	public float curveDistance;

	public Transform[] obstacles;

	public ObstacleType[] currentObstacleType;
	//=================SENSORS

	private bool isTurning;
	private float initialYrot;
	private bool completedTurning;
	private Transform curve;

	private vehicleController controller;

	void Awake () 
	{
		obstacles = new Transform[2];
		currentObstacleType = new ObstacleType[2];
		controller = GetComponent<vehicleController> ();
		turnedOn = false;
		moving = false;
	}
	
	void Update () 
	{
		if (hasObstacle)
			CalculateObstacleDistance ();

		//Apply values to the car
		ApplyValues ();
	}

	//public void ReceiveCar ()


	public void ReceiveObstacle (Transform newObstacle, ObstacleType newObstacleType)
	{
		for(int i=0;i<obstacles.Length;i++)
		{
			if (obstacles [i] == null)
			{
				obstacles[i] = newObstacle;
				currentObstacleType[i] = newObstacleType;
				hasObstacle = true;
				HandleObstacle ();
				return;
			}
		}
	}

	public void RemoveObstacle (Transform obstacleToBeRemoved)
	{
		for(int i=0;i<obstacles.Length;i++)
		{
			if (obstacles [i].transform.name == obstacleToBeRemoved.name)
			{
				obstacles [i] = null;
				currentObstacleType [i] = ObstacleType.NULL;
				if (obstacles [0] == null && obstacles [1] == null)
					hasObstacle = false;
				return;
			}
		}
	}
		
	void ApplyValues ()
	{
		if (obstacles[0] == null && obstacles[1] == null)
		{
			controller.inputY += 0.01f;
		}
		else
		{
			for (int i = 0; i < currentObstacleType.Length; i++)
			{
				if (currentObstacleType[i] == ObstacleType.car || currentObstacleType[i] == ObstacleType.light)
				{
					if (controller.zVel > 0)
						controller.inputY -= 1.0f / obstacleDistance;
					else
						controller.inputY = 0.0f;
				}
				if (currentObstacleType[i] == ObstacleType.curve && button)
				{
					if (controller.zVel > 0)
						controller.inputY -= 1.0f / curveDistance;
					if (controller.inputY <= 0.3f)
						controller.inputY = 0.3f;
				}
			}
		}

		if (button && curveDistance < 5.0f)
		{
			if (isTurning == false) 
			{
				initialYrot = transform.localRotation.eulerAngles.y;
				isTurning = true;
			}

			switch(isRight)
			{
			case true:
				if (transform.localRotation.eulerAngles.y > initialYrot + 87)
				{
					controller.inputX = 0.0f;
					button = false;
					isTurning = false;
					transform.localRotation = Quaternion.Euler (transform.localRotation.eulerAngles.x, initialYrot + 90, transform.localRotation.eulerAngles.z);
				} 
				else 
				{
					controller.inputX += 0.1f;
				}
				break;

			case false:
				if (transform.localRotation.eulerAngles.y < initialYrot - 87)
				{
					controller.inputX = 0.0f;
					button = false;
					isTurning = false;
					transform.localRotation = Quaternion.Euler (transform.localRotation.eulerAngles.x, initialYrot - 90, transform.localRotation.eulerAngles.z);
				}
				else 
				{
					controller.inputX -= 0.1f;
				}
				break;
			}
		}
	}

	private void CalculateObstacleDistance ()
	{
		for (int i = 0; i < currentObstacleType.Length; i++)
		{
			if (currentObstacleType [i] != ObstacleType.curve && currentObstacleType [i] != ObstacleType.NULL)
			{
				obstacleDistance = Vector3.Distance (transform.position, obstacles [i].position);
			}
			else if (currentObstacleType [i] == ObstacleType.curve)
			{
				curveDistance = Vector3.Distance (transform.position, obstacles [i].position);
			}
		}
	}

	private void HandleObstacle ()
	{
		for (int i = 0; i < currentObstacleType.Length; i++)
		{
			if (currentObstacleType[i] == ObstacleType.curve)
			{
				//if (Random.Range(0, 2) == 1)
				//{
				if (obstacles[i].GetComponent<CurveScript> ().isRight)
				{
					//Set turning to +1
					//Compute obstacle multiplier
					button = true;
					isRight = true;
					Debug.Log ("Curve - Turn right");
				} 
				else
				{
					//Set turning to -1
					//Compute obstacle multiplier
					button = true;
					isRight = false;
					Debug.Log ("Curve - Turn left");
				}

				obstacles[i].GetComponent<CurveScript> ().IncreaseNumberOfCarsThatTurned ();
				//}
			}
			else if (currentObstacleType[i] == ObstacleType.light && obstacles[i].GetComponent<LightScript> ().isGreen == false)
			{
				//Fucking stop
				Debug.Log ("RED Light - Fucking stop");
			}
			else
			{
				//Fucking stop? Maybe a little slower
				Debug.Log ("Car - Fucking stop?");
			}
		}
	}
}