using UnityEngine;
using System.Collections;

public class Open_Door : MonoBehaviour {

	private Vector3 originalPosition;
	public Transform door;
	public Transform doorTarget;
	public float speedPos;
	public float speedRot;

	public bool openDoor;
	public bool closeDoor;

	// Use this for initialization
	void Start () {
		originalPosition = door.position;
		openDoor = false;
		closeDoor = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (openDoor)
		{
			door.position = Vector3.Lerp (door.position, doorTarget.position, speedPos * Time.deltaTime);
			door.rotation = Quaternion.Lerp (door.rotation, doorTarget.rotation, speedRot * Time.deltaTime);
			if(door.position == doorTarget.position)
			{
				openDoor = false;
			}
		}
		if (closeDoor)
		{
			door.position = Vector3.Lerp (door.position, originalPosition, speedPos * Time.deltaTime);
			door.rotation = Quaternion.Lerp (door.rotation, Quaternion.identity, speedRot * Time.deltaTime);
			if (door.position == originalPosition)
			{
				closeDoor = false;
			}
		}
	}

	void OnTriggerEnter()
	{
		//aqui fazer uma animação da porta
		openDoor = true;
		closeDoor = false;
	}

	void OnTriggerExit()
	{
		closeDoor = true;
		openDoor = false;
	}
}
