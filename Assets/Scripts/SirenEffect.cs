using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SirenEffect : NetworkBehaviour {

	[SerializeField] Light redLight;
	[SerializeField] Light blueLight;

	private Vector3 redTemp;
	private Vector3 blueTemp;

	[SerializeField] int speed;

	[SyncVar]
	public bool inPursuit;

	public AudioSource sirenAudio;
	public AudioClip[] sirenType;
	public GameObject sirenObject;

	// Update is called once per frame
	void Update ()
	{
		if(inPursuit)
		{
			redTemp.y += speed * Time.deltaTime;
			blueTemp.y -= speed * Time.deltaTime;

			redLight.transform.eulerAngles = redTemp;
			blueLight.transform.eulerAngles = blueTemp;
		}
	}

	[Command]
	public void CmdToggleSiren(bool OnOff)
	{
		if(inPursuit && OnOff)
		{
			RpcSirenFast ();
		}
		else
		{
			inPursuit = OnOff;
			RpcToggleSiren (inPursuit);
		}
	}

	[ClientRpc]
	void RpcToggleSiren(bool OnOff)
	{
		inPursuit = OnOff;
		sirenAudio.clip = sirenType [0];

		if (inPursuit)
		{
			sirenAudio.Play ();
			sirenObject.SetActive (true);
		}
		else
		{
			sirenAudio.Stop ();
			sirenObject.SetActive (false);
		}
	}

	[ClientRpc]
	void RpcSirenFast()
	{
		sirenAudio.clip = sirenType [1];
		sirenAudio.Play ();
	}
}