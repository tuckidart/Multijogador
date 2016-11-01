using UnityEngine;
using System.Collections;

public class CurveScript : MonoBehaviour {

	public bool isRight;
	public float multiplier;

	private int numberOfCarsThatTurned;

	void Start () 
	{
		numberOfCarsThatTurned = 0;
		RandMutiplier ();
	}

	public void IncreaseNumberOfCarsThatTurned ()
	{
		RandMutiplier ();
		numberOfCarsThatTurned++;
	}

	private void RandMutiplier ()
	{
		multiplier = Random.Range (0.0f, 1.0f);
	}
}
