using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 180, gravityForce = 10f, dragOnGround = 3f;

    private bool grounded;

    private float speedInput, turnInput;


    public LayerMask whatIsGround;
    public float groundRayLength = .5f;
    public Transform groundRayPoint;

    public Transform leftFrontWheel, rightFrontWheel;

    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f;
    private float emissionRate;

    // Start is called before the first frame update


    void Start()
    {
        theRB.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0f; // Getting forward and backwards input using getAxis vertical, if it registers the W or up arrow key, go forward * 1000, if it registers S or down arrow key, go revsereAccel by 1000
        if(Input.GetAxis("Vertical") > 0 )
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }

        // this is for horizontal movement
        turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x,turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);

        transform.position = theRB.transform.position;
    }

    private void FixedUpdate()
    {
        emissionRate = 0;
        grounded = false;
        RaycastHit hit; 
        
        if(Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if (grounded)
        {
            theRB.drag = dragOnGround;
            if (Mathf.Abs(speedInput) > 0)
            {
                theRB.AddForce(transform.forward * speedInput);
                emissionRate = maxEmission;

            }
        }
        else
        {
            theRB.drag = 0.1f;
            theRB.AddForce(Vector3.up * -gravityForce * 100f);
        }
        
            foreach(ParticleSystem part in dustTrail)
        {
            var emissionModule = part.emission;
            emissionModule.rateOverTime = emissionRate; 
        }
    }
}



