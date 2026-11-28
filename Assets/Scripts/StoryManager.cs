using System.Collections;
using System.ComponentModel.Design.Serialization;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StoryManager : MonoBehaviour
{

    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private Gamemanager gameManager;
    [SerializeField] private ScreenShake screenShake;

    [Header ("Karding Things")]
    [SerializeField] private GameObject karding; 
    [SerializeField] private Animator kardingAnimator;
    [SerializeField] private KardingAutoMovement kardingAutoMovement;
    [SerializeField] private GeneralAutoMovement kardingGeneralAutoMovement;
    [SerializeField] private SpriteRenderer kardingSpriteRenderer;
    [SerializeField] private float kardingFollowDistance = 0.5f;

    private bool isKardingFollowing;

    [Header ("Mayor Things")]
    [SerializeField] private GameObject Mayor;
    [SerializeField] private Animator MayorAnimator;
    [SerializeField] private MayorAuraFarmScript mayorAuraFarmScript;

    [Header ("Jeff Things")]
    [SerializeField] private GameObject Jeff;
    [SerializeField] private Animator JeffAnimator;
    [SerializeField] private GeneralAutoMovement jeffGeneralAutoMovement;
    [SerializeField] private JeffPlayerMovement jeffPlayerMovement;

    [Header ("Guard Things")]
    [SerializeField] private GameObject Guard;
    [SerializeField] private Animator GuardAnimator;

    [Header ("Timing Game")]
    [SerializeField] private TimingMinigame timingMinigame;
    [SerializeField] private Gunflash gunflash;

   
    


    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MayorBodegaScene")
        {
            StartCoroutine(Section1());
        }
        else if (SceneManager.GetActiveScene().name == "MayorLobbyScene")
        {
            

            StartCoroutine(Section2());
        }
    }

    private void Update()
    {
        if (!isKardingFollowing)
            return;

        // Check whether Jeff is moving.
        bool jeffIsMoving = jeffPlayerMovement.IsMoving;

        if (jeffIsMoving)
        {
            kardingAutoMovement.FollowJeff();

            // Walking animation.
            kardingAnimator.SetBool("KardingIsWalking", true);
            kardingAnimator.SetBool("KardingIsSideIdle", false);
        }
    }

    private IEnumerator LoadNextScene(string sceneName)
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    }

    private void Startsequence()
    {
        StartCoroutine(StoryFlow());
        
    }

    private IEnumerator StoryFlow()
    {
        yield return StartCoroutine(LoadNextScene("MayorBodegaScene"));
        
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
                timingMinigame.enabled = false;
                gunflash.Flash();
                dialogManager.StartDialogue(2);
                yield return new WaitUntil(() => dialogManager.isDialogActive == false);
                timingMinigame.enabled = true;
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
                kardingAutoMovement.WalkToMayor();
                yield return new WaitUntil(() => kardingAutoMovement.HasArrived);
                kardingAnimator.SetBool("KardingIsSideIdle", false);
                kardingAnimator.SetBool("KardingIsWalking", false);
                kardingAnimator.SetBool("KardingIsHitting", true);
                yield return new WaitUntil(() => kardingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
                MayorAnimator.SetBool("MayorDown", true);
                
                yield return new WaitUntil(() => kardingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
                kardingAnimator.SetBool("KardingIsHitting", false);
                kardingAnimator.SetBool("KardingIsSideIdle", false);
               
                kardingAutoMovement.WalkToPosition();

                yield return new WaitUntil(() => kardingAutoMovement.HasArrived);
               
                kardingAnimator.SetBool("KardingIsWalkingBack", false);
                kardingSpriteRenderer.flipX = false;
                
                kardingAnimator.SetBool("KardingIsSideIdle", true);
                yield return new WaitForSeconds(1f);
                gameManager.StartEnding();
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(LoadNextScene("MayorLobbyScene"));

            }
        }
    }

    private IEnumerator Section2()
    {
        gameManager.StartEntrance();
        yield return new WaitUntil(() => gameManager.EntranceFinished);

        jeffPlayerMovement.enabled = false;

        jeffGeneralAutoMovement.MoveTo(-0.03f, -0.5f);
        JeffAnimator.SetInteger("Direction", 3);

        yield return new WaitUntil(() => jeffGeneralAutoMovement.HasArrived());

        JeffAnimator.SetInteger("Direction", 0);

        yield return new WaitForSeconds(1f);

        jeffPlayerMovement.enabled = true;
        isKardingFollowing = true;
    }



}
