using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // 스파크 프리팹을 저장할 변수
    public GameObject sparkEffect;

    // 충돌이 시작할 때 발생하는 이벤트
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 게임오브젝트의 태그값 비교
        if(collision.collider.tag == "BULLET")
        {
            // 스파크 효과 함수 호출
            ShowEffect(collision);
            // 충돌한 게임 오브젝트 삭제
            // Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision col)
    {
        // 충돌 지점의 정보를 추출. [0]인 이유 - 구조가 새로운 충돌이 더 있을 때는 그 앞 전에 있었던 충돌 지점의 대한 정보를 담는 배열 인덱스가 앞으로 밀리고 새 인덱스가 [0]번부터 추가되는 방식.
        ContactPoint contact = col.contacts[0];
        // 법선 벡터가 이루는 회전각도를 추출. -Vector3.forward == Vector3.back

        /*
            '오일러 각'은 수학자 오일러가 고안한 것으로 '3차원 공간의 절대 좌표를 기준으로 물체가 얼마나 회전했는지 측정하는 방식'이다. 오일러 각을 이용한 회전 방식 X, Y, Z 축을 차례대로 회전을 시키는 것으로
            '회전'하는 동안 X, Y, Z 축 중 '2개의 축이 겹쳐졌을 때 어느 축으로도 회전되지 않고 잠기는 현상'이 발생하는데, 이 현상을 '짐벌락(Gimbal Lock)'이라고 한다.

            이러한 문제점을 해결하기 위해 쿼터니언이라는 대안이 제시됬다. 앞서 언급했듯이 쿼터니언은 '4차원 복소수'로서 '오일러 각이 각축에 대해 차례대로 회전'시켰다면 '쿼터니언은 세 개의 축을 동시에 회전'시켜
            '짐벌락 현상을 방지'한다. 따라서 '짐벌락을 방지'하기 위해 '유니티는 모든 객체의 회전(Rotation)을 쿼터니언을 이용해 처리'한다.
        */
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        // 스파크 효과를 생성
        GameObject spark = Instantiate(sparkEffect, contact.point + (-contact.normal * 0.05f), rot);

        // 스파크 효과의 부모를 드럼통 또는 벽으로 설정. 만약 움직이는 오브젝트인 경우엔 이펙트 효과(스파크, 탄흔)가 위치를 따라와야 하기 때문에  
        spark.transform.SetParent(this.transform);
    }
}
