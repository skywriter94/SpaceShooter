using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable] // 클래스는 System.Serializable이라는 어트리뷰트(Attribute, 속성)를 명시해야 인스펙터 뷰에 노출됨
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}



public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    // 접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용, 속성(Attribute) 사용. 변수가 private를 유지한 채 인스펙터에 노출하게 할 수 있음.
    [SerializeField] private Transform tr; // 약어를 쓰장!
   
    // 이동 속도 변수(public으로 선언되어 Inspector에 노출됨)
    public float moveSpeed = 10.0f; // 만약, 인스펙터 뷰에서 값을 20으로 설정하면 속도는 20임. 고로 인스펙터 뷰에서 설정한게 우선 순위가 높음.

    // 회전 속도 변수
    public float rotSpeed = 80.0f;

    // 인스펙터 뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;

    [HideInInspector]     // 인스펙터 뷰에서 감춤. (== [System.NonSerialized] 도 같은 의미.)
    public Animation anim;     // Animation 컴포넌트를 저장하기 위한 변수.

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        moveSpeed = GameManager.instance.gameData.speed;
    }

    void Start()
    {
        // 스크립트가 실행된 후 처음 수행되는 Start 함수에서 Transform 컴포넌트를 할당
        tr = GetComponent<Transform>();
        // ==> tr = this.gameObject.GetComponent<Transform>(); "이 스크립트가 포함된 게임오브젝트가 가진 여러 컴포넌트 중에서 transform 컴포넌트를 추출해서 tr 변수에 저장하라"

        // Animation 컴포넌트를 변수에 할당
        anim = GetComponent<Animation>();
        // Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        anim.clip = playerAnim.idle;
        anim.Play();

        // 불러온 데이터 값을 moveSpeed에 적용
        moveSpeed = GameManager.instance.gameData.speed;
    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        //Debug.Log("h = " + h.ToString()); // ToString() == 문자열 변환 
        //Debug.Log("v = " + v.ToString());

        // 전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준 좌표). 대각선 이동 시 대각선 벡터 값이 생기는데 이때 속도는 대략 1.4(루트2가)가 된다. 속도가 빨라지는 것을 방지하기 위해 벡터의 방향 성분만 추출하기 위해 normalized 사용함.
        //vector의 크기는 Vector3.Magnitude 함수를 이용해 가져올 수 있다. p131 확인.

        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self); // Space.Self = 로컬 좌표계 기준, Space.World = 월드 좌표계 기준.
        // transform.Translate(Vector3.forward  * 10); // 매 프레임마다 10 유닛만큼씩 이동
        // transform.Translate(Vector3.forward * Time.deltaTime * 10); // 매 초마다 10 유닛만큼씩 이동

        // Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);

        // 키보드 입력 값을 기준으로 동작할 애니메이션 수행
        if (v >= 0.1f)
        {
            anim.CrossFade(playerAnim.runF.name, 0.3f); // 전진 애니메이션
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f); // 전진 애니메이션
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f); // 전진 애니메이션
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f); // 전진 애니메이션
        }
        else
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f); // 전진 애니메이션
        }
    }
}