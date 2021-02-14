using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{        
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    //게임을 종료한다.
    public void ExitGame()
    {
        Application.Quit();
    }

    //Build를 할 때 Scene의 순서(BuildIndex)를 지정할 수 있다(0번 부터 시작)
    //현재 Level의 Scene을 불러온다.
    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //다음 Level을 불러온다
    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;   //현재 화면에서 작동하고 있는 Scene의 BuildIndex를 가져온다.
        int nextSceneIndex = currentSceneIndex + 1; //현재 Scene을 기준으로 다음 Scene을 불러온다

        //다음 BuildIndex가 총 BuildIndex가 클 경우 BuildIndex를 0으로 바꾼다
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
            nextSceneIndex = 0;

        SceneManager.LoadScene(nextSceneIndex);
    }
}
