using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarScript : MonoBehaviour {

	public Slider mySlider;
	public bool updateAutomatically;
	public float maxValue;
	public float currentValue;

	void Start ()
	{
		TurnChildrenOnOff (false);
	}

	void Update ()
	{
		if (updateAutomatically) 
		{
			mySlider.value = currentValue / maxValue;	
		}
	}

	public void TurnChildrenOnOff (bool newStatus)
	{
		for (int i = 0; i < transform.childCount; i++) 
		{
			transform.GetChild (i).gameObject.SetActive (newStatus);
		}
	}

	public void SetCurrentValue (float newValue)
	{
		currentValue = newValue;

		AdjustValues ();
	}

	public void AddToCurrentValue (float addValue)
	{
		currentValue += addValue;

		AdjustValues ();
	}

	private void AdjustValues ()
	{
		if (currentValue > maxValue)
			currentValue = maxValue;
		else if (currentValue <= 0)
			currentValue = 0;

		mySlider.value = currentValue / maxValue;	
	}
}