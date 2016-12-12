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
		if(inPursuit == true && OnOff == true)
		{
			RpcSirenFast ();
		}
		else
		{
			inPursuit = OnOff;
			sirenAudio.clip = sirenType [0];
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
			sirenObject.SetActive (true);
			sirenAudio.Play ();
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
		if(sirenAudio.clip == sirenType [0])
			sirenAudio.clip = sirenType [1];
		else if(sirenAudio.clip == sirenType [1])
			sirenAudio.clip = sirenType [0];
		
		sirenAudio.Play ();
	}
}