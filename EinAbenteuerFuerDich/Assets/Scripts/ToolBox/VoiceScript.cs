using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.MathHelpers;

public class VoiceScript : MonoBehaviour
{
    AudioSource aSrc;
    AudioClip clip_Name_normal, clip_Name_question;
    public AnimationCurve questionPitch;
    public AnimationCurve ShoutPitch;
    public AnimationCurve ExplainPitch;
    public AnimationCurve MumblePitch;

    static int sampleLength = 5;
    public static bool fetching { get; private set; }
    public static bool runFetch;

    public static class Signal
    {
        public static float amplitude;
        public static float[] frequency;
        public static string dominantFrequency;
        public static float power;
        public static float length;
    }


    // Start is called before the first frame update
    void Start()
    {
        Signal.frequency = new float[85];

        aSrc = Camera.main.GetComponent<AudioSource>();

        //StartCoroutine(ReportFrequencies());
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
            aSrc.pitch = curve.Evaluate(timeCount / aSrc.clip.length);
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

    /// <summary>
    /// Nimmt den Sound auf bis ein Soundsample erkannt wird. Schneidet die Tonspur zurecht
    /// </summary>
    /// <param name="freq">Die frequenz, mit der gesampelt wird</param>
    /// <param name="sampleRate">Schrittgröße in Sekunden, mit der gesampelt wird</param>
    /// <returns></returns>
    IEnumerator GetSoundSample(int freq = 44100, float sampleRate = 0.2f)
    {
        if (runFetch)
        {
            runFetch = false;
            yield return new WaitWhile(()=>fetching);
        }
        runFetch = true;

        float[] sample = new float[freq * sampleLength];
        Complex[] spec = new Complex[2048];
        float maxAmp, maxAmp_together = 0, sample_val;
        float sum;
        int ptr = 0;

        int start = 0, end = 0;
        bool voiceHasStarted = false;
        bool voiceStopping = false;
        bool voiceHasStopped = false;

        //Create Hamming Window:
        float[] hamming = CreateHamming(spec.Length);

        aSrc.clip = Microphone.Start(null, true, sampleLength, freq);
        maxAmp_together = 0;
        fetching = true;
        while (!(voiceHasStarted && voiceHasStopped) || !runFetch)
        {
            yield return new WaitForSeconds(sampleRate);
            aSrc.clip.GetData(sample, ptr);

            //copy input data into array
            maxAmp = 0;
            for (int i = 0; i < 2048; i++)
            {
                if (ptr + i >= sample.Length) ptr -= sample.Length;
                sample_val = sample[ptr + i];
                maxAmp = maxAmp < Mathf.Abs(sample_val) ? Mathf.Abs(sample_val) : maxAmp;
                spec[i] = new Complex(sample_val * hamming[i], 0);
            }

            sum = 0;
            if (maxAmp > 0.01f)
            {
                //https://www.dpamicrophones.de/mikrofon-universitaet/fakten-zur-sprachverstaendlichkeit#:~:text=sind%20%C3%BCberwiegend%20im%20Frequenzbereich%20oberhalb,2%20kHz%20bis%204%20kHz.&text=Dem%20Spektrum%20von%20auf%20der,Bereich%20von%202%20%2D%204%20kHz.
                //ein bin entspricht 22050 / 1024 = 21,5Hz
                //Get spectrum:
                FFT.CalculateFFT(spec, false);
                for (int i = 8; i < 512; i++) { sum += spec[i].fSqrMagnitude; }//normaler Weise bis 1024 -> Stimmbereich liegt jedoch meißt nur bis 8kHz
            }
            else
            {
                //no signal or to quiet
            }

            //Auswertung:
            Debug.Log("sum: " + sum + ", maxA: " + maxAmp);
            if (sum > 1e-4f)//wenn spektrale leistung über den grenzwert geht:
            {
                if (!voiceHasStarted)//wahrscheinlich überlappend, daher wird der Start um einen Durchgang zurückverschoben
                {
                    start = ptr - (int)(sampleRate * 4 * freq);
                    if (start < 0) start += sample.Length;
                    Debug.Log("start at: " + start);
                }
                ptr = Microphone.GetPosition(null);
                voiceHasStarted = true;
                voiceStopping = false;
                voiceHasStopped = false;
                maxAmp_together = maxAmp > maxAmp_together ? maxAmp : maxAmp_together;
            }
            else
            {
                ptr = Microphone.GetPosition(null);
                end = ptr;
                voiceHasStopped = voiceStopping;
                voiceStopping = true;
            }

        }
        Microphone.End(null);
        //end -= (int)(sampleRate * 2 * freq);
        Debug.Log("stop");
        fetching = false;
        if (!runFetch) yield break;
        runFetch = false;

        //Bereite Soundclips for:
        int dataLength = end > start ? end - start : sample.Length - start + end;
        Debug.Log("maxAmp: " + maxAmp_together);
        if (maxAmp_together != 0) maxAmp_together = 0.5f / maxAmp_together;
        Debug.Log("amplify by : " + maxAmp_together);
        float[] tmp = new float[dataLength];
        for (int i = 0; i < dataLength; i++)
        {
            if (start + i >= sample.Length) start -= sample.Length;
            tmp[i] = sample[start + i] * maxAmp_together;
        }

        //Soundclip:
        //Laufe aufnahme nochmals ab und ermittle den genauen Start- und Stoppunkt
        start = -1;
        end = -1;
        bool signal = false;
        int stepSize = (int)(freq * 0.02f);
        List<Complex> currentData = new List<Complex>();

        for (int i = 0; i < 2048; i++) currentData.Add(new Complex(tmp[i], 0));

        Debug.Log("ermittle genaue Position...");
        for (int i = 0; i < tmp.Length - 2048; i += stepSize)
        {
            //Test:
            sum = 0;
            Complex[] fftData = FFT.CalculateFFT(currentData.ToArray(), false);
            for (int j = 8; j < 512; j++) sum += fftData[j].fSqrMagnitude;
            signal = sum > 1e-5f;
            if (signal && start == -1) start = i;
            else if (!signal && start != -1)
            {
                end = i + 2048;
                break;
            }

            //Ändere Werte:
            if (i + stepSize + 2048 >= tmp.Length) break;
            for (int j = 0; j < stepSize; j++) { currentData.RemoveAt(0); currentData.Add(new Complex(tmp[i + j + 2048], 0)); }
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

    /// <summary>
    /// Gibt die stärkste Frequenz aus, bis die Funktion wieder deaktiviert wird
    /// </summary>
    /// <param name="freq"></param>
    /// <param name="sampleRate"></param>
    /// <param name="minFreq"></param>
    /// <param name="maxFreq"></param>
    /// <returns></returns>
    public static IEnumerator ReportFrequencies(int freq = 8000, float sampleRate = 0.05f, string minTone = "C1", string maxTone = "C8", float thres = 1e-6f )
    {
        if (runFetch)
        {
            runFetch = false;
            yield return new WaitWhile(() => fetching);
        }
        runFetch = true;

        float[] sample = new float[freq * sampleLength];
        Complex[] spec = new Complex[2048];
        float maxAmp, sample_val, maxFreqPower, freqPower;
        float activeFrequency, stepFrequency = freq / (float)spec.Length;
        float sum;
        int ptr = 0;

        int activeTone, startTone = ToneLookUp.NameToIndex[minTone], maxFrequency;
        int minFreqIndex = (int)(ToneLookUp.IndexToRange[startTone] / stepFrequency);
        int maxFreqIndex = (int)(ToneLookUp.IndexToRange[ToneLookUp.NameToIndex[maxTone]] / stepFrequency);
        if (maxFreqIndex > freq / 2) maxFreqIndex = freq / 2;

        //Create Hamming:
        float[] hamming = CreateHamming(spec.Length);

        AudioClip myClip = Microphone.Start(null, true, sampleLength, freq);
        fetching = true;
        while (runFetch)
        {
            yield return new WaitForSeconds(sampleRate);
            myClip.GetData(sample, ptr);
            ptr = Microphone.GetPosition(null);

            //copy input data into array
            maxAmp = 0;
            for (int i = spec.Length -1; i >= 0; i--)
            {
                sample_val = ptr - i < 0? sample[ptr - i + sample.Length] : sample[ptr - i];
                maxAmp = maxAmp < Mathf.Abs(sample_val) ? Mathf.Abs(sample_val) : maxAmp;
                spec[spec.Length-1 - i] = new Complex(sample_val * hamming[i], 0);
            }

            sum = 0;
            maxFreqPower = 0;
            maxFrequency = -1;

            //Get spectrum:
            FFT.CalculateFFT(spec, false);
            activeTone = startTone;
            activeFrequency = ToneLookUp.IndexToRange[startTone];
            for (int i = 0; i < Signal.frequency.Length; i++) Signal.frequency[i] = 0;
            for (int i = minFreqIndex; i < maxFreqIndex; i++)
            {
                freqPower = spec[i].fSqrMagnitude;
                sum += freqPower;

                if (activeFrequency > ToneLookUp.IndexToRange[activeTone + 1])
                {
                    if (maxFreqPower == 0 && Signal.frequency[activeTone] > thres) { maxFreqPower = Signal.frequency[activeTone]; maxFrequency = activeTone; }
                    activeTone++;
                }
                Signal.frequency[activeTone] += freqPower;

                activeFrequency += stepFrequency;
            }


            //Auswertung:
            
            Signal.power = sum;
            Signal.dominantFrequency = maxFrequency != -1 ? ToneLookUp.IndexToName[maxFrequency] : "";
            Signal.amplitude = maxAmp;
            Signal.length = maxFrequency != -1 ? Signal.length + sampleRate : 0;
            if(maxFrequency != -1) Debug.Log("freq: " + Signal.dominantFrequency + ", freqrange: " + ToneLookUp.IndexToRange[ToneLookUp.NameToIndex[Signal.dominantFrequency]]);
        }
        Microphone.End(null);

        fetching = false;
        runFetch = false;
        yield break;
    }

    /// <summary>
    /// Nimmt nur die Soundamplitude auf, ist aber dafür schnell
    /// </summary>
    /// <param name="freq"></param>
    /// <param name="sampleRate"></param>
    /// <returns></returns>
    public static IEnumerator FetchAmpOnly(int freq = 16000, float sampleRate = 0.02f)
    {
        if (runFetch)
        {
            runFetch = false;
            yield return new WaitWhile(() => fetching);
        }
        runFetch = true;

        int sampleCount = (int)(freq * sampleRate);
        float[] sample = new float[sampleCount];
        float maxAmp;

        AudioClip myClip = Microphone.Start(null, true, sampleLength, freq);
        fetching = true;
        while (runFetch)
        {
            yield return new WaitForSeconds(sampleRate);
            myClip.GetData(sample, 0);

            //copy input data into array
            maxAmp = 0;
            for (int i = 0; i < sampleCount; i++)
                maxAmp = maxAmp < Mathf.Abs(sample[i]) ? Mathf.Abs(sample[i]) : maxAmp;
            Signal.amplitude = maxAmp;
        }

            yield break;
    }

    /// <summary>
    /// Erstellt ein Array zur Fensterung der Daten nach Hamming
    /// </summary>
    /// <param name="size">Fenster-Größe</param>
    /// <returns></returns>
    static float[] CreateHamming(int size)
    {
        float[] hamming = new float[size];
        int a = size - 1;
        for (int i = 0; i < a; i++)
        {
            hamming[i] = 0.54f - 0.46f * Mathf.Cos(2 * Mathf.PI * i / (size - 1));
            hamming[a--] = hamming[i];
        }
        return hamming;
    }
}

