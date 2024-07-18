// 누가 누가 다를까? 에서 사용되는 스크립트
// 동물 이미지 배치, 구분기준 ( 깃발...  )등 배치하는 기능 가짐.
// 정답, 오답 판정 기능을 가지고 있음
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FindDifference : MonoBehaviour
{
    // 초기 시간, 종료 시간을 저장 할 변수
    private int startTime;
    private float endTime;

    // 시도 횟수를 저장 할 변수
    private int tryCount = 0;

    // 메시지 오브젝트 설정
    private GameObject msg_congrate;
    private GameObject msg_retry;

    // 문제 배치
    // 난수 생성을 통해 0 ~ 2 숫자 중 정답을 설정
    private int answer;

    void Start()
    {
        // 시작 시간 저장
        startTime = int.Parse(DateTime.Now.ToString("HHmmss"));

        msg_congrate = GameObject.Find("FindDiffCanvas/msg_congrate");
        msg_retry = GameObject.Find("FindDiffCanvas/msg_retry");

        // msg_congrate, msg_retry 오브젝트를 비활성화
        msg_congrate.SetActive(false);
        msg_retry.SetActive(false);

        // 난수 생성
        answer = UnityEngine.Random.Range(0, 3);


        // answer에 따라 FindDiffCanvas의 btn_option_1, btn_option_2, btn_option_3
        // 의 Rotation y 값을 180도로 설정하고, x 좌표의 부호를 반전시키다.
        GameObject btn_option_1 = GameObject.Find("FindDiffCanvas/btn_option_1");
        GameObject btn_option_2 = GameObject.Find("FindDiffCanvas/btn_option_2");
        GameObject btn_option_3 = GameObject.Find("FindDiffCanvas/btn_option_3");

        // 오브젝트를 찾았는지 확인
        if (btn_option_1 == null || btn_option_2 == null || btn_option_3 == null)
        {
            Debug.LogError("Can't find object");
            return;
        }

        // answer에 따라 각 버튼의 img의 Rotation x 값을 180도로 설정
        GameObject img_option_1 = GameObject.Find("FindDiffCanvas/btn_option_1/img_object");
        GameObject img_option_2 = GameObject.Find("FindDiffCanvas/btn_option_2/img_object");
        GameObject img_option_3 = GameObject.Find("FindDiffCanvas/btn_option_3/img_object");

        // 오브젝트를 찾았는지 확인
        if (img_option_1 == null || img_option_2 == null || img_option_3 == null)
        {
            Debug.LogError("Can't find object");
            return;
        }

        // answer에 따라 각 버튼의 img의 Rotation x 값을 180도로 설정하고, x position을 부호 반전
        switch (answer)
        {
            case 0:
                img_option_1.transform.Rotate(0, 180, 0);
                img_option_1.transform.localPosition = new Vector3(-img_option_1.transform.localPosition.x - 10, img_option_1.transform.localPosition.y, img_option_1.transform.localPosition.z);
                break;
            case 1:
                img_option_2.transform.Rotate(0, 180, 0);
                img_option_2.transform.localPosition = new Vector3(-img_option_2.transform.localPosition.x - 10, img_option_2.transform.localPosition.y, img_option_2.transform.localPosition.z);
                break;
            case 2:
                img_option_3.transform.Rotate(0, 180, 0);
                img_option_3.transform.localPosition = new Vector3(-img_option_3.transform.localPosition.x - 10, img_option_3.transform.localPosition.y, img_option_3.transform.localPosition.z); break;
        }

        // 각 버튼에 대한 이벤트 추가
        btn_option_1.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => CheckAnswer(0));
        btn_option_2.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => CheckAnswer(1));
        btn_option_3.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => CheckAnswer(2));
    }

    // 정답, 오답 판정
    public void CheckAnswer(int selected)
    {
        // 디버깅 메시지
        // Debug.Log("selected: " + selected + ", answer: " + answer);
        tryCount++;

        // 정답 판정    
        if (selected == answer)
        {
            // 정답 판정 시, msg_congrate 오브젝트를 활성화
            msg_congrate.SetActive(true);

            // msg_congrate 오브젝트를 1초 후 비활성화
            StartCoroutine(DisableMsgCongrate());

            IEnumerator DisableMsgCongrate()
            {
                yield return new WaitForSeconds(1.0f);
                msg_congrate.SetActive(false);
            }
            // 종료 시간 저장
            endTime = int.Parse(DateTime.Now.ToString("HHmmss"));

            // DB에 저장하는 함수 호출
            // attentionScore는 아직 미구현
            // CalculateProgressScore("sp", 1, startTime, endTime, tryCount, concentrationScore, attentionScore );

            // 게임 종료 코드 추가

        }
        // 오답 판정
        else
        {
            // 오답 판정 시, msg_retry 오브젝트를 활성화하고, 1초 후 비활성화
            msg_retry.SetActive(true);

            StartCoroutine(DisableMsgRetry());

            IEnumerator DisableMsgRetry()
            {
                yield return new WaitForSeconds(1.0f);
                msg_retry.SetActive(false);
            }
        }
    }

}
