using UnityEngine;
using System.Collections;

public class ObjectivesController : MonoBehaviour {

	public bool fugitiveWon;
	public bool copWon;

	public void SetCopWon ()
	{
		copWon = true;
	}

	public void SetFujitiveWon ()
	{
		fugitiveWon = true;
		DoTimerOverActions ();
	}

	public void DoTimerOverActions ()
	{
		//Fugitive got away successfuly
		if (fugitiveWon) 
		{
			DoFugitiveWinActions ();
		} 
		//Cop captured fugitive
		else if (copWon) 
		{
			DoCopWinActions ();
		}
		//Time is over and fugitive couldnt get away
		else 
		{
			copWon = true;
			DoCopWinActions ();
		}
	}

	private void DoCopWinActions ()
	{
		Debug.Log ("Cop won");
	}

	private void DoFugitiveWinActions ()
	{
		Debug.Log ("Fugitive won");
	}
}