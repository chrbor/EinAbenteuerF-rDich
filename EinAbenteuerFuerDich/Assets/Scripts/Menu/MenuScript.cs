using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static LoadSave;
using static GameManager;
using static CameraScript;
using static ppAccess;
using static TasklistScript;

public class MenuScript : MonoBehaviour
{
    public static MenuScript menu;
    public bool isStartScreen;
    public bool isPortrait;
    public Material transition;
    public GameObject nextSeq;

    [HideInInspector]
    public bool useLeftDecision;
    [HideInInspector]
    public bool decisionMade;

    private bool setTaskListBack = false;

    private void Awake()
    {
        menu = this;
        if (!isStartScreen) return;

        //LoadProgress();
        //SaveProgress();
        //DeleteSaveFile();

        if (SaveFileExists()) return;
        Canvas.ForceUpdateCanvases();
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    public void RotateMenu(bool isPortrait, float changeTime = 1) => StartCoroutine(SetMenuRotation(isPortrait, changeTime));
    public IEnumerator SetMenuRotation(bool _isPortrait, float changeTime = 1)
    {
        isPortrait = _isPortrait;
        RectTransform menuButton = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        RectTransform ingameMenu = transform.GetChild(1).GetComponent<RectTransform>();

        //Rotating to Portrait:
        ingameMenu.anchorMin = .5f * (isPortrait ? Vector2.right : Vector2.up);
        ingameMenu.anchorMax = ingameMenu.anchorMin;

        float timeStep = Time.fixedDeltaTime / changeTime;
        float percent;
        for(float count = 0; count < 1; count += timeStep)
        {
            menuButton.eulerAngles = Vector3.zero;
            ingameMenu.eulerAngles = menuButton.eulerAngles;

            percent = isPortrait ? count : 1 - count;
            ingameMenu.anchoredPosition = new Vector2(200 * (1 - percent), 200 * percent);

            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    public void SetMenuActive(bool active)
    {
        pauseGame = active;
        pauseMove = active || runGame;
        staticCam = active || runGame;
        StartCoroutine(OpenCloseMenu(active));
    }
    IEnumerator OpenCloseMenu(bool open)
    {
        //Transform menuTransform = transform.GetChild(0);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);

        if(taskList)
            if(taskList.activeSelf ^ !open && !runGame)
            {
                StartCoroutine(SetTaskList(!open && !taskList.activeSelf && setTaskListBack));
                setTaskListBack = taskList;
            }
        CanvasGroup ingameButtons = transform.GetChild(0).GetComponent<CanvasGroup>();
        CanvasGroup menuButtons = transform.GetChild(1).GetComponent<CanvasGroup>();
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            menuButtons.alpha = open? count : 1 - count;
            ingameButtons.alpha = 1 - menuButtons.alpha;
            postprocess.doF.focalLength.value = 1 + menuButtons.alpha * 49;
            postprocess.bloom.threshold.value = .95f - menuButtons.alpha * .2f;
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(cScript.SetRotation(isPortrait? 90 : 0));

        transform.GetChild(open? 0 : 1).gameObject.SetActive(false);
        yield break;
    }

    public void ChoseLeftDecision() { useLeftDecision = true; decisionMade = true; StartCoroutine(OpenCloseDecision(false)); }
    public void ChoseRightDecision() { useLeftDecision = false; decisionMade = true; StartCoroutine(OpenCloseDecision(false)); }
    public void SetDecisionActive(bool active) => StartCoroutine(OpenCloseDecision(active));
    IEnumerator OpenCloseDecision(bool open)
    {
        transform.GetChild(2).gameObject.SetActive(true);

        CanvasGroup decisionButtons = transform.GetChild(2).GetComponent<CanvasGroup>();
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            decisionButtons.alpha = open ? count : 1 - count;
            yield return new WaitForFixedUpdate();
        }

        pauseGame = open;
        transform.GetChild(2).gameObject.SetActive(open);
        decisionMade = false;
        yield break;
    }

    /// <summary>
    /// Läuft die Transition durch. direction: 1 = nach rechts,  closing: 1 = Verlauf von links(opaque) nach rechts(transparent)
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="closing"></param>
    public void DoTransition(float direction, float closing, bool hide) => StartCoroutine(UseTransition(direction, closing, hide));
    IEnumerator UseTransition(float direction, float closing, bool hide)
    {
        GameObject transition = (isStartScreen ? transform : transform.parent).GetChild((isStartScreen ? transform : transform.parent).childCount - 1).gameObject;
        Material trans_mat = transition.GetComponent<Image>().material;
        transition.SetActive(true);
        for (float count = -1.2f; count < 1.2f; count += Time.deltaTime)
        {
            trans_mat.SetFloat("_end", count * direction);
            trans_mat.SetFloat("_start", (count + closing * 0.05f) * direction);

            yield return new WaitForEndOfFrame();
        }
        transition.SetActive(!hide);

    }
    public void SetTransition(Material mat) => (isStartScreen ? transform : transform.parent).GetChild((isStartScreen ? transform : transform.parent).childCount - 1).GetComponent<Image>().material = mat;

    public void BackToHome()
    {
        LoadProgress();
        difficulty = progress.level;
        StartCoroutine(ChangeScene("World"));
    }
    public void BackToMenu()
    {
        StartCoroutine(ChangeScene("StartScreen"));
    }
    private IEnumerator ChangeScene(string sceneName)
    {
        SetTransition(transition);
        DoTransition(1, 1, false);
        Mat_Intro = transition;
        yield return new WaitForSeconds(2.4f);
        SceneManager.LoadScene(sceneName);
        yield break;
    }

    public void OpenVerification()
    {
        if (SaveFileExists()) StartCoroutine(OpenVerif());
        else OpenLevelSelect();
    }
    IEnumerator OpenVerif()
    {
        EventSystem.current.SetSelectedGameObject(null);
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().interactable = false;

        transform.GetChild(1).gameObject.SetActive(true);

        CanvasGroup verifPanel = transform.GetChild(1).GetComponent<CanvasGroup>();
        verifPanel.alpha = 0;

        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            verifPanel.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        verifPanel.transform.GetChild(0).GetComponent<Animator>().Play("signAppear");
        yield break;
    }
    public void CloseVerification() => StartCoroutine(CloseVerif());
    IEnumerator CloseVerif()
    {
        CanvasGroup verifPanel = transform.GetChild(1).GetComponent<CanvasGroup>();
        verifPanel.alpha = 1;

        verifPanel.transform.GetChild(0).GetComponent<Animator>().Play("signDisappear");
        yield return new WaitForSeconds(.5f);

        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            verifPanel.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().interactable = true;
        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().interactable = true;

        transform.GetChild(1).gameObject.SetActive(false);
        yield break;
    }

    public void OpenLevelSelect() => StartCoroutine(OpenLevSelect());
    IEnumerator OpenLevSelect()
    {
        if (SaveFileExists()) StartCoroutine(CloseVerif());

        //Deaktiviere Buttons im Hintergrund:
        EventSystem.current.SetSelectedGameObject(null);
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().interactable = false;

        transform.GetChild(2).gameObject.SetActive(true);

        CanvasGroup selectPanel = transform.GetChild(2).GetComponent<CanvasGroup>();
        CanvasGroup selection = selectPanel.transform.GetChild(1).GetComponent<CanvasGroup>();
        selectPanel.alpha = 0;
        selection.alpha = 0;

        Animator bird = selectPanel.transform.GetChild(0).GetComponent<Animator>();
        bird.Play("HoldStartPos");

        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            selectPanel.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        selectPanel.transform.GetChild(0).GetComponent<Animator>().SetTrigger("start");
        yield return new WaitForSeconds(3);

        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            selection.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }
    public void CloseLevelSelect() => StartCoroutine(CloseLevSelect());
    IEnumerator CloseLevSelect()
    {
        CanvasGroup selectPanel = transform.GetChild(2).GetComponent<CanvasGroup>();
        selectPanel.alpha = 1;

        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            selectPanel.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().interactable = true;
        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().interactable = true;

        transform.GetChild(2).gameObject.SetActive(false);
        yield break;
    }

    public void StartNewGame(int level)
    {
        DeleteSaveFile();
        LoadProgress();
        difficulty = level;
        DontDestroyOnLoad(Instantiate(nextSeq, new Vector3(6.84f, 4.68f), Quaternion.identity));
        StartCoroutine(ChangeScene("Home"));
    }
}
