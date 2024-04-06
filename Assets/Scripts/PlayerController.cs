
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;


public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    private Animator _animator;

    public TextMeshProUGUI puanTxt;
    public TextMeshProUGUI coinTxt;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI gameOverHighScore;


    public GameObject playerObject;
    public GameObject gameOverPanel;
    public GameObject model;

    private UI uiManager;

    private int bulunanSerit = 1; // 0-sol 1-orta 2-sað
    public float seritMesafesi = 3;
    public float speed = 15;
    public float jumpForce = 10f;
    public float jumpDuration = 0.2f; 
    public int toplamCan = 3;
    private int varolanCan;
    public bool isJumping = false;
    public bool comingDown = false;
    private bool isSliding = false;
    public float minSpeed = 15;
    public float maxSpeed = 20;
    private bool invincible = false;
    static int blinkingValue;
    public float effectDuration; 
    private float scoreDistance;
  
    public int puan = 0;
    int coin = 0;
    private int highScore = 0;

    public Color normalColor = Color.black;
    public Color highScoreColor = Color.red;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = playerObject.GetComponent<Animator>();
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        varolanCan = toplamCan;
        speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        uiManager = FindObjectOfType<UI>();
        puan = 0;
        scoreDistance = transform.position.z;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Update()
    {
        Move();
        UpdateSpeed();
        UpdateScore();
        MesafePuaniniGuncelle();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (invincible)
            return;

        switch (other.gameObject.tag)
        {
            case "Engel":
                varolanCan--;
                uiManager.UpdateLives(varolanCan);
                speed = 0;
                if (varolanCan <= 0)
                {
                    gameOverPanel.SetActive(true);
                    
                }
                else
                {
                    other.gameObject.SetActive(false);
                    StartCoroutine(CanEfekt(effectDuration));
                }
                break;
            case "Coin":
                coin++;
                puan += 50;
                uiManager.UpdateCoins(coin);
                uiManager.UpdateScore(puan);
                
                other.transform.parent.gameObject.SetActive(false);
                break;
        }
    }

    private void UpdateSpeed()
    {
        if (speed < maxSpeed)
        {
            speed += 0.1f * Time.deltaTime;
        }
    }
    private void UpdateScore()
    {
        puanTxt.text = "Score: " + puan.ToString(); //Anlýk puaný günceller
        if (varolanCan <= 0)
        {
            Time.timeScale = 0; // Oyunu durdur
            gameOverPanel.SetActive(true);
            gameOverScore.text = "SCORE: " + puan.ToString();
        }
        if (puan > highScore)
        {
            highScore = puan;
            PlayerPrefs.SetInt("HighScore", highScore);//High score kaydetme islemi
            //puanTxt.color = highScoreColor;
        }
        if (puan == highScore)
        {
            puanTxt.color = highScoreColor;
        }
        else
        {
            puanTxt.color = normalColor; // Deðilse normal rengini kullan
        }
        gameOverHighScore.text ="HIGH SCORE: " + highScore.ToString();
    }
   
    private void MesafePuaniniGuncelle()
    {
        float katEdilenMesafe = transform.position.z - scoreDistance; // Kat edilen mesafe
        int eklenenPuan = Mathf.FloorToInt(katEdilenMesafe); // Kat edilen mesafeyi tam sayýya çevirdim

        if (eklenenPuan > 0)
        {
            puan += eklenenPuan;
            scoreDistance = transform.position.z;
        }
    }
    void Move()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            bulunanSerit++;
            if (bulunanSerit == 3)
                bulunanSerit = 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            bulunanSerit--;
            if (bulunanSerit == -1)
                bulunanSerit = 0;
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (bulunanSerit == 0)
        {
            targetPosition += Vector3.left * seritMesafesi;
        }
        else if (bulunanSerit == 2)
        {
            targetPosition += Vector3.right * seritMesafesi;
        }
        targetPosition.y = transform.position.y;// Karakterin yüksekliðinin korunmasý

        // Karakterin z ekseninde ilerlemesi
        targetPosition.z += Time.deltaTime * speed; // Karakterin hýzý
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);

        //-------------------Jump--------------------------
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            if (isJumping == false)
            {
                isJumping = true;
                if (!playerObject.activeInHierarchy)
                    playerObject.SetActive(true); // Eðer playerObject devre dýþý býrakýlmýþsa etkinleþtir
                playerObject.GetComponent<Animator>().Play("Jump");
                StartCoroutine(JumpSettings());
            }
        }

        if (isJumping == true)
        {
            if (comingDown == false)
            {
                transform.Translate(Vector3.up * Time.deltaTime * 5, Space.World);
            }
            if (comingDown == true)
            {
                transform.Translate(Vector3.up * Time.deltaTime * -5, Space.World);
            }
        }
        //-------------------Slide--------------------------
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isSliding == false)
            {
                isSliding = true;
                if (!playerObject.activeInHierarchy)
                    playerObject.SetActive(true);
                playerObject.GetComponent<Animator>().Play("Slide");
                StartCoroutine(SlideSettings());
            }
        }
    }
    IEnumerator JumpSettings()
    {
        isJumping = true;
        _animator.Play("Jump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(jumpDuration);
        yield return new WaitForSeconds(0.5f);
        comingDown = true;
        yield return new WaitForSeconds(0.1f);
        isJumping = false;
        comingDown = false;
        _animator.Play("Run");
    }

    IEnumerator SlideSettings()
    {
        isSliding = true;
        _animator.Play("Slide");
        yield return new WaitForSeconds(0.45f);
        isSliding = false;
        _animator.Play("Run");
    }
    IEnumerator CanEfekt(float time)
    {
        invincible = true;
        float timer = 0;
        float currentBlink = 1f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        bool enabled = false;
        yield return new WaitForSeconds(1f);
        speed = minSpeed;


        while (timer < time && invincible)
        {
            model.SetActive(enabled);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }
        model.SetActive(true);
        invincible = false;
    }

}




