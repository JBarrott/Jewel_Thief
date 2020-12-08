using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public int maxScore = 0;
    public int maxCogs = 4;
    public int maxFlowers = 0;
    public int maxJewels = 0;
    public static int level = 1;
    private int win;

    public GameObject projectilePrefab;
    public ParticleSystem hitPrefab;
    public ParticleSystem healthPrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public int score { get { return currentScore; } }
    int currentScore;

    public int cogs { get { return currentCogs; } }
    int currentCogs;

    public int flowers { get { return currentFlowers; } }
    int currentFlowers;

    public int jewels { get { return currentJewels; } }
    int currentJewels;
    
    public Text scoreText;
    public Text GameOverText;
    public Text nameText;
    public Text flowersText;
    public Text cogsText;
    public Text jewelsText;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    public AudioClip musicClipThree;
    public AudioSource musicSource;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        currentScore = maxScore;
        currentCogs = maxCogs;
        currentFlowers = maxFlowers;
        currentJewels = maxJewels;

        scoreText.text = "Fixed Robots: " + currentScore;

        audioSource = GetComponent<AudioSource>();

        scoreText.text = "";
        GameOverText.text = "";
        nameText.text = "";
        flowersText.text = "";
        cogsText.text = "";
        jewelsText.text = "";

        musicSource.clip = musicClipOne;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (win == 3)
        {
            musicSource.clip = musicClipTwo;
                musicSource.Play();

                GameOverText.text = "You Win! Press R to Restart.";
                nameText.text = "Game created by Jordan Barrott";
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            if (currentCogs > 0)
            {
                Launch();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if (currentScore < 4)
                    {
                        character.DisplayDialog();
                    }

                    else
                    {
                        SceneManager.LoadScene("SecondScene");
                        level = level + 1;
                    }
                }
            }
        }

        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameOverText == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            if (level == 1)
            {
                SceneManager.LoadScene("PlayScene");
                level = level + 1;
            }
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            if (level == 2)
            {
                SceneManager.LoadScene("MainScene");
                level = level + 1;
            }
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.LoadScene("HomeScene");
            level = 1;
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        ChangeCogs(-1);

        animator.SetTrigger("Launch");
            PlaySound(throwSound);
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            ParticleSystem hitEffect = Instantiate(hitPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }

        if (amount > 0)
        {
            ParticleSystem healthEffect = Instantiate(healthPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth <= 0)
        {
            musicSource.clip = musicClipThree;
            musicSource.Play();
            GameOverText.text = "You Lose! Press R to Restart.";
            nameText.text = "Game created by Jordan Barrott";
            speed = 0.0f;
        }
    }

    public void ChangeScore(int amount)
    {
        currentScore += amount;
        scoreText.text = "Fixed Robots: " + currentScore + "/4";
        if (currentScore == 4)
        {
            if (level < 4)
            {
                GameOverText.text = "Talk to Jambi to visit Stage 2!";
                nameText.text = "Complete Task List to Win";
            }

            if (level == 4)
            {
                win = win + 1;
            }
        }
    }

    public void ChangeCogs(int amount)
    {
        currentCogs += amount;
        cogsText.text = "Cogs: " + currentCogs;
    }

    public void ChangeFlowers(int amount)
    {
        currentFlowers += amount;
        flowersText.text = "Flowers: " + currentFlowers + "/6";

        if (currentFlowers == 6)
        {
            win = win + 1;
        }
    }

    public void ChangeJewels(int amount)
    {
        currentJewels += amount;
        jewelsText.text = "Jewels: " + currentJewels + "/8";

        if (currentJewels == 8)
            {
                win = win + 1;
            }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
