using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.MathHelpers;

public class GameManager : MonoBehaviour
{
    AudioSource aSrc;
    AudioClip clip_Name_normal, clip_Name_question;
    public AnimationCurve questionPitch;
    public AnimationCurve ShoutPitch;
    public AnimationCurve ExplainPitch;
    public AnimationCurve MumblePitch;

    // Start is called before the first frame update
    void Start()
    {
        aSrc = Camera.main.GetComponent<AudioSource>();
    }

    public void PlayName()
    {
        aSrc.clip = clip_Name_normal;
        aSrc.Play();
    }

    public void PlayQuestion()
    {
        StartCoroutine(PlayPitch(questionPitch));
    }
    public void PlayShout()
    {
        StartCoroutine(PlayPitch(ShoutPitch));
    }
    public void PlayExplain()
    {
        StartCoroutine(PlayPitch(ExplainPitch));
    }
    public void PlayMumble()
    {
        StartCoroutine(PlayPitch(MumblePitch));
    }

    IEnumerator PlayPitch(AnimationCurve curve)
    {
        Debug.Log("Question");
        aSrc.clip = clip_Name_normal;
        float timeCount = 0;
        aSrc.PlayDelayed(0.5f);
        yield return new WaitForSeconds(0.5f);
        while (aSrc.isPlaying)
        {
            aSrc.pitch = curve.Evaluate(timeCount/aSrc.clip.length);
            yield return new WaitForFixedUpdate();
            timeCount += Time.fixedDeltaTime;
        }
        aSrc.pitch = 1;
        Debug.Log("question ended");
    }



    public void StartListenForSound()
    {
        StartCoroutine(GetSoundSample());
    }

    IEnumerator GetSoundSample(int freq = 44100, float sampleRate = 0.2f)
    {
        float[] sample = new float[freq * 3];
        Complex[] spec = new Complex[2048];
        float sum;
        int ptr = 0;

        int start = 0, end = 0;
        bool voiceHasStarted = false;
        bool voiceHasStopped = false;

        aSrc.clip = Microphone.Start(null, true, 5, freq);
        while(!(voiceHasStarted && voiceHasStopped))
        {
            yield return new WaitForSeconds(sampleRate);
            aSrc.clip.GetData(sample, ptr);

            //copy input data into array
            for (int i = 0; i < 2048; i++)
            {
                if (ptr + i >= sample.Length) ptr -= sample.Length;
                spec[i] = new Complex(sample[ptr + i], 0);
            }
            
            //Get spectrum:
            FFT.CalculateFFT(spec, false);

            sum = 0;
            for(int i = 0; i < 1024; i++){ sum += spec[i].fSqrMagnitude; }

            //Auswertung:
            Debug.Log(sum);
            if (sum > 1e-6f)//wenn spektrale leistung über den grenzwert geht:
            {
                if (!voiceHasStarted)//wahrscheinlich überlappend, daher wird der Start um einen Durchgang zurückverschoben
                {
                    start = ptr - (int)(sampleRate * 3 * freq);
                    if (start < 0) start += sample.Length;
                    Debug.Log("start at: " + start);
                }
                ptr = Microphone.GetPosition(null);
                voiceHasStarted = true;
                voiceHasStopped = false;
            }
            else
            {
                ptr = Microphone.GetPosition(null);
                end = ptr;
                voiceHasStopped = true;
            }

        }
        Microphone.End(null);
        //end -= (int)(sampleRate * 2 * freq);
        Debug.Log("stop");

        //Bereite Soundclips for:
        int dataLength = end > start ? end - start : sample.Length - start + end;

        float[] tmp = new float[dataLength];
        for(int i = 0; i < dataLength; i++)
        {
            if (start + i >= sample.Length) start -= sample.Length;
            tmp[i] = sample[start + i];
        }

        //Soundclip:
        //Laufe aufnahme nochmals ab und ermittle den genauen Start- und Stoppunkt
        start = -1;
        end = -1;
        bool signal = false;
        int stepSize = (int)(freq * 0.01f);
        List<Complex> currentData = new List<Complex>();
        
        for(int i = 0; i < 2048; i++) currentData.Add(new Complex(tmp[i], 0));

        Debug.Log("ermittle genaue Position...");
        for(int i = 0; i < tmp.Length-2048; i+= stepSize)
        {
            //Test:
            sum = 0;
            foreach (Complex val in FFT.CalculateFFT(currentData.ToArray(), false)) sum += val.fSqrMagnitude;//nyquist führt zu doppelter spec. Leistung
            signal = sum > 1e-6f;
            if (signal && start == -1) start = i;
            else if(!signal && start != -1)
            {
                end = i + 2048;
                break;
            }

            //Ändere Werte:
            if (i + stepSize + 2048 >= tmp.Length) break;
            for(int j = 0; j < stepSize; j++) { currentData.RemoveAt(0); currentData.Add(new Complex(tmp[i + j + 2048], 0)); }
        }
        if (start == -1) start = 0;
        if (end == -1) end = tmp.Length;

        float[] data = new float[end - start];
        for (int i = 0; i < end - start; i++) data[i] = tmp[start + i];
        Debug.Log("genaue Position ermittelt: start: " + start + ", ende: " + end);

        clip_Name_normal = AudioClip.Create("name_normal", data.Length, 1, freq, false);
        clip_Name_normal.SetData(data, 0);
        Debug.Log("name_normal created");

        aSrc.clip = clip_Name_normal;
        aSrc.Play();
    }
}
