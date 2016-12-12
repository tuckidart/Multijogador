using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarScript : MonoBehaviour {

	public Slider mySlider;
	public bool updateAutomatically;
	public float maxValue;
	public float currentValue;

	void Update ()
	{
		if (updateAutomatically) 
		{
			mySlider.value = currentValue / maxValue;	
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
