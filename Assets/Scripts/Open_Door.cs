using UnityEngine;
using System.Collections;

public class Open_Door : MonoBehaviour {

	private Vector3 originalPosition;
	public Transform door;
	public Transform doorTarget;
	public float speed;

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
			door.position = Vector3.Lerp (door.position, doorTarget.position, speed * Time.deltaTime);
			door.rotation = Quaternion.Lerp (door.rotation, doorTarget.rotation, speed * Time.deltaTime);
			if(door.position == doorTarget.position)
			{
				openDoor = false;
			}
		}
		if (closeDoor)
		{
			door.position = Vector3.Lerp (door.position, originalPosition, speed * Time.deltaTime);
			door.rotation = Quaternion.Lerp (door.rotation, Quaternion.identity, speed * Time.deltaTime);
			if (door.position == originalPosition)
			{
				closeDoor = false;
			}
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		openDoor = true;
		closeDoor = false;
	}

	void OnTriggerExit(Collider other)
	{
		closeDoor = true;
		openDoor = false;
	}
}
