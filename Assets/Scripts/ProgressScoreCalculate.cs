// 진척도 계산 및 DB 저장을 위한 스크립트
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO
// 1. 게임 종료 시 진척도 계산 - 완료
// 2. 계산된 진척도를 DB의 스테이지별 진척도에 갱신 - 완료
// 3. 스테이지 렌더링 시 레벨별 별 활성화, 비활성화 시기 - 스테이지 렌더링
// 4. 레벨 선택 하단에 진척도 바 표시?


public class ProgressScoreCalculate : MonoBehaviour
{
    // 기본 진척도 가져와서 변수 저장
    private int initialProgressScore;

    // 매개변수로 게임명(스테이지), 레벨, 시작시간, 종료시간, 시도 횟수 받아와서 진척도 계산해서 DB에 저장
    // 진척도는 기본 유저 진척도 - (( 종료시간 - 시작시간 ) / 10 ) - ( (시도 횟수 - 1) * 5 )
    public void CalculateProgressScore(string gameName, int level, float startTime, float endTime, int tryCount, float concentrationScore)
    {
        /*UserDataManager.Instance.GetGameData("게임명").ContinueWith(task =>
        {
                var (var1, var2, var3) = task.Result;
        }
        only need gen_prog

        var1 = level
        var2 = star
        var3 = progressScore
        */

        // 진척도 계산
        int playTime = (int)(endTime - startTime);
        int tryNumber = tryCount - 1;
        int newProgressScore = initialProgressScore - (playTime / 10) - (tryNumber * 5);
        float correctScore = 1 / tryCount;
        // fg의 별을 찾아줘는 답이 여러 개라 정확도 계산 다름.
        if (gameName == "fg" && level == 0)
        {
            correctScore = 3 / tryCount;
        }

        // 해당 회차 정보를 훈련 저장
        // Save game session -> need to add attention score.
        // LocalDataManager.Instance.AddGameSession( gameName , DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"), level, newProgressScore, tryCount, 1 / (tryCount), playTime, concentrationScore);


        /*
        progressScore += newProgressScore ;
        if (progressScore >= 350)
        {
            if(level == 3)
            {
                return;
            }
            level += 1;
            progressScore = 0;
        }
        */

        /*UserDataManager.Instance.UpdateLevel();
            update level for the game
        void UpdateLevel(string game, int level, int stars, int prog)

        0 ~ 50 : 0 star
        51 ~ 150 : 1 star
        150 ~ 250 : 2 star
        251 ~ 350 : 3 star

        350점 되면 다음 레벨 해금, prog 0으로 초기화
        */


    }

}
