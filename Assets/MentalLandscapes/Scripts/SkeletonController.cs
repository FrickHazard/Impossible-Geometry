using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CapsuleCollider))]
public class SkeletonController : MonoBehaviour
{

    public Camera cam;
    public Vector3 GravityDirection;
    public float GravityConstant;
    public float Mass=1f;
    public bool onGround;
    public float speed;
    public float maximumPlayerVelocity;
    public float mouseSentivityMulitplier;
    public float jumpForce;
    public float InAirMovementMultiplier;
    public float FrictionOnStop;
    public LayerMask jumpMask;

    protected Vector3 temp;
    protected float height;
    protected float width;
    protected Rigidbody rigid;
    protected CapsuleCollider capsuleCollider;

    private float _startDynamicFriction = 0f;

    // Use this for initialization
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        height = capsuleCollider.height;
        width = capsuleCollider.radius * 2;
        Cursor.lockState = CursorLockMode.Locked;
        rigid = GetComponent<Rigidbody>();
        GravityDirection = GravityDirection.normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GravityDirection = -this.transform.up.normalized;
        ApplyGravity();
        GroundCheck();

       
            temp = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) { temp += this.transform.forward; }
            if (Input.GetKey(KeyCode.S)) { temp += -this.transform.forward; }
            if (Input.GetKey(KeyCode.A)) { temp += -this.transform.right; }
            if (Input.GetKey(KeyCode.D)) { temp += this.transform.right;  }
           
            temp = Vector3.Normalize(temp) * speed;
            ApplyFriction(!(temp.magnitude == 0));
            if (!onGround) { temp *= InAirMovementMultiplier; }
            rigid.AddForce(temp * Time.deltaTime , ForceMode.Impulse);

        

            
        float tempY = Input.GetAxis("Mouse Y");
        float tempX = Input.GetAxis("Mouse X");
        //this.transform.Rotate(0, tempX * mouseSentivityMulitplier, 0);
        this.transform.Rotate(this.transform.up, tempX * mouseSentivityMulitplier,Space.World);
        cam.transform.Rotate(cam.transform.right,-tempY*mouseSentivityMulitplier,Space.World);

        cam.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, 0,0);
   
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (onGround == false) { return; }
        rigid.AddForce(this.transform.up * jumpForce, ForceMode.VelocityChange);
    }


    public bool GroundCheck()
    {
        onGround = Physics.BoxCast(transform.position, new Vector3(width / 4, 0.1f, width / 4), -this.transform.up, Quaternion.identity, height / 2, jumpMask);
        Debug.DrawLine(transform.position, transform.position - (-this.transform.up * height / 2), Color.blue, 0.2f, false);
        return onGround;
    }


    void ApplyGravity()
    {
        Vector3 vec = Vector3.zero;
        vec += GravityDirection * GravityConstant;
        Vector3 acceleration = vec / Mass;
        rigid.velocity += acceleration * Time.fixedDeltaTime;
    }

    void ApplyFriction(bool moving)
    {
        if(!moving)
        capsuleCollider.material.dynamicFriction = FrictionOnStop;
        else { capsuleCollider.material.dynamicFriction = _startDynamicFriction; }
    }
}