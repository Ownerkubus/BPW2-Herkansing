using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Gun : MonoBehaviour
{
    [Header("Gun")]
    public bool isUsingRifle = false;
    public GameObject Rifle;
    [Space]
    [Header("Gun statistics")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 5f;
    public float recoilRange = 1;
    [Space]
    [Header("Reload statistics")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1f;
    public float recoilTime = 0.1f;
    private bool isReloading = false;
    public float aimRecoil = 2;
    [Space]
    public Text ammoUI;
    [Space]
    [Header("Camera")]
    public Camera fpsCam;
    public GameObject fpsc;
    [Space]
    [Header("Effects")]
    public Animator animator;
    [Space]
    [Header("Audio")]
    public AudioSource gunFX;
    public AudioSource outOfAmmoFX;
    public AudioSource reloadFX;
    public AudioSource enemyDeadSound;

    private float nextTimeToFire = 0f;
    private float normalRecoil;

    void Awake()
    {
      if(isUsingRifle){
        Rifle.SetActive(true);
      }
    }

    void Start()
    {
      currentAmmo = maxAmmo;
      normalRecoil = recoilRange;
      //enemyDeadSound = GetComponent<AudioSource>();
    }

    void Update()
    {
      if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0 && isReloading == false)
      {
        nextTimeToFire = Time.time + 1f / fireRate;
        Shoot();
      }
      if(Input.GetButtonDown("Fire1") && currentAmmo == 0)
      {
        outOfAmmoFX.Play();
      }
      if(Input.GetKeyDown(KeyCode.R) && currentAmmo != 30)
      {
        StartCoroutine(Reload());
      }
    }

    void Shoot()
    {
      StartCoroutine(Recoil());
      gunFX.Play();
      currentAmmo--;

      fpsc.transform.eulerAngles = new Vector3(
      fpsc.transform.eulerAngles.x + Random.Range(-recoilRange, recoilRange),
      fpsc.transform.eulerAngles.y + Random.Range(-recoilRange, recoilRange),
      fpsc.transform.eulerAngles.z
      );

      RaycastHit hit;
      if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, 11) && hit.transform.gameObject.tag == "Enemy")
      {
            Destroy(hit.transform.gameObject);
            ScoreScript.ScoreNum++;
            enemyDeadSound.Play();
        }
    }

    IEnumerator Reload()
    {
      isReloading = true;

      reloadFX.Play();

      animator.SetBool("Reloading", true);

      yield return new WaitForSeconds(reloadTime);

      animator.SetBool("Reloading", false);

      currentAmmo = maxAmmo;

      isReloading = false;
    }

    IEnumerator Recoil()
    {
      animator.SetBool("Shooting",true);
      yield return new WaitForSeconds(recoilTime);
      animator.SetBool("Shooting",false);
    }
}
