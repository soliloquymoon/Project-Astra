using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    [SerializeField] Transform noteAppearLocation = null;
    [SerializeField] GameObject notePrefab = null;

    public RhythmChartParser chartParser;
    private Queue<float> noteTimesQueue = new Queue<float>();
    private Queue<RhythmChartParser.NoteData> notes = new Queue<RhythmChartParser.NoteData>();
    private Sprite[] noteSprites;

    private float musicStartTime = -1f;

    void Start()
    {
        noteSprites = new Sprite[2];
        noteSprites[0] = Resources.Load<Sprite>("Sprite/Note1");
        noteSprites[1] = Resources.Load<Sprite>("Sprite/Note2");

        string jsonContent = File.ReadAllText(Application.streamingAssetsPath + "/fallrain_takmfk.json");
        chartParser.LoadChart(jsonContent);
        // enqueue note spawn times
        foreach (var time in chartParser.ConvertNoteDataToTimes())
        {
            noteTimesQueue.Enqueue(time);
        }
        notes = chartParser.notes;

        musicStartTime = AudioManager.instance.GetMusicTime();
    }

    void Update()
    {
        float musicTime = AudioManager.instance.GetMusicTime();
        if (musicStartTime < 0f) return;

        float spawnTimeThreshold = 2.0f;
        // Check the peek of the queue
        if (noteTimesQueue.Count > 0 && musicTime >= noteTimesQueue.Peek() - spawnTimeThreshold)
        {
            RhythmChartParser.NoteData noteData = notes.Dequeue();
            float noteTime = noteTimesQueue.Dequeue(); // dequeue a note that has been spawned
            SpawnNote(noteTime, noteData.noteKey);

        }
    }

    void SpawnNote(float noteTime, int noteKey)
    {
        GameObject newNote = Instantiate(notePrefab, noteAppearLocation.position, Quaternion.identity);
        Note noteComponent = newNote.GetComponent<Note>();

        noteComponent.noteStartTime = noteTime - musicStartTime; // Music time when the note was instantiated
        noteComponent.targetTime = noteTime - musicStartTime; // Target time the note should arrive at the hit line
        noteComponent.startX = noteAppearLocation.position.x; // Initial x
        noteComponent.targetX = -650f; // 도착 위치

        Sprite noteSprite = (noteKey == 0 || noteKey == 1) ? noteSprites[noteKey] : null;
        noteComponent.setSprite(noteSprite);

        newNote.transform.SetParent(this.transform);
    }
}