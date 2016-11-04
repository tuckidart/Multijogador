using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
//using UnityStandardAssets.CrossPlatformInput;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class vehicleController : NetworkBehaviour {

	public bool alive = true;
	
	[Header("Meshes")]
	
	[Tooltip("Add a vehicle body here. we recommend creating an empty GameObject (reset it's transforms) and put your car body parts inside.")]
	public GameObject vehicleBody;
	[Tooltip("Add a front left wheel here. This should be fine as is but the same rule as the body can apply if you wish")]
	public GameObject wheelLeftFront;
	[Tooltip("Add a front right wheel here. This should be fine as is but the same rule as the body can apply if you wish")]
	public GameObject wheelRightFront;
	[Tooltip("Add a back left wheel here. This should be fine as is but the same rule as the body can apply if you wish")]
	public GameObject wheelLeftBack;
	[Tooltip("Add a back right wheel here. This should be fine as is but the same rule as the body can apply if you wish")]
	public GameObject wheelRightBack;
	
	[Header("Physics Materials")]
	
	[Tooltip("Add a Physics Material for the tires, To edit in the inspector select the material in the project folder. we recommend a low dynamic and static friction of 0.1, and Minimum Friction Combine")]
	public PhysicMaterial tirePhysicsMat;
	[Tooltip("Add a Physics Material for the body of the vehicle, To edit in the inspector select the material in the project folder. increase Bounciness above 0 will cause stronger bounce on collision")]
	public PhysicMaterial bodyPhysicsMat;

	[Header("Body Colliders")]
	public GameObject[] bodyColliders;
	
	[Header("Vehicle Tuning")]
	
	[Tooltip("Want to go faster? Pick a higher number")]
	[Range(0f,3000f)]
	[ContextMenuItem("Reset 'Horsepower' to default", "resetHorsepower")]
	public float horsepower = 220f;
	[Tooltip("Want responsive steering? Go higher!")]
	[Range(0f,300f)]
	[ContextMenuItem("Reset 'Steering' to default", "resetSteering")]
	public float steering = 70f;
	[Tooltip("Want to stop drifting sideways so much? Bigger number will help resist the soapy floor")]
	[Range(0f,100f)]
	[ContextMenuItem("Reset 'Tire Grip' to default", "resetTireGrip")]
	public float tireGrip = 100f;
	[Tooltip("Want to help your wheels stay on the floor? This will hang the wheels a little more (1 = half wheel radius) 1 or less is recommended")]
	[Range(0f,10f)]
	[ContextMenuItem("Reset 'Wheel Hang Distance' to default", "resetWheelHangDistance")]
	public float wheelHangDistance = 1f;
	[Tooltip("The wheels wobble with the body rotation, If you have loose suspension you probably want this to be false, if you have tight suspension it should be true")]
	[ContextMenuItem("Reset 'Wheel Tilt with Body' to default", "resetWheelTiltBody")]
	public bool wheelTiltWithBody = true;
	[Tooltip("Want to make the body swing around like babies rattle? Higher value is better. Each axis can be tweaked. Most times Y can equal 0")]
	[ContextMenuItem("Reset 'Suspension Lengths' to default", "resetSuspensionLengths")]
	public Vector3 suspensionLengths = new Vector3(0.4f,0f,0.4f);
	[Tooltip("Want to limit the swing strength of the body? A higher value is less swing (more tension)")]
	[Range(0f,100)]
	[ContextMenuItem("Reset 'Suspension Tension' to default", "resetSuspensionTension")]
	public float suspensionTension = 15f;
	[Tooltip("If you happen to drive through a puddle, this is the width of the trail renderer")]
	[ContextMenuItem("Reset 'Tire Trail Width' to default", "resetTireTrailWidth")]
	public float tireTrailWidth = 0.5f;
	
	[Header("Behaviour")]
	
	[Tooltip("Did you land on your head? This will fix that and flip you upright")]
	public bool autoCorrectRot = true;
	
	
	[Header("Vehicle Lights (Optional)")]
	public Renderer brakelights;
	public Material brakelightON;
	public Material brakelightOFF;
	public Renderer headlights;
	public Material headlightsON;
	public Material headlightsOFF;
	public Renderer indicatorLEFT;
	public Renderer indicatorRIGHT;
	public Material indicatorON;
	public Material indicatorOFF;
	private bool brakesON = false;
	private bool headsON = true;
	private bool indLeftON = false;
	private bool indRightON = false;
	private float indLtime = 0f;
	private float indRtime = 0f;
	
	[HideInInspector]
	public Rigidbody rbody;
	private Rigidbody jbody;

	[HideInInspector]
	private GameObject suspensionBody;
	private GameObject colSuspension;
	private GameObject wheels;
	private GameObject colliders;
	private GameObject colLB;
	private GameObject colLF;
	private GameObject colRB;
	private GameObject colRF;
	private GameObject turnLF;
	private GameObject turnRF;
	
	[HideInInspector]
	[SyncVar]
	public float inputX;
	//[HideInInspector]
	public float inputY;
	private float xVel;
	[HideInInspector]
	[SyncVar]
	public float zVel;

	[HideInInspector]
	public bool touchLB;
	[HideInInspector]
	public bool touchLF;
	[HideInInspector]
	public bool touchRB;
	[HideInInspector]
	public bool touchRF;
	private int tiresOnGround;
	private int FtiresOnGround;
	private int BtiresOnGround;
	private float airTime;
	private float unstableTime = 0f;
	[HideInInspector]
	public bool roofOnGround = false;
	private Vector3 defaultCOG;
	private float boundSize = 1f;
	private bool speedboostON = false;
	private float speedboostLife = 0f;
	private float speedboostStrength = 0f;
	
	#if UNITY_EDITOR
	private void resetHorsepower()  
	{
		horsepower = 220f;
		EditorUtility.SetDirty(this);
	}
	private void resetSteering()  
	{
		steering = 70f;
		EditorUtility.SetDirty(this);
	}
	private void resetTireGrip()  
	{
		tireGrip = 100f;
		EditorUtility.SetDirty(this);
	}
	private void resetWheelHangDistance()  
	{
		wheelHangDistance = 1f;
		EditorUtility.SetDirty(this);
	}
	private void resetWheelTiltBody()  
	{
		wheelTiltWithBody = true;
		EditorUtility.SetDirty(this);
	}
	private void resetSuspensionLengths()  
	{
		suspensionLengths = new Vector3(0.4f,0f,0.4f);
		EditorUtility.SetDirty(this);
	}
	private void resetSuspensionTension()  
	{
		suspensionTension = 15f;
		EditorUtility.SetDirty(this);
	}
	private void resetTireTrailWidth()  
	{
		tireTrailWidth = 0.5f;
		EditorUtility.SetDirty(this);
	}
	#endif
	
	void Awake()
	{
		//make sure vehicleBody has the correct parent//
		vehicleBody.transform.SetParent(transform);
		
		//create a wheels holder
		wheels = new GameObject("Wheels");
		wheels.transform.SetParent(transform);
		wheels.transform.position = transform.position;
		wheels.transform.rotation = transform.rotation;
		
		//create rigidbody//
		rbody = GetComponent<Rigidbody>();
		rbody.interpolation = RigidbodyInterpolation.Interpolate;
		rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rbody.mass = 100f;
		rbody.angularDrag = 5f;
		
		//set up the wheels//
		
		//rear left wheel//
		Mesh mesh = wheelLeftBack.GetComponentInChildren<MeshFilter>().mesh;
		getDiameter(mesh);
		colLB = new GameObject("colLB");
		SphereCollider col = colLB.AddComponent<SphereCollider>();
		if(wheelLeftBack.transform.childCount > 0)
		{
			col.radius = (boundSize/2f)*wheelLeftBack.transform.GetChild(0).transform.lossyScale.y;
			colLB.transform.position = wheelLeftBack.transform.position;
		}
		else
		{
			col.radius = (boundSize/2f)*wheelLeftBack.transform.lossyScale.y;
			colLB.transform.position = wheelLeftBack.transform.position + mesh.bounds.center;
		}
		col.material = tirePhysicsMat;
		col.center = new Vector3(0f,0f,0f);
		colLB.transform.SetParent(transform);
		wheel wheelScript = colLB.AddComponent<wheel>();
		wheelScript.vehicleObj = gameObject;
		wheelScript.wheelObj = wheelLeftBack;
		wheelScript.wheelLB = true;
		wheelScript.wheelHangDistance = wheelHangDistance;
		wheelScript.tireTrailWidth = tireTrailWidth;
		wheelLeftBack.transform.SetParent(wheels.transform);
		
		//front left wheel//
		mesh = wheelLeftFront.GetComponentInChildren<MeshFilter>().mesh;
		getDiameter(mesh);
		colLF = new GameObject("colLF");
		col = colLF.AddComponent<SphereCollider>();
		if(wheelLeftFront.transform.childCount > 0)
		{
			col.radius = (boundSize/2f)*wheelLeftFront.transform.GetChild(0).transform.lossyScale.y;
			colLF.transform.position = wheelLeftFront.transform.position;
		}
		else
		{
			col.radius = (boundSize/2f)*wheelLeftFront.transform.lossyScale.y;
			colLF.transform.position = wheelLeftFront.transform.position + mesh.bounds.center;
		}
		col.material = tirePhysicsMat;
		col.center = new Vector3(0f,0f,0f);
		colLF.transform.SetParent(transform);
		wheelScript = colLF.AddComponent<wheel>();
		wheelScript.vehicleObj = gameObject;
		wheelScript.wheelObj = wheelLeftFront;
		wheelScript.wheelLF = true;
		wheelScript.wheelHangDistance = wheelHangDistance;
		wheelScript.tireTrailWidth = tireTrailWidth;
		
		turnLF = new GameObject("turnLF");
		turnLF.transform.SetParent(wheels.transform);
		turnLF.transform.position = wheelLeftFront.transform.position;
		turnLF.transform.rotation = wheels.transform.rotation;
		wheelLeftFront.transform.SetParent(turnLF.transform);
		wheelScript.turnObj = turnLF;
		wheelScript.frontTire = true;
		
		//rear right wheel//
		mesh = wheelRightBack.GetComponentInChildren<MeshFilter>().mesh;
		getDiameter(mesh);
		colRB = new GameObject("colRB");
		col = colRB.AddComponent<SphereCollider>();
		if(wheelRightBack.transform.childCount > 0)
		{
			col.radius = (boundSize/2f)*wheelRightBack.transform.GetChild(0).transform.lossyScale.y;
			colRB.transform.position = wheelRightBack.transform.position;
		}
		else
		{
			col.radius = (boundSize/2f)*wheelRightBack.transform.lossyScale.y;
			colRB.transform.position = wheelRightBack.transform.position + mesh.bounds.center;
		}
		col.material = tirePhysicsMat;
		col.center = new Vector3(0f,0f,0f);
		colRB.transform.SetParent(transform);
		wheelScript = colRB.AddComponent<wheel>();
		wheelScript.vehicleObj = gameObject;
		wheelScript.wheelObj = wheelRightBack;
		wheelScript.wheelRB = true;
		wheelScript.wheelHangDistance = wheelHangDistance;
		wheelScript.tireTrailWidth = tireTrailWidth;
		wheelRightBack.transform.SetParent(wheels.transform);
		
		//front right wheel//
		mesh = wheelRightFront.GetComponentInChildren<MeshFilter>().mesh;
		getDiameter(mesh);
		colRF = new GameObject("colRF");
		col = colRF.AddComponent<SphereCollider>();
		if(wheelRightFront.transform.childCount > 0)
		{
			col.radius = (boundSize/2f)*wheelRightFront.transform.GetChild(0).transform.lossyScale.y;
			colRF.transform.position = wheelRightFront.transform.position;
		}
		else
		{
			col.radius = (boundSize/2f)*wheelRightFront.transform.lossyScale.y;
			colRF.transform.position = wheelRightFront.transform.position + mesh.bounds.center;
		}
		col.material = tirePhysicsMat;
		col.center = new Vector3(0f,0f,0f);
		colRF.transform.SetParent(transform);
		wheelScript = colRF.AddComponent<wheel>();
		wheelScript.vehicleObj = gameObject;
		wheelScript.wheelObj = wheelRightFront;
		wheelScript.wheelRF = true;
		wheelScript.wheelHangDistance = wheelHangDistance;
		wheelScript.tireTrailWidth = tireTrailWidth;
		
		turnRF = new GameObject("turnRF");
		turnRF.transform.SetParent(wheels.transform);
		turnRF.transform.position = wheelRightFront.transform.position;
		turnRF.transform.rotation = wheels.transform.rotation;
		wheelRightFront.transform.SetParent(turnRF.transform);
		wheelScript.turnObj = turnRF;
		wheelScript.frontTire = true;
		
		//create a body holder//
		suspensionBody = new GameObject("Suspension");
		suspensionBody.transform.SetParent(transform);
		suspensionBody.transform.position = transform.position;
		suspensionBody.transform.rotation = transform.rotation;
		
		jbody = suspensionBody.AddComponent<Rigidbody>();
		jbody.useGravity = false;
		jbody.mass = 10f;
		jbody.drag = 1f;
		jbody.angularDrag = 2f;
		jbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		CharacterJoint joint = suspensionBody.AddComponent<CharacterJoint>();
		joint.connectedBody = rbody;
		joint.axis = new Vector3(1f,0f,0f);
		joint.swingAxis = new Vector3(1f,0f,0f);
		joint.enablePreprocessing = false;
		joint.enableProjection = true;
		
		SoftJointLimitSpring temp = joint.twistLimitSpring;
		temp.spring = suspensionTension*400f;
		temp.damper = 0.4f;
		joint.twistLimitSpring = temp;
		
		SoftJointLimit temp2 = joint.lowTwistLimit;
		temp2.limit = suspensionLengths.x*-1f;
		temp2.bounciness = 1f;
		joint.lowTwistLimit = temp2;
		
		SoftJointLimit temp3 = joint.highTwistLimit;
		temp3.limit = suspensionLengths.x;
		temp3.bounciness = 1f;
		joint.highTwistLimit = temp3;
		
		SoftJointLimitSpring temp4 = joint.swingLimitSpring;
		temp4.spring = suspensionTension*400f;
		temp4.damper = 0.4f;
		joint.swingLimitSpring = temp4;
		
		SoftJointLimit temp5 = joint.swing1Limit;
		temp5.limit = suspensionLengths.y*0.1f;
		temp5.bounciness = 1f;
		joint.swing1Limit = temp5;
		
		SoftJointLimit temp6 = joint.swing2Limit;
		temp6.limit = suspensionLengths.z;
		temp6.bounciness = 1f;
		joint.swing2Limit = temp6;
		
		//create body collider//
		colSuspension = new GameObject("colSuspension");
		colSuspension.transform.position = suspensionBody.transform.position;
		colSuspension.transform.rotation = suspensionBody.transform.rotation;
		colSuspension.transform.SetParent(suspensionBody.transform);
		BoxCollider col2 = colSuspension.AddComponent<BoxCollider>();
		float distX = Vector3.Distance(wheelLeftFront.transform.position,wheelRightFront.transform.position);
		float distZ = Vector3.Distance(wheelLeftFront.transform.position,wheelLeftBack.transform.position);
		col2.size = new Vector3(distX - (col.radius),(col.radius)*0.5f,distZ + ((col.radius)*2f));
		col2.center = new Vector3(0f,col.radius,0f);
		col2.material = bodyPhysicsMat;
		
		jbody.centerOfMass = col2.center + new Vector3(0f,col.radius*0.5f,0f);
		rbody.centerOfMass = col2.center + new Vector3(0f,-col.radius*0.5f,0f);
		defaultCOG = rbody.centerOfMass;

		colliders = new GameObject("Wheel Colliders");
		colliders.transform.SetParent(transform);
		colliders.transform.position = transform.position;
		colliders.transform.rotation = transform.rotation;
		colLB.transform.SetParent(colliders.transform);
		colRB.transform.SetParent(colliders.transform);
		colRF.transform.SetParent(colliders.transform);
		colLF.transform.SetParent(colliders.transform);

		for(int i=0;i<bodyColliders.Length;i++)
		{
			bodyColliders[i].transform.SetParent(vehicleBody.transform);
		}
	}
	
	void getDiameter(Mesh mesh)
	{
		//of x y z, if two of them are equal, that must be the diameter//
		if(mesh.bounds.size.z == mesh.bounds.size.x)
		{
			boundSize = mesh.bounds.size.z;
		}
		else if(mesh.bounds.size.z == mesh.bounds.size.y)
		{
			boundSize = mesh.bounds.size.z;
		}
		else if(mesh.bounds.size.x == mesh.bounds.size.y)
		{
			boundSize = mesh.bounds.size.x;
		}
		//if the wheel is not round, go with the largest length for diameter//
		else
		{
			boundSize = mesh.bounds.size.z;
			
			if(mesh.bounds.size.x > boundSize)
			{
				boundSize = mesh.bounds.size.x;
			}
			if(mesh.bounds.size.y > boundSize)
			{
				boundSize = mesh.bounds.size.y;
			}
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		inputX = 0f;
		inputY = 0f;
		
		xVel = 0f;
		zVel = 0f;
		
		//prevent colliding with seams in ground//
		Physics.defaultContactOffset = 0.001f;
	}
	
	void Update()
	{
		//track how many tires are touching the ground//
		tiresOnGround = 0;
		FtiresOnGround = 0;
		BtiresOnGround = 0;
		if(touchLB)
		{
			tiresOnGround++;
			BtiresOnGround++;
			airTime = 0f;
		}
		if(touchLF)
		{
			tiresOnGround++;
			FtiresOnGround++;
			airTime = 0f;
		}
		if(touchRB)
		{
			tiresOnGround++;
			BtiresOnGround++;
			airTime = 0f;
		}
		if(touchRF)
		{
			tiresOnGround++;
			FtiresOnGround++;
			airTime = 0f;
		}
		
		if(tiresOnGround <= 0)
		{
			airTime+=Time.deltaTime;
		}
		
		if(tiresOnGround >= 4)
		{
			unstableTime = 0f;
		}
		else
		{
			unstableTime += Time.deltaTime;
		}
		
		//prevent being stuck upside down//
		if(autoCorrectRot)
		{
			Vector3 rayUp = transform.TransformDirection(Vector3.up);
			//Debug.DrawRay(physicsBody.transform.position, rayUp*5f, Color.green);
			RaycastHit[] hits;
			roofOnGround = false;
			hits = Physics.RaycastAll (transform.position, rayUp,5f);
			
			if(hits.Length > 2)
			{
				roofOnGround = true;
			}
			
			if(tiresOnGround <= 0 && roofOnGround == true)
			{
				rbody.centerOfMass = new Vector3(0f,-5f*transform.localScale.y,0f);
			}
			
			if(tiresOnGround >=3)
			{
				rbody.centerOfMass = defaultCOG;
			}
			
			if(unstableTime > 5f)
			{
				if(rbody.velocity.magnitude < 2f)
				{
					rbody.centerOfMass = new Vector3(0f,-5f*transform.localScale.y,0f);
					
					if(unstableTime > 10f)
					{
						transform.rotation = Quaternion.Euler(0f,0f,0f);
						unstableTime = 6f;
					}
				}
			}
		}
		
		//head lights//
		if(headlights)
		{
			//switch lights on/off by pressing L key//
			if(Input.GetKeyDown(KeyCode.L))
			{
				if(headsON)
				{
					headlights.sharedMaterial = headlightsOFF;
					headsON = false;
				}
				else
				{
					headlights.sharedMaterial = headlightsON;
					headsON = true;
				}
			}
		}
		
		//indicator lights//
		if(indicatorLEFT)
		{
			
			if(indLeftON)
			{
				indLtime+=Time.deltaTime;
				float floor = 0f;
				float ceiling = 1f;
				float emission = floor + Mathf.PingPong(indLtime*2f, ceiling - floor);
				indicatorLEFT.sharedMaterial.SetColor("_EmissionColor",new Color(1f,1f,1f)*emission);
				
				//not pressing left//
				if(inputX >= -0.3f)
				{
					//wait until light dims before shutting off//
					if(emission <= 0.1f)
					{
						indicatorLEFT.sharedMaterial = indicatorOFF;
						indLeftON = false;
					}
				}
			}
			else if(!indRightON)
			{
				//pressing left//
				if(inputX < -0.3f)
				{
					indicatorLEFT.sharedMaterial = indicatorON;
					indLeftON = true;
					indLtime = 0f;
				}
			}
			
			
			
		}
		if(indicatorRIGHT)
		{
			if(indRightON)
			{
				indRtime+=Time.deltaTime;
				float floor = 0f;
				float ceiling = 1f;
				float emission = floor + Mathf.PingPong(indRtime*2f, ceiling - floor);
				indicatorRIGHT.sharedMaterial.SetColor("_EmissionColor",new Color(1f,1f,1f)*emission);
				
				//not pressing right//
				if(inputX <= 0.3f)
				{
					if(indRightON)
					{
						//wait until light dims before shutting off//
						if(emission <= 0.1f)
						{
							indicatorRIGHT.sharedMaterial = indicatorOFF;
							indRightON = false;
						}
					}
				}
			}
			else if(!indLeftON)
			{
				//pressing right//
				if(inputX > 0.3f)
				{
					indicatorRIGHT.sharedMaterial = indicatorON;
					indRightON = true;
					indRtime = 0f;
				}
			}
			
			
			
		}
		
		//brake lights//
		if(brakelights)
		{
			//braking//
			if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
			{
				if(!brakesON)
				{
					if(zVel > 0.01f)
					{
						brakelights.sharedMaterial = brakelightON;
						brakesON = true;
					}
				}
				else if(zVel < 0f)
				{
					brakelights.sharedMaterial = brakelightOFF;
					brakesON = false;
				}
			}
			//not braking//
			else if(brakesON)
			{
				brakelights.sharedMaterial = brakelightOFF;
				brakesON = false;
			}
		}

		//count down, then expire speed boost//
		if(speedboostON)
		{
			//wait a moment//
			if(speedboostLife > 0f)
			{
				speedboostLife -= Time.deltaTime;
			}
			//slowly lose strength//
			else if(speedboostStrength > 0f)
			{
				speedboostStrength -= (Time.deltaTime*10f);
			}
			//expire//
			else
			{
				speedboostON = false;

				//detach fire trail from rear wheels//
				colLB.GetComponent<wheel>().abandonFireTrail();
				colRB.GetComponent<wheel>().abandonFireTrail();
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(isLocalPlayer)
		{
			//input//
			inputX = Mathf.SmoothStep(inputX,Input.GetAxis("Horizontal"),(steering*0.2f)*Time.deltaTime);
			inputY = Input.GetAxis("Vertical");
		}
		else
		{
			if (inputX >= 1.0f)
				inputX = 1.0f;
			else if (inputX <= -1.0f)
				inputX = -1.0f;

			if (inputY >= 1.0f)
				inputY = 1.0f;
			else if (inputY <= -1.0f)
				inputY = -1.0f;
		}

		//cannot let off gas during a speedboost//
		if(speedboostON)
		{
			inputY = 1f + speedboostStrength;
		}

		//track velocity//
		xVel = transform.InverseTransformDirection(rbody.velocity).x;
		zVel = transform.InverseTransformDirection(rbody.velocity).z;

		if (alive) 
		{
			//accellerate forwards//
			if (inputY > 0)
			{
				if (FtiresOnGround > 0 || airTime < 0.6f)
				{
					rbody.AddForce (inputY * transform.forward * (horsepower * 400f) * Time.deltaTime);
				}
			}
			//accellerate backwards
			else
			{
				if (BtiresOnGround > 0)
				{
					rbody.AddForce (inputY * transform.forward * (horsepower * 400f) * Time.deltaTime);
				}
			}
		}
		
		if(tiresOnGround > 0 || airTime < 0.3f)
		{
			//stop sliding sideways//
			rbody.AddForce(transform.right*xVel*-(tireGrip*140f)*Time.deltaTime);
			
			//downforce//
			rbody.AddForce(transform.up*(Mathf.Abs(zVel)*-5000f)*Time.deltaTime);
		}
		else
		{
			if(airTime > 0.3f)
			{
				//increase gravity//
				rbody.AddForce((Physics.gravity * rbody.mass)*2f);
			}
		}
		
		//steer//
		float y = inputX*Time.deltaTime*steering;
		if(y > Mathf.Abs(zVel*0.2f))
		{
			y=Mathf.Abs(zVel*0.2f);
		}
		else if(y < (Mathf.Abs(zVel*0.2f))*-1f)
		{
			y = (Mathf.Abs(zVel*0.2f))*-1f;
		}
		if(zVel < 0f)
		{
			y*=-1f;
		}
		
		//steer//
		rbody.angularVelocity = new Vector3(rbody.angularVelocity.x,0f,rbody.angularVelocity.z);
		if(airTime > 0f)
		{
			transform.Rotate(0f,y*1.5f,0f);
		}
		else
		{
			transform.Rotate(0f,y*2f,0f);
		}
	
		//smoothen out the visual movement//
		float smooth = 0.1f + (rbody.velocity.magnitude*0.02f);
	
		transform.position = Vector3.Slerp(transform.position,transform.position,smooth);
		transform.rotation = Quaternion.Slerp(transform.rotation,transform.rotation,0.2f);
		vehicleBody.transform.rotation = Quaternion.Slerp(vehicleBody.transform.rotation, suspensionBody.transform.rotation,0.2f);
	}

	void LateUpdate()
	{
		wheels.transform.position = transform.position;
		if(wheelTiltWithBody)
		{
			wheels.transform.rotation = vehicleBody.transform.rotation;
		}
		else
		{
			wheels.transform.rotation = transform.rotation;
		}
	}

	public void hitSpeedBoost(GameObject fireTrailL, GameObject fireTrailR)
	{
		speedboostON = true;	
		speedboostLife = 0.5f;
		speedboostStrength = 1000f/horsepower;

		//ignite fire trail on rear wheels//
		colLB.GetComponent<wheel>().attachFireTrail(fireTrailL);
		colRB.GetComponent<wheel>().attachFireTrail(fireTrailR);
	}
	
	//Vehicle Controller Script - Version 1.2 - Aaron Hibberd - 1.12.2016 - www.hibbygames.com//
}