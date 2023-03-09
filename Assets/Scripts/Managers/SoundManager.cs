using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private float _volume = 1f;

    private Camera _mainCamera;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
        DeliveryManager.Instance.OnRecipeDeliveryFailed += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeExpired += DeliveryManager_OnRecipeExpired;
        WalletManager.Instance.OnWalletAmountChanged += WalletManager_OnWalletAmountChanged;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnAnyPickupObject += Player_OnPickedUpObject;
        BaseCounter.OnAnyObjectPlacedOnCounter += BaseCounter_OnAnyObjectPlacedOnCounter;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void WalletManager_OnWalletAmountChanged(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.coinSounds, _mainCamera.transform.position);
    }

    private void DeliveryManager_OnRecipeExpired(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.expiredSounds, _mainCamera.transform.position);
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trashSounds, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedOnCounter(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.objectDropSounds, baseCounter.transform.position);
    }

    private void Player_OnPickedUpObject(object sender, EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(audioClipRefsSO.objectPickupSounds, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.choppingSounds, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliveryFailSounds, deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeDelivered(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.deliverySuccessSounds, deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume);
    }

    public void PlayFootstepSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.footstepSounds, position, volume);
    }

    public void PlayCountdownSound()
    {
        PlaySound(audioClipRefsSO.warningSounds, Vector3.zero, _volume);
    }

    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipRefsSO.warningSounds, position, _volume);
    }

    public void ChangeVolume(float volume)
    {
        _volume = volume;
    }

    public float GetVolume()
    {
        return _volume;
    }
}
