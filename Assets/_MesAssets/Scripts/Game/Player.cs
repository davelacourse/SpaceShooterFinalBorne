using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour

{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private GameObject _laserPrefab = default;
    [SerializeField] private GameObject _tripleLaserPrefab = default;
    [SerializeField] private float _delai = 0.5f;
    [SerializeField] private int _viesJoueur = 3;
    [SerializeField] private bool _isTripleLaserActif = false;
    [SerializeField] private AudioClip _laserSound = default;
    [SerializeField] private AudioClip _endSound = default;
    [SerializeField] private GameObject _playerHurt1 = default;
    [SerializeField] private GameObject _playerHurt2 = default;
    [SerializeField] private GameObject _bigExplosionPrefab = default;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameObject _shield;
    private Animator _anim;

    private float _cadenceInitiale;
    private float _canFire = -1;

    private void Awake()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = FindObjectOfType<UIManager>().GetComponent<UIManager>();
        _shield = transform.GetChild(0).gameObject;
        _anim = GetComponent<Animator>();
    }

    void Start()
    {
        transform.position = new Vector3(0f, -2.4f, 0f);  // position initiale du joueur
        if (PlayerPrefs.HasKey("compteur"))
        {
            int compteur = PlayerPrefs.GetInt("compteur");
            compteur += 1;
            PlayerPrefs.SetInt("compteur", compteur);
        }
        else
        {
            PlayerPrefs.SetInt("compteur", 1);
        }
    }

    void Update()
    {
        Move();
        Tir();
    }
    
    // M�thode qui g�re le tir du joueur ainsi que le d�lai entre chaque tir
    private void Tir()
    {
        //Debug.Log(Input.GetAxis("Fire1"));
        if (Input.GetAxis("Fire1") == 1 && Time.time > _canFire)
        {
            _canFire = Time.time + _delai;
            // Si le bool�en du triplelaser est actif on instancie des triple laser � la place
            AudioSource.PlayClipAtPoint(_laserSound, Camera.main.transform.position, 0.3f);
            if (_isTripleLaserActif)
            {
                Instantiate(_tripleLaserPrefab, (transform.position + new Vector3(0f, 2.19f, 0f)), Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, (transform.position + new Vector3(0f, 0.9f, 0f)), Quaternion.identity);
            }
        }
    }

    // D�placements et limitation des mouvements du joueur
    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0f);
        transform.Translate(direction * Time.deltaTime * _speed);

        if (horizontalInput < 0)
        {
            _anim.SetBool("Turn_Left", true);
            _anim.SetBool("Turn_Right", false);
        }
        else if(horizontalInput > 0)
        {
            _anim.SetBool("Turn_Left", false);
            _anim.SetBool("Turn_Right", true);
        }
        else
        {
            _anim.SetBool("Turn_Left", false);
            _anim.SetBool("Turn_Right", false);
        }
        //G�rer la zone verticale
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8.3f, 8.3f),
        Mathf.Clamp(transform.position.y, -3.07f, 2.3f), 0f);
        

        //G�rer d�passement horizontaux
        //if (transform.position.x >= 11.3)
        //{
        //    transform.position = new Vector3(-11.3f, transform.position.y, 0f);
        //}
        //else if (transform.position.x <= -11.3)
        //{
        //    transform.position = new Vector3(11.3f, transform.position.y, 0f);
        //}
    }

    // M�thodes publiques ==================================================================

    // M�thode appell� quand le joueur subit du d�gat
    public void Degats()
    {
        // Si le shield est actif on le d�astive sinon on enl�ve une vie au joueur
        if (!_shield.activeSelf)
        {
            _viesJoueur--;
            if (_viesJoueur == 2)
            {
                _playerHurt1.SetActive(true);
            }
            else if (_viesJoueur == 1)
            {
                _playerHurt2.SetActive(true);
            }
            _uiManager.ChangeLivesDisplayImage(_viesJoueur);
        }
        else
        {
            _shield.SetActive(false);
        }

        // Si le joueur n'a plus de vie on arr�te le spwan et d�truit le joueur
        if (_viesJoueur < 1)
        {
            _spawnManager.mortJoueur();
            Instantiate(_bigExplosionPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(_endSound, Camera.main.transform.position, 0.8f);
            Destroy(this.gameObject);
        }
    }

    // M�thode et coroutine li� � l'am�lioration triple shot de mon joueur
    public void PowerTripleShot()
    {
        _isTripleLaserActif = true;
        StartCoroutine(TripleShotRoutine());
    }

    IEnumerator TripleShotRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleLaserActif = false;
    }

    // M�thode et coroutine li� � l'augmentation de la vitesse du joueur
    public void SpeedPowerUp()
    {
        _cadenceInitiale = _delai;
        _delai = 0.1f;
        StartCoroutine(SpeedRoutine());
    }

    IEnumerator SpeedRoutine()
    {
        yield return new WaitForSeconds(5f);
        _delai = _cadenceInitiale;
    }

    // M�thode li� � l'activation du shield
    public void ShieldPowerUp()
    {
        _shield.SetActive(true);
    }
}
