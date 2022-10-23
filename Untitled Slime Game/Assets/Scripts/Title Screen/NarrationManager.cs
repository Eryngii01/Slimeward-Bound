using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NarrationManager : MonoBehaviour {
    private static NarrationManager _instance;
    public static NarrationManager Instance {
        get {
            if (_instance == null)
                Debug.LogError("Narration Manager is null!");

            return _instance;
        }
    }

    [SerializeField]
    private GameObject _narrationMenu;

    [SerializeField]
    private Button _nextButton;

    [SerializeField]
    private TextMeshProUGUI _narrationText;

    [SerializeField]
    private Image _illustration;
    public float textDelay;

    private bool _isTyping;
    private string _fullText;

    public Queue<NarrationBase.Text> dialogueQueue = new Queue<NarrationBase.Text>();

    void Awake() {
        _instance = this;
    }

    public void EnqueueText(NarrationBase nBase)
    {
        dialogueQueue.Clear();

        foreach (NarrationBase.Text text in nBase.narrationInfo)
        {
            dialogueQueue.Enqueue(text);
        }

        DequeueText();
    }

    public void DequeueText()
    {
        // Trying to continue before finishing typing the script, so automatically finish the text
        if (_isTyping) {
            _narrationText.text = _fullText;
            StopAllCoroutines();
            _isTyping = false;

            return;
        } else if (dialogueQueue.Count == 1) {
            _nextButton.GetComponent<NarrationButtonController>().ChangeText();
        } else if (dialogueQueue.Count == 0) {
            StartCoroutine(LoadScene());
            return;
        }

        NarrationBase.Text text = dialogueQueue.Dequeue();

        _fullText = text.text;
        _illustration.sprite = text.illustration;

        _narrationText.text = "";
        StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(NarrationBase.Text info)
    {
        _isTyping = true;

        _narrationText.text = "";
        foreach (char letter in info.text.ToCharArray()) {
            yield return new WaitForSeconds(textDelay);
            _narrationText.text += letter;
            MusicManager.Instance.PlayType();
            yield return null;
        }

        _isTyping = false;
    }

    IEnumerator LoadScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Kitchen");
        _narrationMenu.SetActive(false);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}