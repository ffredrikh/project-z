using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed;

    [Header("Drive Settings")]
    public float driveForce = 200f;
    public float slowingVelFactor = .99f;
    public float brakingVelFactor = .95f;
    public float angleOfRoll = 0f;

    [Header("Hover Settings")]
    public float hoverHeight = 4f;
    public float maxGroundDist = 4f;
    public float hoverForce = 100f;
    public LayerMask whatIsGround;
    public PIDController hoverPID;

    [Header("Physics Settings")]
    public Transform shipBody;
    public float terminalVelocity = 100f;
    public float hoverGravity = 20f;
    public float fallGravity = 400f;

    Rigidbody rigidBody;
    float drag;
    bool isOnGround;

    Vector3 startPos;
    public float moveSpeed = 10f;
    public float jumpForce = 200;
    public float playerForceFrwd = 8f;
    public PlayerCamera Camera;
    private Stopwatch stopwatch;
    public GameObject finish;
    public GameObject Explosion;
    private ParticleSystem ExplotionParticles;
    public Animator fadeAnim;
    bool intro;

    // UI
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI speedUI;
    public Button jumpForward;
    public Button accelerate;
    public Button inGameMenu;
    public Image fadeImg;

    private bool right = false;
    private bool left = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        drag = driveForce / terminalVelocity;
        rigidBody.freezeRotation = true;
        startPos = gameObject.transform.position;

        stopwatch = new Stopwatch();
        stopwatch.Start();

        ExplotionParticles = Explosion.GetComponent<ParticleSystem>();

        intro = true;
        //Time.timeScale = 1;
    }


    public void Jump()
    {
        if (isOnGround)
        {
            rigidBody.AddForce(new Vector3(0f, jumpForce, 50f), ForceMode.Impulse);
        }
    }


    public void AccelerateDown()
    {
        playerForceFrwd = 1f;
    }


    public void AccelerateUp()
    {
        playerForceFrwd = 8f;
    }

    public void RightDown()
    {
        right = true;
    }

    public void RightUp()
    {
        right = false;
    }

    public void LeftDown()
    {
        left = true;
    }

    public void LeftUp()
    {
        left = false;
    }


    void FixedUpdate()
    {
        if (intro)
            RunIntro();

        timeUI.text = stopwatch.Elapsed.Seconds.ToString();
        speed = Vector3.Dot(rigidBody.velocity, transform.forward);
        speedUI.text = ((int)speed / 5).ToString();

        CalculatHover();
        CalculatePropulsion();

        // FOR TEST IN EDITOR

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Jump();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 movement = new Vector3(-1, 0, 0);
            rigidBody.AddForce(movement * 2, ForceMode.Impulse);
            transform.Rotate(new Vector3(0, -1, 1));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 movement = new Vector3(1, 0, 0);
            rigidBody.AddForce(movement * 2, ForceMode.Impulse);
            transform.Rotate(new Vector3(0, 1, -1));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerForceFrwd = 1f;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {     
            playerForceFrwd = 8f;
        }

        // END TEST

        if (gameObject.transform.position.y < 1000 || (int)speed <= 0 && gameObject.transform.position.z > 2000)
        {
            ExplotionParticles.Play();
            StartCoroutine(ShipExplosion());
        }

        if (right)
        {
            Vector3 movement = new Vector3(1, 0, 0);
            rigidBody.AddForce(movement * 2, ForceMode.Impulse);
            transform.Rotate(new Vector3(0, 1, -1));
        } 


        if (left)
        {
            Vector3 movement = new Vector3(-1, 0, 0);
            rigidBody.AddForce(movement * 2, ForceMode.Impulse);
            transform.Rotate(new Vector3(0, -1, 1));
        }

    }


    private void RunIntro()
    {
        if (stopwatch.Elapsed.Seconds == 10)
        {
            jumpForward.gameObject.SetActive(true);
        }

        if (stopwatch.Elapsed.Seconds == 11)
        {
            jumpForward.gameObject.SetActive(false);
        }

        if (stopwatch.Elapsed.Seconds == 12)
        {
            jumpForward.gameObject.SetActive(true);
        }

        if (stopwatch.Elapsed.Seconds == 14)
        {
            accelerate.gameObject.SetActive(true);
        }

        if (stopwatch.Elapsed.Seconds == 15)
        {
            accelerate.gameObject.SetActive(false);
        }

        if (stopwatch.Elapsed.Seconds == 16)
        {
            accelerate.gameObject.SetActive(true);
        }

        if (stopwatch.Elapsed.Seconds == 20)
        {
            fadeImg.gameObject.SetActive(false);
            timeUI.gameObject.SetActive(true);
            intro = false;
        }

    }


    void CalculatHover()
    {
        Vector3 groundNormal;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitInfo;
        isOnGround = Physics.Raycast(ray, out hitInfo, maxGroundDist, whatIsGround);

        if (isOnGround)
        {
            float height = hitInfo.distance;

            groundNormal = hitInfo.normal.normalized;
            float forcePercent = hoverPID.Seek(hoverHeight, height);
            Vector3 force = groundNormal * hoverForce * forcePercent;
            Vector3 gravity = -groundNormal * hoverGravity * height;

            rigidBody.AddForce(force, ForceMode.Acceleration);
            rigidBody.AddForce(gravity, ForceMode.Acceleration);

        }
        else
        {
            groundNormal = Vector3.up;
            Vector3 gravity = -groundNormal * fallGravity;
            rigidBody.AddForce(gravity, ForceMode.Acceleration);
        }

        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);
        rigidBody.MoveRotation(Quaternion.Lerp(rigidBody.rotation, rotation, Time.deltaTime * 10f));
    }


    void CalculatePropulsion()
    {
        if (Input.GetAxis("Vertical") <= 0f)
            rigidBody.velocity *= slowingVelFactor;

        float propulsion = driveForce * playerForceFrwd - drag * Mathf.Clamp(speed, 0f, terminalVelocity);
        rigidBody.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
        rigidBody.AddForce(transform.forward * (Input.GetAxis("Vertical") - drag), ForceMode.Acceleration);
    }


    public float GetSpeedPercentage()
    {
        return rigidBody.velocity.magnitude / terminalVelocity;
    }


    IEnumerator FixPos()
    {
        var xpos = transform.position.x;
        var min = xpos - 97;
        var max = xpos + 97;

        while (xpos > min && xpos < max)
        {
            xpos = transform.position.x;
            yield return null;
        }
        rigidBody.freezeRotation = true;
    }


    IEnumerator FinishCameraRotation()
    {
        bool dflag = true;
        bool hflag = true;
        while (true)
        {

            if (Camera.distance % 200 == 0)
                dflag = false;

            if (Camera.distance % 300 == 0)
                dflag = true;

            if (dflag)
                Camera.distance += -0.5f;
            else
                Camera.distance += 0.5f;


            if (Camera.height % 200 == 0)
                hflag = false;

            if (Camera.height % 300 == 0)
                hflag = true;

            if (hflag)
                Camera.height += 0.1f;
            else
                Camera.height += -0.1f;

            yield return new WaitForSeconds(0.05f);

        }
    }


    IEnumerator ShipExplosion()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        ExplotionParticles.Stop();

        gameObject.SetActive(true);
        gameObject.transform.position = startPos;

        rigidBody.rotation = Quaternion.Euler(0, 0, 0);
        rigidBody.velocity = new Vector3(0, 0, 0);
        stopwatch.Restart();

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            ExplotionParticles.Play();
            StartCoroutine(ShipExplosion());
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "FinishLine")
        {
            fadeImg.gameObject.SetActive(true);
            stopwatch.Stop();

            playerForceFrwd = 0f;
            StartCoroutine(FadeOut());
        }

        if (other.gameObject.tag == "Wall")
        {

            ExplotionParticles.Play();
            StartCoroutine(ShipExplosion());
        }
    }


    IEnumerator FadeOut()
    {
        //fadeAnim.Play("OutroFadeAnim");
        yield return new WaitForSeconds(6f);
        rigidBody.drag = 20;
    }
}
