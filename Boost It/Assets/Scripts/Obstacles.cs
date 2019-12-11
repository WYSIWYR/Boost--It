using UnityEngine;

[DisallowMultipleComponent]
public class Obstacles : MonoBehaviour
{
    [SerializeField] Vector3 moveVector = new Vector3(10f, 10f, 10f);   //움직일 위치를 설정(Inspector창에서 개별 설정 가능)

    Vector3 startPos;   //시작 위치

    [Range(0, 1)] float moveFactor; //움직인 정도 (0%~100%)
    float period = 2f; //startPos에서 moveVector로 왕복하는 주기

    // Start is called before the first frame update
    void Start()
    {
        //현재 위치를 시작점으로 설정
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //period를 [SerializeField]로 선언해 Inspector창에서 사용할 경우 0이하의 값이 되면 함수 종료
        //if (period <= Mathf.Epsilon) { return; }
        
        float shuttle = Time.time / period; //왕복한 정도 (현재 시간 / 주기)

        const float tau = Mathf.PI * 2; //상수 2 PI 라디안
        //tau에 shuttle 값을 곱해 현재 각이 몇도인지 구하기
        //Sin 함수를 사용해 각에 따라 -1~1 사이의 값을 반환
        float rawSinWave = Mathf.Sin(shuttle * tau);

        moveFactor = (rawSinWave / 2f) + 0.5f;  //rawSinWave를 moveFactor의 범위에 맞게 바꾸기
        Vector3 offset = moveVector * moveFactor; //moveVector에서 moveFactor(%)만큼의 값 가져오기
        transform.position = startPos + offset; //현재 위치에서 offset만큼 움직이기
    }
}
