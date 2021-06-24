using System.Collections;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float _shotCooldown = 0.4f;
    [Space]
    [Header("Weapons")]
    [SerializeField] private GameObject _laserPrefab;

    private bool _canFire = true;


    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && _canFire)
            ShootLaser();
    }

    public void ShootLaser()
    {
        Vector3 spawnPoint = transform.position + new Vector3(0, 0.8f, 0);
        Instantiate(_laserPrefab, spawnPoint, Quaternion.identity);
        StartCoroutine(ShotCooldown());
    }

    private IEnumerator ShotCooldown()
    {
        _canFire = false;
        yield return new WaitForSeconds(_shotCooldown);
        _canFire = true;
    }
}
