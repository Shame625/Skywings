using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFaderIntro : MonoBehaviour
{

    public Image FadeImg;
    public Image[] FadeImgs;
    public float fadeSpeed = 1.5f;
    public bool sceneStarting = true;

    bool secondPart = false;


    void Start()
    {
        FadeImg = FadeImgs[0];
    }

    void Update()
    {
        // If the scene is starting...
        if (sceneStarting)
            // ... call the StartScene function.
            StartScene();
    }


    void FadeToClear()
    {
        // Lerp the colour of the image between itself and transparent.
        FadeImg.color = Color.Lerp(FadeImg.color, Color.clear, fadeSpeed * Time.deltaTime);
    }



    void StartScene()
    {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (FadeImg.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            FadeImg.color = Color.clear;
            FadeImg.enabled = false;

            // The scene is no longer starting.
            if (secondPart)
            {
                sceneStarting = false;
                gameObject.SetActive(false);
                SceneManager.LoadScene("mainMenu");
            }
            FadeImg = FadeImgs[1];
            secondPart = true;
        }
    }
}