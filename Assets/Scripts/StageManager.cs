using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
    public static StageManager Instance;
    public float flickerDuration = 0.05f;
    public GameObject sphere;
    public float sphereSize = 1f;
    public Animation sphereExpansion;
    public GameObject filter;
    Image filterImage;
    public GameObject black;
    public GameObject white;
    public GameObject quote;
    public GameObject question;
    public GameObject rings;
    public GameObject chasers;
    CameraScript cam;
    public float zoneKnockForce;
    Escaper escaper;
    Player player;
    Color blackColor = new Color(0, 0, 0, 192 / 255f);
    Color redColor = Color.red;
    public int escaperCount = 0;

    void Awake() {
        Instance = this;
    }
    void Start () {
        escaper = Escaper.Instance;
        player = Player.Instance;
        filterImage = filter.GetComponent<Image>();
        sphere.transform.localScale = new Vector3(1, 1, 1) * sphereSize;
        cam = Camera.main.GetComponent<CameraScript>();
        StartCoroutine(OpeningScene());
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {

        }
    }

    public void EscaperHit() {
        escaperCount++;
        Debug.Log("count " + escaperCount);
        if (escaperCount == 1) {
            escaper.EscapeStage();
            StartCoroutine(Flicker(flickerDuration * 2, true));
            foreach (Chaser chaser in Chaser.chasers) {
                chaser.chasing = true;
                chaser.followDrag = 0.98f;
            }
        } else if (escaperCount < 5) {
            StartCoroutine(Flicker(flickerDuration, true));
        } else if (escaperCount == 5) {
            StartCoroutine(Flicker(flickerDuration, false));
            StartCoroutine("DarkZone");
        } else if (escaperCount < 27) {
            StartCoroutine(Flicker(flickerDuration, true));
            filterImage.color = Color.Lerp(blackColor, redColor, (escaperCount - 5f) / 20f);
        } else if (escaperCount < 34) {
            StartCoroutine(Flicker(flickerDuration, true));
        } else if (escaperCount== 35) {
            StartCoroutine(Restore());
        }
    }

    public void EndGame() {
        StartCoroutine(End());
    }

    IEnumerator End() {
        filterImage.color = Color.black;
        filter.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        filter.SetActive(false);
        white.SetActive(true);
        yield return new WaitForSecondsRealtime(4f);
        question.SetActive(true);
        yield return new WaitForSecondsRealtime(4f);
        Application.Quit();
    }

    IEnumerator Restore() {
        escaper.EndStage();
        player.Pause();
        rings.SetActive(true);
        chasers.SetActive(false);
        cam.smoothTime = 0f;
        cam.transform.position = new Vector3(0, 15.5f, -55f);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(5f);
        filterImage.color = Color.white;
        Time.timeScale = 1f;
        player.rb.velocity = Vector3.zero;
        player.pause = true;
        player.rb.velocity = Vector3.zero;
        yield return new WaitForSecondsRealtime(2f);
        GetComponent<AudioSource>().Play();
        player.pause = false;
        cam.smoothTime = 0.2f;
        float t = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - t < 7f) {
            filterImage.color = Color.Lerp(Color.white, new Color(0,0,0,0), (Time.realtimeSinceStartup - t) / 7f);
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator RedFlicker(float sec) {
        Time.timeScale = 0f;
        black.SetActive(true);
        yield return new WaitForSecondsRealtime(sec);
        black.SetActive(false);
        white.SetActive(true);
        yield return new WaitForSecondsRealtime(sec);
        white.SetActive(false);

    }

    IEnumerator OpeningScene() {
        Time.timeScale = 0f;
        Text quoteText = quote.GetComponent<Text>();
        Image blackImage = black.GetComponent<Image>();
        black.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        quote.SetActive(true);
        float t = Time.realtimeSinceStartup;
        //while (Time.realtimeSinceStartup - t < 1f) {
        //    quoteText.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(1, 1, 1, 1), Time.realtimeSinceStartup - t);
        //    yield return new WaitForEndOfFrame();
        //}
        yield return new WaitForSecondsRealtime(5f);
        t = Time.realtimeSinceStartup;
        Time.timeScale = 1f;
        while (Time.realtimeSinceStartup - t < 3f) {
            quoteText.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), (Time.realtimeSinceStartup - t)/3f);
            blackImage.color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), (Time.realtimeSinceStartup - t) /3f);
            yield return new WaitForEndOfFrame();
        }
        quote.SetActive(false);
        blackImage.color = Color.black;
        black.SetActive(false);
    }

    IEnumerator Flicker(float sec, bool resume) {
        Time.timeScale = 0f;
        black.SetActive(true);
        yield return new WaitForSecondsRealtime(sec);
        black.SetActive(false);
        white.SetActive(true);
        yield return new WaitForSecondsRealtime(sec);
        white.SetActive(false);

        if (resume) {
            Time.timeScale = 1f;
        } else {
            player.transform.position = new Vector3(player.transform.position.x, 0.5f, player.transform.position.z );
        }
    }

    IEnumerator DarkZone() {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        sphereExpansion.Play();
        AnimationState animState = sphereExpansion["SphereExpansion"];
        bool isPlaying = true;
        float currentTime = Time.realtimeSinceStartup;
        float deltaTime = 0f;
        float progressTime = 0f;
        while (isPlaying) {
            if (Physics.CheckSphere(cam.transform.position, 5f)) {
                ResumeFromDarkZone();
            }
            deltaTime = Time.realtimeSinceStartup - currentTime;
            currentTime = Time.realtimeSinceStartup;
            progressTime += deltaTime;
            animState.normalizedTime = progressTime / animState.length;
            sphereExpansion.Sample();
            if (progressTime >= animState.length) {
                isPlaying = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void ResumeFromDarkZone() {
        Time.timeScale = 1f;
        StopCoroutine("DarkZone");
        sphereExpansion.Stop();
        sphere.SetActive(false);
        filter.SetActive(true);
        player.rb.velocity = Vector3.zero;
        Quaternion rot = player.transform.rotation;
        rot.x = 0;
        rot.z = 0;
        rot.w = 0;
        player.transform.rotation = rot;
        Vector3 force = (player.transform.position - escaper.transform.position).normalized * zoneKnockForce;
        player.Bounce(force);
        escaper.ChaseStage();
        black.GetComponent<Image>().color = Color.red;
        foreach (Chaser chaser in Chaser.chasers) {
            chaser.gameObject.SetActive(false);
        }
    }
}
