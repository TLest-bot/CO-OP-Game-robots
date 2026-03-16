using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SequenceManager : MonoBehaviour
{
    [Header("Setup")]
    public List<WorldButton> worldButtons = new List<WorldButton>();

    [Header("Visuals")]
    public Color normalColor = Color.yellow;
    public Color pressedColor = Color.red;
    public float resetDelay = 1.0f;

    [Header("Logic")]
    public int sequenceLength = 4;
    public List<int> targetSequence = new List<int>();
    private List<int> currentInput = new List<int>();
    private bool isPlayingSequence = false;

    [Header("Succes/Fail")]
    public AudioSource VictorySound;
    public AudioSource Failsound;

    void Start()
    {
        // 1. Check of de lijst gevuld is VOORDAT we genereren
        if (worldButtons.Count == 0)
        {
            Debug.LogError("ERROR: De worldButtons lijst is leeg! Sleep de Triggers in de Inspector.");
            return;
        }

        GenerateNewSequence();

        // 2. Zet alle knoppen op de begin kleur
        foreach (var btn in worldButtons)
        {
            if (btn != null) btn.SetColor(normalColor);
        }
    }

    public void OnButtonStepped(int id, WorldButton button)
    {
        // Voorkom dat de speler input geeft terwijl de Replay bezig is
        if (isPlayingSequence) return;

        StartCoroutine(HandleButtonFeedback(button));
        ReceiveInput(id);
    }

    IEnumerator HandleButtonFeedback(WorldButton button)
    {
        button.SetPressState(true);
        button.PlaySound();
        button.SetColor(pressedColor);

        yield return new WaitForSeconds(resetDelay);

        button.SetColor(normalColor);
        button.SetPressState(false);
    }

    public void PlayTargetSequenceSounds()
    {
        if (!isPlayingSequence) StartCoroutine(SequencePlaybackRoutine());
    }

    IEnumerator SequencePlaybackRoutine()
    {
        isPlayingSequence = true;
        Debug.Log("Systeem laat volgorde zien...");

        foreach (int id in targetSequence)
        {
            // Zoek de button op basis van het ID dat we in de Inspector hebben gegeven
            WorldButton targetBtn = worldButtons.Find(b => b.myID == id);

            if (targetBtn != null)
            {
                targetBtn.PlaySound();
                targetBtn.SetColor(pressedColor);
                yield return new WaitForSeconds(1.2f);
                targetBtn.SetColor(normalColor);
            }
            yield return new WaitForSeconds(0.2f);
        }

        isPlayingSequence = false;
    }

    public void ReceiveInput(int number)
    {
        currentInput.Add(number);
        Debug.Log("Huidige input: " + string.Join(", ", currentInput));

        if (currentInput.Count == targetSequence.Count)
        {
            if (currentInput.SequenceEqual(targetSequence))
            {
                Debug.Log("<color=green>CODE CORRECT!</color>");
                HandleSuccess();
            }
            else
            {
                StartCoroutine(HandleWrongInput());
            }
        }
    }

    private IEnumerator HandleWrongInput()
    {
        Debug.Log("<color=red>CODE FOUT!</color> Probeer opnieuw.");
        currentInput.Clear();

        foreach (WorldButton button in worldButtons)
        {
            button.SetColor(pressedColor);
            button.SetPressState(true);
        }

        Failsound.Play();

        yield return new WaitForSeconds(2f);

        foreach (WorldButton button in worldButtons)
        {
            button.SetColor(normalColor);
            button.SetPressState(false);
        }
    }

    void HandleSuccess()
    {
        VictorySound.Play();
        GenerateNewSequence();
    }

    void GenerateNewSequence()
    {
        targetSequence.Clear();
        currentInput.Clear();

        for (int i = 0; i < sequenceLength; i++)
        {
            int randomID = Random.Range(1, worldButtons.Count - 1);
            targetSequence.Add(randomID);
        }

        Debug.Log("Nieuwe sequence gegenereerd: " + string.Join(", ", targetSequence));
    }

    public IEnumerator HandleReplayButtonFlash(WorldButton button)
    {
        button.SetPressState(true);
        button.SetColor(pressedColor);
        yield return new WaitForSeconds(0.5f);
        button.SetColor(normalColor);
        button.SetPressState(false);
    }
}