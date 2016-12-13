using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RobberyScript : NetworkBehaviour {

	private int moneyThousands;
	private int moneyHundreds;
	public Text moneyText;

	public float secondsToWait;
	public BarScript barUI;

	private bool hasEntered;

	private float EnteredRobberyTime;

	void Awake ()
	{
		barUI = GameObject.FindGameObjectWithTag ("ScapeSlider").GetComponent<BarScript> ();
		barUI.maxValue = secondsToWait;
		barUI.updateAutomatically = false;

		moneyText.text ="$00" + moneyHundreds.ToString() + ",00";
	}

	void Update ()
	{
		if(hasEntered)
		{
//			CmdIncreaseMoney ();
			IncreaseMoney();
			barUI.SetCurrentValue(Time.time - EnteredRobberyTime + 0.25f);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.name == "colbody1")
			if (other.transform.parent.parent.gameObject.tag == "Suspect") 
			{
				hasEntered = true;
				barUI.TurnChildrenOnOff (true);

				EnteredRobberyTime = Time.time;
				Invoke ("EndRobbery", secondsToWait);
			}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.name == "colbody1")
			if (other.transform.parent.parent.gameObject.tag == "Suspect") 
			{
				hasEntered = false;
				barUI.TurnChildrenOnOff (false);
				CancelInvoke ();
			}
	}
		
	private void EndRobbery ()
	{
		if (hasEntered == true)
		{
			barUI.TurnChildrenOnOff (false);
			if(isServer)
				DestroyPoint ();
		}
	}		
	void DestroyPoint()
	{
		GetComponent<bl_MiniMapItem>().RpcDestroyItem(true);
	}

	private void IncreaseMoney ()
	{
		moneyHundreds++;
		if(moneyHundreds > 999)
		{
			moneyThousands++;
			moneyHundreds = 0;
		}
		if(moneyThousands > 0)
		{
			if(moneyHundreds < 10)
			{
				moneyText.text ="$" + moneyThousands.ToString() + ".00" + moneyHundreds.ToString() + ",00";
			}
			else if(moneyHundreds >= 10 && moneyHundreds < 100)
				moneyText.text ="$" + moneyThousands.ToString() + ".0" + moneyHundreds.ToString() + ",00";
			else
				moneyText.text ="$" + moneyThousands.ToString() + "." + moneyHundreds.ToString() + ",00";
		}
		else
		{
			if(moneyHundreds < 10)
			{
				moneyText.text ="$00" + moneyHundreds.ToString() + ",00";
			}
			else if(moneyHundreds >= 10 && moneyHundreds < 100)
				moneyText.text ="$0" + moneyHundreds.ToString() + ",00";
			else
				moneyText.text ="$" + moneyHundreds.ToString() + ",00";
		}
	}

//	[Command]
//	private void CmdIncreaseMoney ()
//	{
//		RpcIncreaseMoney ();
//	}
//
//	[ClientRpc]
//	private void RpcIncreaseMoney ()
//	{
//		moneyHundreds++;
//		if(moneyHundreds > 999)
//		{
//			moneyThousands++;
//			moneyHundreds = 0;
//		}
//		if(moneyThousands > 0)
//		{
//			if(moneyHundreds < 10)
//			{
//				moneyText.text ="$" + moneyThousands.ToString() + ".00" + moneyHundreds.ToString() + ",00";
//			}
//			else if(moneyHundreds >= 10 && moneyHundreds < 100)
//				moneyText.text ="$" + moneyThousands.ToString() + ".0" + moneyHundreds.ToString() + ",00";
//			else
//				moneyText.text ="$" + moneyThousands.ToString() + "." + moneyHundreds.ToString() + ",00";
//		}
//		else
//		{
//			if(moneyHundreds < 10)
//			{
//				moneyText.text ="$00" + moneyHundreds.ToString() + ",00";
//			}
//			else if(moneyHundreds >= 10 && moneyHundreds < 100)
//				moneyText.text ="$0" + moneyHundreds.ToString() + ",00";
//			else
//				moneyText.text ="$" + moneyHundreds.ToString() + ",00";
//		}
//	}
}