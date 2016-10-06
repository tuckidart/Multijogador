using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectivesController : MonoBehaviour {

	public bool fugitiveWon;
	public bool copWon;

	public Text endGameText;

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
		//Time is over and fugitive couldn't get away
		else
		{
			copWon = true;
			DoCopWinActions ();
		}
	}

	public void CallCarDied (bool isCop)
	{
		if (!isCop)
			DoCopWinActions ();
		else
			DoFugitiveWinActions ();
	}

	public void CallSupectScaped ()
	{
		if (fugitiveWon == false) 
		{
			DoFugitiveWinActions ();
		}
	}

	private void DoCopWinActions ()
	{
		fugitiveWon = false;
		copWon = true;

		endGameText.transform.parent.gameObject.SetActive (true);
		endGameText.text = "Cop won";

		Time.timeScale = 0;
	}

	private void DoFugitiveWinActions ()
	{
		fugitiveWon = true;
		copWon = false;

		endGameText.transform.parent.gameObject.SetActive (true);
		endGameText.text = "Fugitive won";

		Time.timeScale = 0;
	}
}