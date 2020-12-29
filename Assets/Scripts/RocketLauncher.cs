using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{

    [SerializeField] float mainTrust = 100f;
    [SerializeField] AudioClip rocketTrustSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] ParticleSystem rocketTrustVFX;
    [SerializeField] ParticleSystem deathVFX;

    //Constants
    const string FRIENDLY = "Friendly";
    const string FINISH = "Finish";
    const string EJECT = "Eject";

    //Member Variables
    Rigidbody rigidbody;
    AudioSource audioSource;
    enum States { Alive, Dying, Ejecting };
    States state = States.Alive;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.Alive)
        {
            RespondToTrustInput();
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


    void OnCollisionEnter(Collision collision)
    {

        if (state != States.Alive)  //ignore collisions
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case FRIENDLY:
                //do nothing
                break;
            case FINISH:
                //do nothing
                break;
            default:
                Die();
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == EJECT)
        {
            Eject();
        }

    }

    private void Eject()
    {
        state = States.Ejecting;
        audioSource.Stop();
        rocketTrustVFX.Stop();

        float ejectTrust = mainTrust * Time.deltaTime;
        rigidbody.AddRelativeForce(new Vector3(-1, -2, 0) * ejectTrust);

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

    }
}
