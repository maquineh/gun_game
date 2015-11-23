using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameUI : MonoBehaviour {

    public GameObject gameOverUI;
    public Text txtGameOver;
    public Button btnGameOver;
    public Image fadeImage;

	// Use this for initialization
	void Start () {
        gameOverUI.SetActive(false);
        FindObjectOfType<Player>().OnDeath += OnGameOver;
	}
	
	// Update is called once per frame
	void OnGameOver () {
        StartCoroutine(fadeCor(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
	}

    // UI Input
    public void StartNewGame()
    {
        Application.LoadLevel("Scene_Game");
    }

    IEnumerator fadeCor(Color de, Color para, float time) {
        float speed = 1 / time;
        float perc = 0;

        while(perc < 1){
            perc += Time.deltaTime * speed;
            fadeImage.color = Color.Lerp(de, para, perc);
            yield return null;
        }
    }
}
