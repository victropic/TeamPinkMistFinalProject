using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    /* Input */
    float h, v;
    float mouseX, mouseY;
    float jump;

    /* Movement Stats */
    public float crouchSpeed;
    public float walkSpeed;
    public float runSpeed;
    public Vector2 lookSpeed;
    public float jumpIntensity;
    
    /* States */
    bool running;
    bool crouching;
    public bool canRun;
    bool frozen;

    /* Physics */
    public float gravityMult;
    float verticalSpeed;

    /* Components */
    Camera cam;
    CharacterController charContr;
    GameController gameController;

    /* Audio */
    public float stepRate = 0.5f;
    public List<GameObject> stepSounds;
    private float stepTimer;
    private float stepChangeTimer;

    // Start is called before the first frame update
    void Start()
    {
        canRun = true;

        cam = GetComponentInChildren<Camera>();
        charContr = GetComponent<CharacterController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        stepTimer = stepRate;
    }

    // Update is called once per frame
    void Update()
    {
        running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        crouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        jump = Input.GetAxis("Jump");

        if(!gameController.UIMode && !frozen) {
            transform.rotation *=  Quaternion.AngleAxis(lookSpeed.x * mouseX, Vector3.up);
            cam.transform.rotation *= Quaternion.AngleAxis(lookSpeed.y * mouseY, Vector3.left);
        }

        if(Walking) {

            stepChangeTimer = 0.1f;
            
        } else {
            stepChangeTimer -= Time.deltaTime;
        }
        
        if(Walking || stepChangeTimer > 0f) {
            if(Running)
                stepTimer -= Time.deltaTime * 1.5f;
            else
                stepTimer -= Time.deltaTime;
            if(stepTimer < 0f) {
                PlayStepSound();
                stepTimer = stepRate;
            }
        }
        
    }

    void FixedUpdate() {

        if(!frozen) {
            float speed = walkSpeed;
            if(Running) {
                speed = runSpeed;

            } else if(crouching) {
                speed = crouchSpeed;
                charContr.height = 0.75f;
            }

            if(!crouching) {
                charContr.height = 1.5f;
            }

            charContr.Move(transform.rotation * new Vector3(h, 0f, v).normalized * speed * Time.deltaTime + Vector3.up * verticalSpeed * Time.deltaTime);

            if(charContr.isGrounded) {
                verticalSpeed = jump * jumpIntensity;
            } else {
                verticalSpeed -= Physics.gravity.magnitude * gravityMult * Time.deltaTime;
            }
        }
    }

    private void PlayStepSound() {
        if(stepSounds.Count > 0) {
            System.Random random = new System.Random();

            int rand = (int)Mathf.Floor((float)random.NextDouble() * stepSounds.Count);
            AudioSource audio = Instantiate(stepSounds[rand], transform.position, Quaternion.identity).GetComponent<AudioSource>();

            float volume = 0f;
            if(Running) {
                volume = 0.1f;
            }
            audio.volume = audio.volume + (float)random.NextDouble() * 0.02f - 0.01f + volume;
            audio.pitch = audio.pitch + (float)random.NextDouble() * 0.1f - 0.1f;
        }
    }

    public IEnumerator Rest () {
        canRun = false;
        yield return new WaitForSeconds(2f);
        canRun = true;
    }

    public bool Running {
        get {
            return canRun && running && (v > 0.1f && Mathf.Abs(h) < 0.1f);
        }
    }

    public bool Walking {
        get {
            return charContr.isGrounded && (Mathf.Abs(v) > 0.1f || Mathf.Abs(h) > 0.1f);
        }
    }

    public IEnumerator FreezeForShortTime() {
        frozen = true;
        yield return new WaitForSeconds(0.1f);
        frozen = false;
    }

    public bool Frozen {
        get { return frozen; }
        set { frozen = value; }
    }
}
