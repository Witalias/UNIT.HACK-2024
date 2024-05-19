using TMPro;
using UnityEngine;

public class TaskPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _notesText;

    private int _totalNotes;
    private int _collectedNotesCount;

    private void Awake()
    {
        _totalNotes = FindObjectsByType<NoteObject>(FindObjectsSortMode.None).Length;
        UpdateText();
    }

    private void OnEnable()
    {
        NoteObject.NoteCollected += AddNote;
    }

    private void OnDisable()
    {
        NoteObject.NoteCollected -= AddNote;
    }

    private void AddNote()
    {
        _collectedNotesCount++;
        UpdateText();
    }

    private void UpdateText()
    {
        _notesText.text = $"Найди записки: <color=yellow><b>{_collectedNotesCount} / {_totalNotes}</b></color>";
    }
}
