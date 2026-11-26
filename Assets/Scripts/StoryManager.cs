using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.ComponentModel.Design.Serialization;
public class StoryManager : MonoBehaviour
{
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private ScreenShake screenShake;
    [SerializeField] private GameObject karding; 
    [SerializeField] private Animator kardingAnimator;
    [SerializeField] private KardingMovement kardingMovement;
    [SerializeField] private SpriteRenderer kardingSpriteRenderer;
    [SerializeField] private GameObject Mayor;
    [SerializeField] private Animator MayorAnimator;
    [SerializeField] private MayorAuraFarmScript mayorAuraFarmScript;
    [SerializeField] private GameObject Jeff;
    [SerializeField] private Animator JeffAnimator;


    [SerializeField] private GameObject Guard;
    [SerializeField] private Animator GuardAnimator;
    [SerializeField] private TimingMinigame timingMinigame;
    [SerializeField] private Gunflash gunflash;

    [SerializeField] private Gamemanager gameManager;
    void Start()
    {
        Startsequence();
    }

    private void Startsequence()
    {
        StartCoroutine(Section1());
    }

    private IEnumerator Section1()
    {
        Debug.Log("1. Coroutine started");
        yield return new WaitForSeconds(2f);

        Debug.Log("2. Wait finished. DialogManager is: " + dialogManager);

        dialogManager.StartDialogue(0);

        yield return new WaitUntil(() => dialogManager.dialogueLines != null && dialogManager.currentLine == dialogManager.dialogueLines.Length - 1);

        screenShake.Shake();

        yield return new WaitUntil(() => dialogManager.isDialogActive == false);

        
        mayorAuraFarmScript.FallBack();

        karding.SetActive(true);
        kardingAnimator.SetBool("KardingIsSideIdle", true);
        kardingAnimator.SetBool("KardingIsSideIdle", false);
        kardingAnimator.SetBool("KardingIsHitting", true);


        yield return new WaitUntil(() => kardingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        dialogManager.StartDialogue(1);

        yield return new WaitUntil(() => dialogManager.isDialogActive == false);

        timingMinigame.StartMinigame();
        bool success = false;

        while (!success)
        {
            timingMinigame.StartMinigame();

            // Wait for player to click.
            yield return new WaitUntil(
                () => timingMinigame.HasFinished
            );

            success = timingMinigame.WasSuccessful;

            if (!success)
            {
                gunflash.Flash();
                dialogManager.StartDialogue(2);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                timingMinigame.RemoveMinigame();

                kardingAnimator.SetBool("KardingIsHitting", true);
                GuardAnimator.SetBool("IsKnocked", true);
                kardingAnimator.SetBool("KardingIsHitting", false);
                kardingAnimator.SetBool("KardingIsSideIdle", true);

                dialogManager.StartDialogue(3);

                yield return new WaitUntil(() => dialogManager.isDialogActive == false);

                kardingAnimator.SetBool("KardingIsWalking", true);
                kardingMovement.WalkToMayor();


                yield return new WaitUntil(() => kardingMovement.HasArrived);
                kardingAnimator.SetBool("KardingIsHitting", true);

                yield return new WaitUntil(() => kardingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
                MayorAnimator.SetBool("MayorDown", true);
                yield return new WaitUntil(() => kardingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
                kardingAnimator.SetBool("KardingIsHitting", false);
                yield return new WaitUntil(() => kardingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
                kardingAnimator.SetBool("KardingIsWalkingBack", true);
                kardingMovement.WalkToPosition();

                yield return new WaitUntil(() => kardingMovement.HasArrived);

                kardingSpriteRenderer.flipX = false;
                yield return new WaitForSeconds(1f);
                gameManager.StartEnding();

            }
        }

        









    }



}
