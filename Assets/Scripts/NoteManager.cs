using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d;

    [SerializeField] Transform noteAppearLocation = null;
    [SerializeField] GameObject notePrefab = null;

    public RhythmChartParser chartParser;
    private List<float> noteTimes; // Note timing list
    private bool[] noteSpawned;    // Check if note has been spawned

    void Start()
    {
        chartParser.LoadChart(Application.streamingAssetsPath + "/akiame.txt");
        noteTimes = new List<float>();
        foreach (var note in chartParser.notes)
        {
            noteTimes.AddRange(chartParser.ConvertNoteDataToTimes(note));
        }

        noteTimes.Sort();
        noteSpawned = new bool[noteTimes.Count];
    }

    void Update()
    {
        float musicTime = AudioManager.instance.GetMusicTime();
        for (int i = 0; i < noteTimes.Count; i++)
        {
            if (!noteSpawned[i] && musicTime >= noteTimes[i] - 2.0f)  // Create note 2 seconds ahead
            {
                SpawnNote();
                noteSpawned[i] = true;
            }
        }
    }

    void SpawnNote()
    {
        GameObject newNote = Instantiate(notePrefab, noteAppearLocation.position, Quaternion.identity);
        newNote.transform.SetParent(this.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Note"))
        {
            Destroy(collision.gameObject);
        }
    }
}
