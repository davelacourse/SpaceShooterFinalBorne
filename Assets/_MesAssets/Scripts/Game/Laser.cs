using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private string _nom = default;
    [SerializeField] private GameObject _miniExplosionPrefab = default;

    private UIManager _uiManager;
    private float _vitesseLaserEnnemi;

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
    }
    void Update()
    {
        
        if (_nom == "Player")
        {
            // D�place le laser vers le haut
            DeplacementLaserJoueur();
        }
        else if (_nom == "Enemy")
        {
            _vitesseLaserEnnemi = _uiManager.getVitesseEnnemi() + 3.0f;
            transform.Translate(Vector3.down * Time.deltaTime * _vitesseLaserEnnemi);
            if (transform.position.y < -6f)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            _vitesseLaserEnnemi = _uiManager.getVitesseEnnemi() + 6.0f;
            transform.Translate(Vector3.down * Time.deltaTime * _vitesseLaserEnnemi);
            if (transform.position.y < -6f)
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    private void DeplacementLaserJoueur()
    {
        transform.Translate(Vector3.up * Time.deltaTime * _speed);
        if (transform.position.y > 8f)
        {
            // Si le laser sort de l'�cran il se d�truit
            if (this.transform.parent == null)
            {
                Destroy(this.gameObject);
            }
            // Si le laser fait partie d'un conteneur il d�truit le conteneur
            else
            {
                Destroy(this.transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _nom != "Player")
        {
            Player player = other.GetComponent<Player>();
            player.Degats();
            Instantiate(_miniExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
