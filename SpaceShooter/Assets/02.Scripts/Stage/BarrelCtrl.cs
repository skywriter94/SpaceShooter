using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BarrelCtrl : MonoBehaviour
{
    // 폭발 효과 프리팹을 저장할 변수
    public GameObject expEffect;
    // 찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;
    // 드럼통의 텍스처를 저장할 배열
    public Texture[] textures;

    // 총알이 맞은 횟수
    private int hitCount = 0;

    // Rigidbody 컴포넌트를 저장할 변수
    // private Rigidbody rb;

    // MeshFilter 컴포넌트를 저장할 변수
    private MeshFilter meshFilter;
    // MeshRenderer 컴포넌트를 저장할 변수
    private MeshRenderer _renderer;
    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource _audio;

    // 폭발 반경    
    public float expRadius = 10.0f;
    // 폭발음 오디오 클립
    public AudioClip expSfx;

    // Shake 클래스를 저장할 변수
    public Shake shake;

    void Start()
    {
        // Rigidbody 컴포넌트를 추출해 저장
        // rb = GetComponent<Rigidbody>();

        // MeshFilter 컴포넌트를 추출해 저장
        meshFilter = GetComponent<MeshFilter>();

        // MeshRenderer 컴포넌트를 추출해 저장
        _renderer = GetComponent<MeshRenderer>();

        // AudioSource 컴포넌트를 추출해 저장
        _audio = GetComponent<AudioSource>();

        // Shake 스크립트를 추출
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        // 난수를 발생시켜 불규칙적인 텍스처를 적용
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];
    }

    // 충돌이 발생했을 때 한 번 호출되는 콜백 함수
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 게임오브젝트의 태그를 비교
        if(collision.collider.CompareTag("BULLET"))
        {
            // 총알의 충돌 횟수를 증가시키고 3발 이상 맞았는지 확인
            if(++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    // 폭발 효과를 처리할 함수
    void ExpBarrel()
    {
        // 폭발 효과 프리팹을 동적으로 생성. Quaternion.identity는 특정 회전 값 없이 생성하려는 프리팹 또는 3D 모델의 원래 회전 각도로 적용한다는 것으로 조금 다른 의미다.
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);

        // Rigidbody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함. 튀어오르는 효과를 주기 위해 가볍게 함.
        // rb.mass = 1.0f;
        // 위로 솟구치는 힘을 가함. 폭발했을 때 물리적으로 튀어오르는 효과를 줄 수 있게.
        // rb.AddForce(Vector3.up * 1000.0f);

        // 폭발력 생성
        IndirectDamage(transform.position);

        // 난수를 발생. Random.Range 함수의 반환 값 범위는 실수형 일때는 min과 max 값을 포함한 난수 발생, 정수형 일때는 min부터 max - 1이다. 예를 들어 1.0f부터 10.0이면 1.0~10.0까지 난수 범위, 1, 10일때는 1~9까지.
        int idx = Random.Range(0, meshes.Length);
        // 찌그러진 메쉬를 적용. 찌그러진 메쉬 모양에 맞춰 콜라이더도 변경.
        meshFilter.sharedMesh = meshes[idx];
        GetComponent<MeshCollider>().sharedMesh = meshes[idx];

        // 폭발음 발생
        _audio.PlayOneShot(expSfx, 1.0f);

        // 셰이크 효과 호출
        StartCoroutine(shake.ShakeCamera(0.5f, 1f, 1f));
    }

    // 폭발력을 주변에 전달하는 함수
    void IndirectDamage(Vector3 pos)
    {
        // 주변에 있는 드럼통을 모두 추출
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8); // 레이어는 비트 연산 표기법을 사용해 8번째 레이어를 의미하는 1<<8을 사용한다. 1<<8은 2의 8승을 의미한다.
        // 즉, 10진수로 256(= 2의 8승)을 대입해도 같은 결과를 얻는다. 이처럼 레이어 인자에 10진수를 사용하지 않고 비트 연산 표기법을 사용하는 이유는 다음과 같이 논리연산이 가능하기 때문이다.
        // 1 << 8 | 1 << 9 : OR 연산, 8번 또는 9번 레이어
        // ~(1 << 8) : NOT 연산, 8번 레이어를 제외한 나머지 모든 레이어

        foreach(var col in colls)
        {
            // 폭발 범위에 포함된 드럼통의 rigidbody 컴포넌트 추출
            var _rb = col.GetComponent<Rigidbody>();
            // 드럼통의 무게를 가볍게 함
            _rb.mass = 1.0f;
            // 폭발력을 전달. Rigidbody.AddExplosionForce(횡 폭발력, 폭발원점, 폭발반경, 종 폭발력);
            _rb.AddExplosionForce(1200.0f, pos, expRadius, 1000.0f);
        }
    }
}
