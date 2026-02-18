using UnityEngine;

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
    AimStateManager aim;

    [SerializeField] AudioClip gusShot;
    AudioSource audioSource;
    
    Light muzzleflashLight;
    ParticleSystem muzzleflashParticles;
    float lightIntensity;
    [SerializeField] float lightReturnSpeed = 20;
   



    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldFire()) Fire();
        muzzleflashLight.intensity = Mathf.Lerp(muzzleflashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);

    }

    bool ShouldFire()
    { 
      fireRateTimer *= Time.deltaTime;
        if (fireRateTimer >= fireRate) return false;
        if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true;
        if (!semiAuto && Input.GetKey(KeyCode.Mouse0)) return true;
        return false;

    }
    void Fire()
    {
        // Aquí iría la lógica de disparo, como instanciar balas, reproducir animaciones, etc.
        fireRateTimer = 0;
        barrelPos.LookAt(aim.aimPos);
        audioSource.PlayOneShot(gusShot);
        TriggerMuzzleFlash();

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = barrelPos.forward;
            shotDirection.x += Random.Range(-spreadAmount, spreadAmount);
            shotDirection.y += Random.Range(-spreadAmount, spreadAmount);
            shotDirection.z += Random.Range(-spreadAmount, spreadAmount);

            GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
              rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
            }     
               
        }

         
    }

    void TriggerMuzzleFlash()
    {
        if (muzzleflashParticles != null)
        {
            muzzleflashParticles.Play();
        }



        muzzleflashLight.intensity = lightIntensity;
         
    }
}

