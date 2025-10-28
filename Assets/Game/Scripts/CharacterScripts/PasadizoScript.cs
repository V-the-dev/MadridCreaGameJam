using System;
using UnityEngine;

public class PasadizoScript : CharacterBaseScript
{
    [SerializeField] private SpriteRenderer pasadizoCerrado;
    [SerializeField] private SpriteRenderer pasadizoAbierto;
    [SerializeField] private GameObject barrier;

    private void Start()
    {
        pasadizoAbierto.enabled = false;
        pasadizoCerrado.enabled = true;
        barrier.SetActive(true);
    }

    public override void BucleReset()
    {
        base.BucleReset();
        pasadizoAbierto.enabled = false;
        pasadizoCerrado.enabled = true;
        barrier.SetActive(true);
    }

    public void OpenPasadizo()
    {
        pasadizoAbierto.enabled = true;
        pasadizoCerrado.enabled = false;
        barrier.SetActive(false);
    }
}
