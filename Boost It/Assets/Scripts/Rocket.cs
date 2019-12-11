using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 300f;    //한번에 회전하는 정도
    [SerializeField] float mainThrust = 2000f;  //Boost 출력
    [SerializeField] WaitForSeconds loadLevelDelay = new WaitForSeconds(1f); //새 level을 불러올 때 주는 지연시간
    [SerializeField] AudioClip mainEngine;  //Boost시 출력할 효과음
    [SerializeField] AudioClip success;     //성공지점에 도달할 때 출력할 효과음
    [SerializeField] AudioClip died;        //Player가 죽을 때 출력할 효과음
    [SerializeField] ParticleSystem engineParticle; //Boost시 출력할 효과
    [SerializeField] ParticleSystem successParticle;    //성공지점에 도달할 때 출력할 효과
    [SerializeField] ParticleSystem diedParticle;   //Player가 죽을 때 출력할 효과

    //Player가 가지고있는 RigidBody와 AudioSource를 저장할 변수
    Rigidbody rigidBody;
    AudioSource audioSource;
    //Scene에 있는 sceneLoader를 저장할 변수
    SceneLoader sceneLoader;  

    //bool collisionsDisable = false;   //Debug 모드에서 충돌체 판정을 할지 정하는 bool
    bool isFinish = false;  //게임이 끝났는지 확인하는 bool

    // Use this for initialization
    void Start()
    {
        //Player가 가지고있는 RigidBody와 AudioSource를 사용하기 위해 가져온다
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        //Scene에 sceneLoader가 있으면 가져온다.
        sceneLoader = FindObjectOfType<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        //게임이 끝나지 않았으면 RespondToThrust/Rotate를 실행한다
        if (!isFinish)
        {
            RespondToThrust();
            RespondToRotate();
        }

        //Debug 모드로 게임이 빌드 되면 true를 반환해 RespondToDebugKey가 실행된다
        //if (Debug.isDebugBuild)
        //    RespondToDebugKey();

    }

    //L키를 누르면 다음 level을 불러온다 C키를 누르면 충돌체 판정을 끄거나 킨다
    //private void RespondToDebugKey()
    //{
    //    if (Input.GetKeyDown(KeyCode.L))
    //        sceneHandler.LoadNextLevel();
    //    else if (Input.GetKeyDown(KeyCode.C))
    //        collisionsDisable = !collisionsDisable;
    //}

    private void RespondToThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Thrust();
        }

        else
        {
            StopThrust();
        }
    }

    private void Thrust()
    {
        //Player의 Local위치를 기준으로 위쪽 방향으로 힘을준다
        //Time.deltaTime은 1frame의 시간만큼의 값을 가진다(60fps일 경우 1/60)
        //Time.deltaTime을 곱하는 이유는 Update가 1frame마다 실행되기 때문에
        //1초에 여러번 실행되는 걸 방지하기 위해서이다
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        //AudioSource가 Play중이 아니면 mainEngine 효과음과 engineParticle 효과를 실행한다.
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            engineParticle.Play();
        }

    }

    //Player가 Space키를 때면 mainEngine 효과음과 engineParticle 효과를 중지한다.
    private void StopThrust()
    {
        audioSource.Stop();
        engineParticle.Stop();
    }

    private void RespondToRotate()
    {
    
        //1frame마다 회전하는 정도
        float rotationPerFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            ControlRotation(rotationPerFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            ControlRotation(-rotationPerFrame);
        }

    }

    private void ControlRotation(float rotationPerFrame)
    {
        rigidBody.freezeRotation = true;    //Player가 회전 조작을 할때는 외부에서의 물리 효과에 의해 회전되지 않도록 하기 위해 사용한다
        transform.Rotate(Vector3.forward * rotationPerFrame);
        rigidBody.freezeRotation = false;
    }

    //Player가 충돌체(Collsion)과 충돌하면 실행된다
    private void OnCollisionEnter(Collision collision)
    {
        if(isFinish) //|| collisionsDisable)    //todo Debug 모드를 사용할 떄 주석풀기
            return;
        
        //충돌체가 가지고있는 tag(Object가 가지는 식별표)에 따라 다른 함수를 실행한다.
        switch (collision.gameObject.tag)
        {
            case "Friendly":

                break;

            case "Finish":
                StartCoroutine(FininshLevel());
                break;

            default:
                StartCoroutine(FailLevel());
                break;
        }
    }
    
    //코루틴은 단일쓰레드를 사용하는 유니티에서 멀티테스킹을 구현하기 위해 사용한다.
    //코루틴은 yield를 만나면 현재 함수를 정지하고 빠져나간뒤 일정시간이 지난뒤 돌아온다
    //게임이 끝나면 모든소리를 끈후 success 효과음과 successParticle을 효과를 실행하고 loadLevelDelay만큼 대기시간을 가지고 다음 Level을 불러온다
    private IEnumerator FininshLevel()
    {
        isFinish = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticle.Play();
        yield return loadLevelDelay;
        sceneLoader.LoadNextLevel();
    }

    //Player가 죽으면 모든소리를 끈후 died 효과음과 diedParticle을 효과를 실행하고 loadLevelDelay만큼 대기시간을 가지고 다음 Level을 불러온다
    private IEnumerator FailLevel()
    {
        isFinish = true;
        audioSource.Stop();
        audioSource.PlayOneShot(died);
        diedParticle.Play();
        yield return loadLevelDelay;
        sceneLoader.LoadFirstLevel();
    }
}
