using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class FindBoxCanvas : MonoBehaviour
{
    // 초기 시간, 종료 시간을 저장 할 변수
    private int startTime;
    private float endTime;

    // 시도 횟수를 저장 할 변수
    private int tryCount = 0;

    // 이미지 오브젝트 설정
    private GameObject msg_congrate;
    private GameObject msg_retry;

    public AudioSource backgroundMusicSource;

    public AudioClip failSound; // 실패 시 재생할 음성

    public AudioClip successSound; // 성공 시 재생할 음성

    public string nextSceneName; // 전환할 씬 이름

    private AudioSource audioSource;


    // 난수 들어갈 변수
    // answer에는 정답이, location에는 랜덤 위치 용 정수가 들어감
    private int answer = 1;
    private int animal;
    private int location;

    // 씬의 오브젝트들을 저장 할 변수
    private GameObject btn_option_1;
    private GameObject btn_option_2;
    private GameObject btn_option_3;


    private GameObject img_answer_1;
    private GameObject img_answer_2;
    private GameObject img_answer_3;
    private GameObject img_answer_4;

    


    // btn_option_*의 위치를 위해 더할 값을 저장 할 배열
    private Vector3[] offset = new Vector3[3] { new Vector3(2, -2, 0), new Vector3(-2, 0.5f, 0), new Vector3(6.3f, 0, 0) };

    void Start()
    {
        // 시작 시간 저장
        startTime = int.Parse(DateTime.Now.ToString("HHmmss"));

        Debug.Log("startTime: " + DateTime.Now.ToString("HHmmss"));

        // 메시지 오브젝트 설정
        msg_congrate = GameObject.Find("FindBoxCanvas/msg_congrate");
        msg_retry = GameObject.Find("FindBoxCanvas/msg_retry");

        // msg_congrate, msg_retry 오브젝트를 비활성화
        msg_congrate.SetActive(false);
        msg_retry.SetActive(false);

        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();

        GameObject bgmObject = GameObject.Find("AudioManager");
        backgroundMusicSource = bgmObject.GetComponent<AudioSource>();



        // 난수 생성
        animal = UnityEngine.Random.Range(0, 4);
        location = UnityEngine.Random.Range(0, 2);


        // 씬의 오브젝트를 불러옴
        btn_option_1 = GameObject.Find("FindBoxCanvas/btn_option_1");
        btn_option_2 = GameObject.Find("FindBoxCanvas/btn_option_2");
        btn_option_3 = GameObject.Find("FindBoxCanvas/btn_option_3");

        img_answer_1 = GameObject.Find("FindBoxCanvas/img_sketch_background/img_answer_1");
        img_answer_2 = GameObject.Find("FindBoxCanvas/img_sketch_background/img_answer_2");
        img_answer_3 = GameObject.Find("FindBoxCanvas/img_sketch_background/img_answer_3");
        img_answer_4 = GameObject.Find("FindBoxCanvas/img_sketch_background/img_answer_4");


        // answer에 따라 나머지 img_answer_의 이미지를 비활성화
        switch (animal)
        {
            case 0:
                img_answer_2.SetActive(false);
                img_answer_3.SetActive(false);
                img_answer_4.SetActive(false);
                break;
            case 1:
                img_answer_1.SetActive(false);
                img_answer_3.SetActive(false);
                img_answer_4.SetActive(false);
                break;
            case 2:
                img_answer_1.SetActive(false);
                img_answer_2.SetActive(false);
                img_answer_4.SetActive(false);
                break;
            case 3:
                img_answer_1.SetActive(false);
                img_answer_2.SetActive(false);
                img_answer_3.SetActive(false);
                break;
        }

        // 오브젝트를 찾았는지 확인
        if (btn_option_1 == null || btn_option_2 == null || btn_option_3 == null)
        {
            Debug.LogError("Can't find object");
            return;
        }

        // answer에 따라 각 버튼의 위치를 변경
        switch (location)
        {
            case 0:
                btn_option_1.transform.position += offset[2];
                btn_option_2.transform.position += offset[1];
                btn_option_3.transform.position += offset[0];
                break;
            case 1:
                btn_option_1.transform.position += offset[1];
                btn_option_2.transform.position += offset[2];
                btn_option_3.transform.position += offset[0];
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

        // 시도 횟수 증가
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
            ProgressScoreManager.Instance.CalculateProgressScore("pc", 1, startTime, endTime, tryCount);

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
