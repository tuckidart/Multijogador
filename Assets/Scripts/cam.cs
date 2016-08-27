using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class cam : MonoBehaviour {
	
	[Header("Camera Target")]
	
	[Tooltip("Drag your vehicle here. This is the GameObject with the VehicleController script attached")]
	public GameObject carObj;
	
	[Header("Camera Settings")]
	
	[Tooltip("By default you look at the vehicle centre, change the X values to look more to the sides, Y to look above or below and Z to look infront of or behind your vehicle")]
	[ContextMenuItem("Reset 'Look At Offset' to default", "resetLookAtOffset")]
	public Vector3 lookAtOffset = new Vector3(0f,0f,0f);
	[Tooltip("This will move the camera up/down, left or right, back or forward all the while aiming at the LookAt position.")]
	[ContextMenuItem("Reset 'Follow Position' to default", "resetFollowPosition")]
	public Vector3 followPosition = new Vector3(0f,2f,-5f);
	[Tooltip("When checked the camera will follow behind the vehicle (like a proper racing game). When unchecked the camera will not follow behind (best for isometric or top down camera)")]
	public bool useCarRotation = true;
	[Tooltip("When checked the camera will tilt with the car as it drives up the side of walls and through loops")]
	public bool tiltWithSlopes = false;
	[Tooltip("How much the camera lags/drags behind the vehicle when it accelerates (this increases the sense of speed). 0 means the camera will not follow, 100 means the camera sticks like glue to the vehicle")]
	[Range(0f,100f)]
	[ContextMenuItem("Reset 'Follow Speed' to default", "resetFollowSpeed")]
	public float followSpeed = 5f;
	[Tooltip("how much the camera distorts perspective as the vehicle accelerates. When the car slows the distorition lessens")]
	[Range(0f,3f)]
	[ContextMenuItem("Reset 'Field Of View Effect' to default", "resetFieldOfViewEffect")]
	public float FieldOfViewEffect = 0f;
	
	
	[Header("Mobile Input")]
	#if MOBILE_INPUT
	[Tooltip("speed to zoom camera in/out when pinching the screen with two fingers")]
	public float fingerPinchSpeed = 0.5f;
	#else
	[TextArea(3,10)]
	public string Note = "for mobile options the MobileInput prefab and Canvas prefab must exist in the scene, build setting must be for a mobile device, then go to the drop down 'Mobile Input' and select 'Enable'";
	#endif
	
	//private Transform lookPos;
	private Vector3 goalPos;
	private Vector3 finalLookPos;
	private Camera myCam;
	private float initFOV;
	private Quaternion goalRot;
	
	#if UNITY_EDITOR
	private void resetLookAtOffset()  
	{
		lookAtOffset = new Vector3(0f,0f,0f);
		EditorUtility.SetDirty(this);
	}
	private void resetFollowPosition()  
	{
		followPosition = new Vector3(0f,2f,-5f);
		EditorUtility.SetDirty(this);
	}
	private void resetFollowSpeed()  
	{
		followSpeed = 5f;
		EditorUtility.SetDirty(this);
	}
	private void resetFieldOfViewEffect()  
	{
		FieldOfViewEffect = 0f;
		EditorUtility.SetDirty(this);
	}
	#endif
	
	
	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;
		myCam = GetComponent<Camera>();
		initFOV = myCam.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//pinch fingers to zoom camera//
		#if MOBILE_INPUT
		pinchZoom();
		#endif
		
		
		//pinch camera based on speed of vehicle//
		float fov = initFOV + (carObj.GetComponent<vehicleController>().zVel*FieldOfViewEffect);
		Mathf.Clamp(fov, initFOV, 169.9f);
		
		myCam.fieldOfView = fov;
		if(myCam.fieldOfView > 169.9f)
		{
			myCam.fieldOfView = 169.9f;
		}
		
		if(followSpeed >= 100f)
		{
			moveCamera();
		}
	}
	
	void FixedUpdate()
	{
		if(followSpeed < 100f)
		{
			moveCamera();
		}
	}
	
	void moveCamera()
	{
		if(useCarRotation)
		{
			goalPos = carObj.transform.position + (carObj.transform.forward*followPosition.z) + (carObj.transform.up*followPosition.y) + (carObj.transform.right*followPosition.x);
			finalLookPos = carObj.transform.position + (carObj.transform.forward*lookAtOffset.z) + (carObj.transform.up*lookAtOffset.y) + (carObj.transform.right*lookAtOffset.x);
		}
		else
		{
			goalPos = carObj.transform.position + followPosition;
			finalLookPos = carObj.transform.position + lookAtOffset;
		}
		transform.position = Vector3.Lerp(transform.position,goalPos,followSpeed*Time.deltaTime);

		if(tiltWithSlopes)
		{
			goalRot = Quaternion.LookRotation(finalLookPos - transform.position,carObj.transform.up);
			transform.rotation = Quaternion.Lerp(transform.rotation,goalRot,followSpeed*0.05f);
		}
		else
		{
			transform.LookAt(finalLookPos);
		}



		
	}
	
	void pinchZoom()
	{
		#if MOBILE_INPUT
		//if there are exactly 2 touches//
		if(Input.touchCount == 2)
		{
			//store both touches//
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			//find position of each touch in previous frame//
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			
			//distance between touches on previous and current frame//
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltMag = (touchZero.position - touchOne.position).magnitude;
			
			//find difference in distances between each frame//
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltMag; 
			
			//change default fov//
			initFOV += deltaMagnitudeDiff * fingerPinchSpeed;
			
			//restrict to between 0 and 180//
			initFOV = Mathf.Clamp(initFOV, 0.1f, 169.9f);
		}
		#endif
	}
	
	
	
	//Cam Script - Version 1.2 - Aaron Hibberd - 12.02.2015 - www.hibbygames.com//
}







