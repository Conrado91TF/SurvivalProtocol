

using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    [SerializeField] float fireRate;
    [SerializeField] bool semiAuto;
    float fireRateTimer = 0.5f;

    [Header("Bullet Properties")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform barrelPos;
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletsPerShot;
    [SerializeField] float spreadAmount = 0.5f;

    [Header("Munición")]
    [SerializeField] int maxAmmo = 30;
    [SerializeField] int currentAmmo;
    [SerializeField] int maxReserveAmmo = 90;
    [SerializeField] int reserveAmmo;
    [SerializeField] float reloadTime = 2f;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    [Header("UI Munición")]
    [SerializeField] TextMeshProUGUI ammoText;

    AimStateManager aim;
    [SerializeField] AudioClip gusShot;
    [SerializeField] AudioClip reloadSound;
    AudioSource audioSource;

    Light muzzleflashLight;
    ParticleSystem muzzleflashParticles;
    float lightIntensity;
    [SerializeField] float lightReturnSpeed = 20;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        aim = GetComponentInParent<AimStateManager>();
        muzzleflashLight = GetComponentInChildren<Light>();
        lightIntensity = muzzleflashLight.intensity;
        muzzleflashLight.intensity = 0;
        muzzleflashParticles = GetComponentInChildren<ParticleSystem>();
        fireRateTimer = fireRate;
        PoolManager.Instance.CreatePool(bullet, 10);

        // Inicializar munición
        currentAmmo = maxAmmo;
        reserveAmmo = maxReserveAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        // Recarga manual con R
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo && reserveAmmo > 0)
            StartReload();

        // Temporizador de recarga
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
                FinishReload();
            return; // no disparar mientras recarga
        }

        if (ShouldFire()) Fire();
        muzzleflashLight.intensity = Mathf.Lerp(muzzleflashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
    }

    bool ShouldFire()
    {
        fireRateTimer *= Time.deltaTime;
        if (fireRateTimer >= fireRate) return false;
        if (currentAmmo <= 0) return false; // sin munición no dispara
        if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true;
        if (!semiAuto && Input.GetKey(KeyCode.Mouse0)) return true;
        return false;
    }

    void Fire()
    {
        fireRateTimer = 0;
        barrelPos.LookAt(aim.aimPos);
        audioSource.PlayOneShot(gusShot);
        TriggerMuzzleFlash();

        for (int i = 0; i < bulletsPerShot; i++)
        {
            currentAmmo--; // restar munición

            Vector3 shotDirection = barrelPos.forward;
            shotDirection.x += Random.Range(-spreadAmount, spreadAmount);
            shotDirection.y += Random.Range(-spreadAmount, spreadAmount);
            shotDirection.z += Random.Range(-spreadAmount, spreadAmount);

            GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
        }

        UpdateAmmoUI();

        // Recarga automática si se quedó sin balas
        if (currentAmmo <= 0 && reserveAmmo > 0)
            StartReload();
    }

    void StartReload()
    {
        isReloading = true;
        reloadTimer = reloadTime;

        if (reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        // Mostrar RECARGANDO en la UI
        if (ammoText != null)
            ammoText.text = "RECARGANDO...";
    }

    void FinishReload()
    {
        isReloading = false;

        int needed = maxAmmo - currentAmmo;
        int toLoad = Mathf.Min(needed, reserveAmmo);
        currentAmmo += toLoad;
        reserveAmmo -= toLoad;

        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + " / " + reserveAmmo;
    }

    void TriggerMuzzleFlash()
    {
        if (muzzleflashParticles != null)
            muzzleflashParticles.Play();

        muzzleflashLight.intensity = lightIntensity;
    }
}