using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioUtility : MonoBehaviour{
    public AudioClip MyAudioClip;
    private float[] microphoneBuffer;
    private int head;
    private int position;
    private ClientTcp clientTcp;
    private const float rescaleFactor = 32767; //to convert float to Int16
    public float dbTemp;
    public Boolean isConnect = false;
    public Renderer rend;
    public int int_result;

    void Start(){
        clientTcp = GetComponent<ClientTcp>();
        StartCoroutine(WavRecording(null, 1200, 16000));
    }

    private void Update(){
        //Debug.Log(str_result);
        if(clientTcp.result != ""){
            int_result = Convert.ToInt16(clientTcp.result);
        }else{
            int_result = 0;
        }
    }

    private IEnumerator WavRecording(string micDeviceName, int maxRecordingTime, int samplingFrequency)
    {
        //Buffer
        microphoneBuffer = new float[maxRecordingTime * samplingFrequency];
        //録音開始
        MyAudioClip = Microphone.Start(null, true, maxRecordingTime, samplingFrequency);

        //初期位置
        head = 0;
        while (true){
            //ディバイス待ち（レンテシー　null）
            position = Microphone.GetPosition(null);
            if (position < 0 || head == position){
                yield return null;
            }
            else{
                wavBufferWrite(head, position, MyAudioClip);
                head = position;
            }
            yield return null;
        }
    }

    private float wavBufferWrite(int _head, int _position, AudioClip _clip){
        //Bufferに音声データを取り込み
        _clip.GetData(microphoneBuffer, 0);

        //音声データをFileに追加
        if (_head < _position){
            sendWaveData(_head, _position);
            dbTemp = getVolume(_head, _position);
            return dbTemp;
        }else{
            sendWaveData(_head, microphoneBuffer.Length);
            sendWaveData(0, _position);
            float db_1 = getVolume(_head, microphoneBuffer.Length);
            float db_2 = getVolume(0, _position);
            dbTemp = (db_1 + db_2) / 2f;
            return dbTemp;
        }
    }

    private void sendWaveData(int start, int end){

        for (int i = start; i < end; i++){
            Byte[] _buffer = BitConverter.GetBytes((short)(microphoneBuffer[i] * rescaleFactor));
            try{
                clientTcp.MySend(_buffer);
                isConnect = true;
            }catch{
                isConnect = false;
                return;
            }
        }
    }

    private float getVolume(int start, int end){
        float sum = 0;
        float rmsValue;
        float dbValue;
        for (int i = start; i < end; i++){
            sum += microphoneBuffer[i] * microphoneBuffer[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / (end - start)); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / 1); // calculate dB
        if (dbValue < -80) dbValue = -80; // clamp it to -80dB min
        if (dbValue > 0) dbValue = 0; // clamp it to 0dB max

        return dbValue + 120; // fix 40~120 dB value
    }
}
