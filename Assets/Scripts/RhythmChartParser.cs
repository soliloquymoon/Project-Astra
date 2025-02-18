using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class RhythmChartParser : MonoBehaviour
{
    [Serializable]
    public class ChartData
    {
        public string title;
        public string artist;
        public float waveoffset;
        public float bpm;
        public int tpb;
        public int lastmeasure;
        public int[] measure;
        public string[] chart;
    }

    [Serializable]
    public class NoteData
    {
        public float time; // Tick -> s
        public int laneNumber;
        public int noteType;
        public int noteKey;

        public NoteData(float time, int laneNumber, int noteType, int noteKey)
        {
            this.time = time;
            this.laneNumber = laneNumber;
            this.noteType = noteType;
            this.noteKey = noteKey;
        }
    }

    public float offset = 0.0f;  // WAV Offset (sec)
    public float bpm = 140f;     // default BPM
    public int tpb = 480;        // Tick Per Beat
    public Queue<NoteData> notes = new Queue<NoteData>();

    void Start()
    {
        
    }

    public void LoadChart(string jsonContent)
    {
        ChartData chartData = JsonUtility.FromJson<ChartData>(jsonContent);

        if (chartData == null)
        {
            Debug.LogError("Failed to parse JSON file.");
            return;
        }

        bpm = chartData.bpm;
        tpb = chartData.tpb;
        offset = chartData.waveoffset;

        foreach (string entry in chartData.chart)
        {
            string[] parts = entry.Split('#');
            if (parts.Length != 2) continue;
            if (int.TryParse(parts[0], out int tick))
            {
                float tickToTime = ConvertTickToTime(tick);
                Debug.Log(parts[1]);
                notes.Enqueue(new NoteData(tickToTime, int.Parse(parts[1].Substring(0, 2)),
                    int.Parse(parts[1].Substring(2, 1)), int.Parse(parts[1].Substring(3, 1))));
            }
        }

        Debug.Log("Chart loading completed. Total combo: " + notes.Count);
    }

    public List<float> ConvertNoteDataToTimes()
    {
        List<float> noteTimes = new List<float>();
        foreach (var note in notes)
        {
            noteTimes.Add(note.time);
        }
        return noteTimes;
    }

    public float ConvertTickToTime(int tick)
    {
        float seconds = tick * 60 / (bpm * tpb);
        return seconds + offset;
    }
}