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
	private float completedTime;

	private PlayerSetup suspectSetup;
	private bool lateStartCalled;

	public GameObject RobberyPointPrefab;
	private GameObject temp;

	void Awake ()
	{
		moneyText.text ="$00" + moneyHundreds.ToString() + ",00";
	}

	void Start ()
	{
		barUI = GameObject.FindGameObjectWithTag ("RoberySlider").GetComponent<BarScript> ();
		barUI.maxValue = secondsToWait;
		barUI.updateAutomatically = false;

		Invoke ("LateStartFake", 1f);
	}

	void Update ()
	{
		if(hasEntered)
		{
//			CmdIncreaseMoney ();
			IncreaseMoney();
			barUI.SetCurrentValue(Time.time - EnteredRobberyTime + 0.25f + completedTime);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.name == "colbody1")
		{
			if (other.transform.parent.parent.gameObject.tag == "Suspect") 
			{
				if (other.transform.parent.parent.gameObject.GetComponent<NetworkIdentity> ().isLocalPlayer)
					barUI.TurnChildrenOnOff (true);

				hasEntered = true;
				EnteredRobberyTime = Time.time;
				Invoke ("EndRobbery", secondsToWait - completedTime);

				bl_MMItemInfo myPosition = new bl_MMItemInfo(transform.position);

				if (isServer)
				{
					CmdCreateRobberyPoint (myPosition);
				}
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.name == "colbody1")
		{
			if (other.transform.parent.parent.gameObject.tag == "Suspect")
			{
				if (other.transform.parent.parent.gameObject.GetComponent<NetworkIdentity> ().isLocalPlayer)
					barUI.TurnChildrenOnOff (false);
			
				hasEntered = false;
				CancelInvoke ();

				completedTime += Time.time - EnteredRobberyTime + 0.25f;

				if(temp != null)
					temp.GetComponent<bl_MiniMapItem>().RpcDestroyItem(true);

				if(isServer)
					RpcShowItem ();
			}
		}
	}

	[Command]
	void CmdCreateRobberyPoint(bl_MMItemInfo item)
	{
		RpcHideItem ();
		temp = Instantiate (RobberyPointPrefab, item.Position, Quaternion.identity) as GameObject;

		NetworkServer.Spawn (temp);
		Invoke ("DestroyPoint", secondsToWait - completedTime);
	}
	[ClientRpc]
	void  RpcShowItem()
	{
		GetComponent<bl_MiniMapItem> ().ShowItem();
	}
	[ClientRpc]
	void  RpcHideItem()
	{
		GetComponent<bl_MiniMapItem> ().HideItem();
	}
		
	private void EndRobbery ()
	{
		if (hasEntered == true)
		{
			barUI.TurnChildrenOnOff (false);
			suspectSetup.IncreaseMultiplier ();
			if(isServer)
				DestroyPoint ();
		}
	}		
	void DestroyPoint()
	{
		temp.GetComponent<bl_MiniMapItem>().RpcDestroyItem(true);
		GetComponent<bl_MiniMapItem>().RpcDestroyItem(true);
	}

	private void IncreaseMoney ()
	{
		//moneyHundreds++;
		suspectSetup.AddMoneyHundreds(1);
		if(suspectSetup.GetMoneyHundreds() > 999)
		{
			suspectSetup.AddMoneyThousands(1);
			suspectSetup.SetMoneyHundreds (0);
		}
		if(suspectSetup.GetMoneyThousands()> 0)
		{
			if(suspectSetup.GetMoneyHundreds() < 10)
			{
				moneyText.text ="$" +suspectSetup.GetMoneyThousands().ToString() + ".00" +suspectSetup.GetMoneyHundreds().ToString() + ",00";
			}
			else if(suspectSetup.GetMoneyHundreds() >= 10 &&  suspectSetup.GetMoneyHundreds() < 100)
				moneyText.text ="$" + suspectSetup.GetMoneyThousands().ToString() + ".0" +suspectSetup.GetMoneyHundreds().ToString() + ",00";
			else
				moneyText.text ="$" + suspectSetup.GetMoneyThousands().ToString() + "." + suspectSetup.GetMoneyHundreds().ToString() + ",00";
		}
		else
		{
			if(suspectSetup.GetMoneyHundreds() < 10)
			{
				moneyText.text ="$00" +suspectSetup.GetMoneyHundreds().ToString() + ",00";
			}
			else if(suspectSetup.GetMoneyHundreds() >= 10 &&  suspectSetup.GetMoneyHundreds() < 100)
				moneyText.text ="$0" +suspectSetup.GetMoneyHundreds().ToString() + ",00";
			else
				moneyText.text ="$" + suspectSetup.GetMoneyHundreds().ToString() + ",00";
		}
	}


	private void LateStartFake ()
	{
		suspectSetup = GameObject.FindGameObjectWithTag ("Suspect").GetComponent<PlayerSetup> ();
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