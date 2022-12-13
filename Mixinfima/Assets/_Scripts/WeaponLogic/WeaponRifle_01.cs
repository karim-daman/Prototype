using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRifle_01 : WeaponBase, IWeapon
{

    public int numberOfClips = 5, bulletsLeft;

    [SerializeField] ParticleSystem shellParticles;
    [SerializeField] GameObject rifleSlot;
    [SerializeField] GameObject rifleBoltSlider;
    [SerializeField] GameObject bulletSpawnPoint;
    [SerializeField] GameObject bulletContainer;



    [SerializeField] float lerpSpeed = 15;
    [SerializeField] int reloadStep = -1;
    [SerializeField] bool isCocked = false;
    float reloadLerpTime = 0;

    [SerializeField] IKController iKController;
    [SerializeField] List<Transform> reload_targets;
    [SerializeField] GameObject magazineClip;
    [SerializeField] Inventory inventory;


    // [SerializeField] GameObject slot;
    // [SerializeField] GameObject bagPack;

    // float timer;
    // float start;




    private void Awake() => rb = GetComponent<Rigidbody>();


    private void Start()
    {
        isAvailable = isClipEmpty = true;
        isHolstered = isEquipped = false;
        bulletsLeft = weapon.MaxAmmoPerMag;
    }

    void Update()
    {

        // Debug.DrawRay(bulletSpawnPoint.transform.position, transform.forward * 10, Color.red, 1);
        Debug.DrawRay(bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.TransformDirection(Vector3.forward) * 10, Color.red);


        if (!isEquipped) return;

        // if (!isCocked)
        // {
        //     if (iKController.rightHandIK && iKController.leftHandIK)
        //     {
        //         // if (isReloading || isShooting || !isEquipped || (numberOfClips == 0)) return;
        //         isReloading = true;
        //         reloadStep = 5;
        //     }
        // }

        if (Input.GetMouseButton(0)) if (bulletsLeft > 0 && isEquipped) Shoot();
        Recoil();

        if (Input.GetKeyDown(KeyCode.G)) inventory.DropWeapon();
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isReloading || isShooting || !isEquipped || (numberOfClips == 0)) return;
            isReloading = true;
            reloadStep++;
        }

        Reload();
    }

    public void Shoot() => isShooting = true;

    public void Recoil()
    {
        if (isShooting && !isReloading)
        {
            if (!hasFired) FireBullet();
            recoil_elapsed_time += Time.deltaTime;
            if (recoil_elapsed_time >= weapon.FireRate * .45f && !hasEjectedShell)
            {
                hasEjectedShell = true;
                shellParticles.Emit(1);
            }

            if (recoil_elapsed_time >= weapon.FireRate) isShooting = hasEjectedShell = hasFired = false;

            float lerpRatio = recoil_elapsed_time / weapon.FireRate;
            rifleBoltSlider.transform.localPosition = Vector3.LerpUnclamped(rifleBoltSlider.transform.localPosition, new Vector3(rifleBoltSlider.transform.localPosition.x, rifleBoltSlider.transform.localPosition.y, EvaluateCurve(weapon.boltSliderCurve, lerpRatio)), Time.deltaTime * 100);
            rifleSlot.transform.localPosition = Vector3.LerpUnclamped(rifleSlot.transform.localPosition, new Vector3(rifleSlot.transform.localPosition.x, rifleSlot.transform.localPosition.y, EvaluateCurve(weapon.ZkickBack, lerpRatio)), Time.deltaTime * lerpSpeed);
            // rifleSlot.transform.localRotation = Quaternion.Slerp(
            //         rifleSlot.transform.localRotation, Quaternion.Euler(EvaluateCurve(weapon.XrotationRecoil, lerpRatio),
            //         0,
            //         0), Time.deltaTime * lerpSpeed);

            rifleSlot.transform.localRotation = Quaternion.Slerp(rifleSlot.transform.localRotation,
                    Quaternion.Euler(EvaluateCurve(weapon.XrotationRecoil, lerpRatio) * Random.Range(-2, 2),
                                     EvaluateCurve(weapon.XrotationRecoil, lerpRatio) * .5f,
                                     EvaluateCurve(weapon.XrotationRecoil, lerpRatio) * Random.Range(-1, 1)
                    ), Time.deltaTime * lerpSpeed);

        }
        if (!isShooting && !isReloading) recoil_elapsed_time = 0;

        float EvaluateCurve(AnimationCurve curve, float t) => curve.Evaluate(t);
        void FireBullet()
        {
            isClipEmpty = --bulletsLeft == 0 ? true : false;
            hasFired = true;
            GameObject bulletTransform = Instantiate(weapon.BulletPrefab, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation, bulletContainer.transform);
            bulletTransform.GetComponent<Projectile>().Setup(bulletSpawnPoint.transform.forward, weapon.BulletVelocity);
        }
    }

    public void Reload()
    {

        bool nextStep(GameObject hand, Transform pose)
        {
            reloadLerpTime = Mathf.Clamp01((reloadLerpTime + (Time.deltaTime * weapon.ReloadSpeed)));
            hand.transform.position = Vector3.Slerp(hand.transform.position, pose.position, reloadLerpTime / 5);
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, pose.transform.rotation, reloadLerpTime / 5);
            return reloadLerpTime == 1 ? true : false;
        }

        if (isReloading)
        {
            switch (reloadStep)
            {
                case 0: //hand on mag
                    if (nextStep(iKController.leftHandIK_target, reload_targets[reloadStep]))
                    {
                        magazineClip.transform.SetParent(iKController.leftHandIK_target.transform);
                        reloadStep++;
                        reloadLerpTime = 0;
                    };
                    break;
                case 1: //throw old mag
                        //--------------------------------------------------------------------------------------------------------------------------------------------- ?????????? code not good! 
                    for (int i = 0; i < iKController.fingerBones_left.Count - 1; i++)
                    {
                        iKController.fingerBones_left[i].weight = Mathf.Lerp(iKController.fingerBones_left[i].weight, 0, reloadLerpTime / 5);
                        if (iKController.fingerBones_left[i].weight < .1f)
                        {
                            magazineClip.transform.SetParent(null);
                            magazineClip.AddComponent<Rigidbody>();
                            magazineClip.GetComponent<Rigidbody>().isKinematic = false;
                        }
                    }
                    //--------------------------------------------------------------------------------------------------------------------------------------------- ?????????? code not good! 


                    if (nextStep(iKController.leftHandIK_target, reload_targets[reloadStep]))
                    {
                        reloadStep++;
                        reloadLerpTime = 0;
                    };
                    break;
                case 2: //get new mag

                    for (int i = 0; i < iKController.fingerBones_left.Count - 1; i++)
                    {
                        iKController.fingerBones_left[i].weight = Mathf.Lerp(iKController.fingerBones_left[i].weight, 1, reloadLerpTime / 5);
                    }

                    if (nextStep(iKController.leftHandIK_target, reload_targets[reloadStep]))
                    {
                        magazineClip = Instantiate(weapon.MagazinePrefab, new Vector3(), Quaternion.identity, iKController.leftHandIK_target.transform);
                        magazineClip.transform.localPosition = weapon.MagazineLocalSpawnPosition;
                        magazineClip.transform.localRotation = Quaternion.Euler(weapon.MagazineLocalSpawnRotation);
                        magazineClip.transform.localScale = weapon.MagazineLocalSpawnScale;
                        reloadStep++;
                        reloadLerpTime = 0;
                    };
                    break;
                case 3: //clip in new mag
                    if (nextStep(iKController.leftHandIK_target, reload_targets[reloadStep]))
                    {
                        magazineClip.transform.SetParent(transform);
                        // oldClip.GetComponent<Rigidbody>().isKinematic = true;
                        reloadStep++;
                        reloadLerpTime = 0;
                    };
                    break;
                case 4: // hand left on grip
                    if (nextStep(iKController.leftHandIK_target, reload_targets[reloadStep]))
                    {
                        if (isClipEmpty) reloadStep++;
                        else reloadStep = -1;
                        reloadLerpTime = 0;

                    };
                    break;
                case 5: // right hand on bolt lever
                    if (nextStep(iKController.rightHandIK_target, reload_targets[reloadStep]))
                    {
                        reloadStep++;
                        reloadLerpTime = 0;
                    };
                    break;

                case 6: // right hand on bolt lever pull back

                    // rifleBoltSlider.transform.localPosition = new Vector3(rifleBoltSlider.transform.localPosition.x, rifleBoltSlider.transform.localPosition.y, rifleBoltSlider.transform.localPosition.z - .0002f);
                    rifleBoltSlider.transform.parent = iKController.rightHandIK_target.transform;
                    if (nextStep(iKController.rightHandIK_target, reload_targets[reloadStep]))
                    {
                        rifleBoltSlider.transform.parent = transform;
                        rifleBoltSlider.transform.localPosition = new Vector3();
                        rifleBoltSlider.transform.localRotation = Quaternion.identity;

                        reloadStep++;
                        reloadLerpTime = 0;
                    };
                    break;

                case 7: // hand right on grip

                    if (nextStep(iKController.rightHandIK_target, reload_targets[reloadStep]))
                    {
                        reloadStep++;
                        reloadLerpTime = 0;
                    };
                    break;

                default:
                    reloadStep = -1;
                    isReloading = false;
                    numberOfClips--;
                    bulletsLeft = weapon.MaxAmmoPerMag;
                    break;
            }
        }
    }




}



