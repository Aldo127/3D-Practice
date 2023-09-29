using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileGun : MonoBehaviour
{
    //bullet
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;
    
  
    bool shooting, readyToShoot, reloading;


    //recoil 
    public Rigidbody playerRb;
    public float recoilForce;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;

    //GRAPHICS
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bugs
    public bool allowInvoke = true;
    private void Awake()
    {
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        //ammodisplay if i decide to do it (probably not >:(((((((( )
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + "/" + magazineSize / bulletsPerTap);

        MyInput();
    }

    private void MyInput()
    {
        //Check if allowed to hold down button fire button and take corresponding input
         if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
         else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //weloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();
        //weload automaficaly when shbooting without boullets
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;


        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //ray through middle of screen
        RaycastHit hit;

        //check if ray hit
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        //calculate direction from attackpoint to targetpoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //calculate dif direction wih da spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //I need more boullets!!!
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //boullet direcaltiton
        currentBullet.transform.forward = directionWithSpread.normalized;

        //THRUST the boullets!!
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);


        //spawn dat muzzle flash in there (im prob not making one )
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        //resetang da shootang
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //RECOIL
            playerRb.AddForce(-directionWithoutSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        // more tapang more shootang
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }

        
    }

    private void ResetShot()
    {
        //alow more boullets and invokkkkkkeingngng
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
