using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    //Parameters
    [SerializeField] float rcsTrust = 100f;
    [SerializeField] float mainTrust = 1500f;
    [SerializeField] States startingState = States.Alive;
    [SerializeField] AudioClip rocketTrustSFX;
    [SerializeField] AudioClip successSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] ParticleSystem rocketTrustVFX;
    [SerializeField] ParticleSystem successVFX;
    [SerializeField] ParticleSystem deathVFX;

    //Constants
    const string FRIENDLY = "Friendly";
    const string FINISH = "Finish";
    const string EJECT = "Eject";
    const string ROTATE = "Rotate";

    //Member Variables
    Rigidbody rigidbody;
    AudioSource audioSource;
    enum States { Alive, Dying, Transending, Launching };
    States state;
    bool collisionsDisabled = false;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        state = startingState;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == States.Alive)
        {
            RespondToTrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }

    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (state != States.Alive  || collisionsDisabled)  //ignore collisions
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case FRIENDLY:
                //do nothing
                break;
            case FINISH:
                Success();
                break;
            default:
                Die();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case EJECT:
                Eject();
                break;
            case ROTATE:
                Rotate90Degrees();
                break;
            default:
                break;
        }
    }


    private void RespondToTrustInput()
    {



        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            rocketTrustVFX.Stop();
        }
    }

    private void ApplyThrust()
    {
        rocketTrustVFX.Play();
        float thrustThisFrame = mainTrust * Time.deltaTime;
        rigidbody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(rocketTrustSFX);
        }
        
    }

    private void RespondToRotateInput()
    {
        rigidbody.angularVelocity = Vector3.zero; //Remove rotation caused by Physics engine

        float rotationThisFrame = rcsTrust * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

    }

    private void Success()
    {
        state = States.Transending;
        successVFX.Play();
        audioSource.PlayOneShot(successSFX);
        LoadNextLevel();
    }
    private void Die()
    {

        state = States.Dying;

        deathVFX.Play();
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(deathSFX);

        LoadFirstLevel();
    }

    private void Eject()
    {
        state = States.Alive;
    }

    private void Rotate90Degrees()
    {
        transform.Rotate(new Vector3(0, 90, 0));
    }

    private void LoadNextLevel()
    {

        FindObjectOfType<LevelLoader>().LoadNextScene();
    }

    private void LoadFirstLevel()
    {
        //FindObjectOfType<LevelLoader>().LoadLevel1();
        FindObjectOfType<LevelLoader>().LoadCurrentLevel();
    }


}
