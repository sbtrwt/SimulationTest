using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheel : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 500;
    [SerializeField] float offset = 60;
    [SerializeField] int slotNum = 2;
    private bool isSpinning = false;

    [SerializeField] Button buttonSpin;
    [SerializeField] TMP_InputField textSlot;
    private void Start()
    {
        buttonSpin.onClick.AddListener(OnSpin);
    }
  
    private IEnumerator Rotatate() 
    {
        isSpinning = true;

       
        int fullSpins = 2;
        float totalRotation = 360 * fullSpins + GetSlotAngle(slotNum) ;

        float currentRotation = 0;

        while (currentRotation < totalRotation)
        {
            float rotationStep = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, rotationStep);
            currentRotation += rotationStep;

            yield return null;
        }

       
        transform.eulerAngles = new Vector3(0, 0, GetSlotAngle(slotNum));

        isSpinning = false;
    }

    private float GetSlotAngle(int slotNum) 
    {
        float result = ((slotNum-1) * offset) + 30;
        return result;
    }

    private void OnSpin() 
    {
        if (isSpinning)
            return;

        int.TryParse(textSlot.text,out slotNum);
        slotNum = slotNum == 0 ? 1 : slotNum;
        StartCoroutine(Rotatate());
    }
}
