using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public GameObject projectilePrefab;
    public ParticleSystem damageEffect;
    public ParticleSystem healthEffect;

    public static int level = 1;
    public Image boneBG;
    public Image boneImage;
    public int maxHealth = 5;
    public Text scoreText;
    public Text ammoText;
    public Text boneText;
    public GameObject levelText;
    public GameObject loseText;
    public GameObject winText;
    public GameObject boneParent;
    public static int score = 0;
    public float timeInvincible = 2.0f;
    public int bones = 0;
    public int cogs;
    public int health { get { return currentHealth; } }
    int currentHealth;
    bool gameOver;
    bool isInvincible;
    float invincibleTimer;
    public AudioClip bgMusic;
    public AudioClip cogThrow;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip fixSound;
    public AudioClip collectClip;
    public AudioClip walking;
    public AudioClip bark;
    public AudioClip croak;
    AudioSource audioSource;
    AudioSource musicSource;
    Rigidbody2D rigidbody2d;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    float horizontal;
    float vertical;

    // Start is called before the first frame update
    void Start()
    {
        
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        musicSource = GetComponent<AudioSource>();
        boneText.text = bones.ToString() + "/5";
        scoreText.text = "Robots Fixed: " + score.ToString();
        currentHealth = maxHealth;
        boneBG.enabled = false;
        boneImage.enabled = false;
        boneText.enabled = false;
        boneParent.SetActive(false);
        levelText.SetActive(false);
        loseText.SetActive(false);
        winText.SetActive(false);
        gameOver = false;
        musicSource.clip = bgMusic;
        musicSource.Play();
        musicSource.loop = true;
        cogs = 4;
        ammoText.text = cogs.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ammoText.text = cogs.ToString();
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
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
         
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (hit.collider != null)
                {
                    NonPlayerCharacter noncharacter = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (noncharacter.gameObject.CompareTag("Jambi"))
                    {
                        PlaySound(croak);
                        if (score == 6)
                        {
                            SceneManager.LoadScene("Scene2");
                            loseText.SetActive(false);
                            winText.SetActive(false);
                            level++;
                        }
                    }
                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                    }
                    if (character.gameObject.CompareTag("Fido"))
                    {
                        PlaySound(bark);
                        boneParent.SetActive(true);
                        boneBG.enabled = true;
                        boneImage.enabled = true;
                        boneText.enabled = true;
                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                if (level == 1)
                {
                    score = 0;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                if (level == 2)
                {
                    score = 6;
                    scoreText.text = "Robots Fixed: " + score.ToString();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                 
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlaySound(walking);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlaySound(walking);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlaySound(walking);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlaySound(walking);
        }
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
            damageEffect.Play();
            PlaySound(hitSound);
        }
        if (amount > 0)
        {
            healthEffect.Play();
        }
        if (health <= 1)
        {
            Destroy(rigidbody2d);
            loseText.SetActive(true);
            gameOver = true;
            musicSource.clip = loseSound;
            musicSource.Play();
            musicSource.loop = false;
        }
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        
    }

    public void ChangeScore(int scoreAmount)
    {
        score = score + scoreAmount;
        scoreText.text = "Robots Fixed: " + score.ToString();
        PlaySound(fixSound);
        if (score == 6)
        {
            levelText.SetActive(true);
            gameOver = true;
        }
        if (score == 12)
        {
            if (bones == 5) 
            {
                musicSource.clip = winSound;
                musicSource.Play();
                musicSource.loop = false;
                gameOver = true;
                winText.SetActive(true);

                Destroy(rigidbody2d);
            }
        }
    }

    public void ChangeSpeed(float speedAmount)
    {
        speed = speed + speedAmount;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void BoneCount(int bone)
    {
        bones = bones + bone;
        boneText.text = bones.ToString() + "/5";
        ChangeScore(0);
    }
    void Launch()
    {
        if (cogs >= 1)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);
            cogs--;
            animator.SetTrigger("Launch");
            PlaySound(cogThrow);
        }
       
    }
    public void OnCollisionEnter2D(Collision2D ammo)
    {
        if (ammo.gameObject.CompareTag("CogPickup"))
        {
            PlaySound(collectClip);
            Destroy(ammo.collider.gameObject);
            cogs += 4;
        }
    }

}


