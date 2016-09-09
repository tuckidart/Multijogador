using UnityEngine;
using System.Collections;

public class _GameMaster : MonoBehaviour {

	public static _GameMaster GM;

	public TimeControl myTimeControl;
	public ObjectivesController myObjectivesController;

	void Awake () 
	{
		if(!GM) 
		{
			GM = this;
			DontDestroyOnLoad(gameObject);
		}
		else 
			Destroy(gameObject);
	}

	void Start ()
	{
		myTimeControl = gameObject.GetComponent<TimeControl> ();
		myObjectivesController = gameObject.GetComponent<ObjectivesController> ();
	}
}
