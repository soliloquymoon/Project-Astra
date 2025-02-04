using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    [SerializeField] Transform noteAppearLocation = null;
    [SerializeField] GameObject notePrefab = null;

    public RhythmChartParser chartParser;
    private Queue<float> noteTimesQueue = new Queue<float>();

    void Start()
    {
        string jsonContent = File.ReadAllText(Application.streamingAssetsPath + "/fallrain_takmfk.json");
        chartParser.LoadChart(jsonContent);
        // enqueue note spawn times
        foreach (var time in chartParser.ConvertNoteDataToTimes())
        {
            noteTimesQueue.Enqueue(time);
        }
    }

    void Update()
    {
        float musicTime = AudioManager.instance.GetMusicTime();

        // Check the peek of the queue
        if (noteTimesQueue.Count > 0 && musicTime >= noteTimesQueue.Peek())
        {
            SpawnNote();
            noteTimesQueue.Dequeue(); // dequeue a note that has been spawned
        }
    }

    void SpawnNote()
    {
        GameObject newNote = Instantiate(notePrefab, noteAppearLocation.position, Quaternion.identity);
        newNote.transform.SetParent(this.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            Destroy(collision.gameObject);
        }
    }
}