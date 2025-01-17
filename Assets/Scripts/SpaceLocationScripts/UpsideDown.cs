// 누가누가거꾸로? 레벨에서 사용되는 스크립트
// 문제 배치와 정답, 오답 판정 기능을 가지고 있음
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class UpsideDown : MonoBehaviour
{
    // 초기 시간, 종료 시간을 저장 할 변수
    private int startTime;
    private float endTime;

    // 시도 횟수를 저장 할 변수
    private int tryCount = 0;

    // rotation 변경이 아닌, 뒤집힌 이미지로 이미지 소스 자체를 변경하도록 함
    // 이미지 오브젝트 설정
    public Sprite upsideDownImg;

    // 메시지 오브젝트 설정
    public GameObject msg_congrate;
    public GameObject msg_retry;

    public AudioSource backgroundMusicSource;

    public AudioClip failSound; // 실패 시 재생할 음성

    public AudioClip successSound; // 성공 시 재생할 음성

    public string nextSceneName; // 전환할 씬 이름

    private AudioSource audioSource;




    // 문제 배치
    // 난수 생성을 통해 0 ~ 2 숫자 중 정답을 설정
    public int answer;

    // Start is called before the first frame update
    void Start()
    {
        // 시작 시간 저장
        startTime = int.Parse(DateTime.Now.ToString("HHmmss"));

        msg_congrate = GameObject.Find("UpsideDownCanvas/msg_congrate");
        msg_retry = GameObject.Find("UpsideDownCanvas/msg_retry");

        // msg_congrate, msg_retry 오브젝트를 비활성화
        msg_congrate.SetActive(false);
        msg_retry.SetActive(false);

        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();

        GameObject bgmObject = GameObject.Find("AudioManager");
        backgroundMusicSource = bgmObject.GetComponent<AudioSource>();



        // 난수 생성
        answer = UnityEngine.Random.Range(0, 3);

        // answer에 따라 UpsideDownCanvas의 btn_option_1, btn_option_2, btn_option_3
        // 의 Rotation x 값을 180도로 설정
        GameObject btn_option_1 = GameObject.Find("UpsideDownCanvas/btn_option_1");
        GameObject btn_option_2 = GameObject.Find("UpsideDownCanvas/btn_option_2");
        GameObject btn_option_3 = GameObject.Find("UpsideDownCanvas/btn_option_3");

        // 오브젝트를 찾았는지 확인
        if (btn_option_1 == null || btn_option_2 == null || btn_option_3 == null)
        {
            Debug.LogError("Can't find object");
            return;
        }

        // answer에 따라 각 버튼의 img의 Rotation x 값을 180도로 설정
        switch (answer)
        {
            case 0:
                btn_option_1.GetComponent<UnityEngine.UI.Image>().sprite = upsideDownImg;
                break;
            case 1:
                btn_option_2.GetComponent<UnityEngine.UI.Image>().sprite = upsideDownImg;
                break;
            case 2:
                btn_option_3.GetComponent<UnityEngine.UI.Image>().sprite = upsideDownImg;
                break;
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
            msg_retry.SetActive(false);
            // 정답 판정 시, msg_congrate 오브젝트를 활성화
            msg_congrate.SetActive(true);

            // msg_congrate 오브젝트를 1초 후 비활성화
            StartCoroutine(DisableMsgCongrate());

            IEnumerator DisableMsgCongrate()
            {
                backgroundMusicSource.Stop();
                audioSource.clip = successSound;
                audioSource.Play();
                msg_congrate.SetActive(true);

                yield return new WaitForSeconds(successSound.length);
                
            }
            // 종료 시간 저장
            endTime = int.Parse(DateTime.Now.ToString("HHmmss"));

            // DB에 저장하는 함수 호출
            ProgressScoreManager.Instance.CalculateProgressScore("sp", 0, startTime, endTime, tryCount);

            // 게임 종료 코드 추가
            SceneManager.LoadScene(nextSceneName);

        }
        // 오답 판정
        else
        {
            msg_congrate.SetActive(false);

            // 오답 판정 시, msg_retry 오브젝트를 활성화하고, 1초 후 비활성화
            msg_retry.SetActive(true);

            StartCoroutine(DisableMsgRetry());

            IEnumerator DisableMsgRetry()
            {
                audioSource.clip = failSound;
                audioSource.Play();
                // 실패 음성 길이만큼 대기
                yield return new WaitForSeconds(1.0f);
                msg_retry.SetActive(false);
            }
        }
    }

}
