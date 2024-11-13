﻿using System;
using UnityEngine;

public class AudioTimeProvider : MonoBehaviour
{
    public float AudioTime; //notes get this value
    public float RealTime;
    public float ScrollDist; //表示当前状况在正常流速且无SC下的时间，没有出现scene control的时候与Audio Time相同
    public bool isStart;
    public bool isRecord;
    public float offset;
    private float speed;

    private float startTime;
    private long ticks;

    public float CurrentSpeed => isRecord ? Time.timeScale : speed;

    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (isStart)
        {
            if (isRecord)
                AudioTime = Time.time - startTime + offset;
            else
                AudioTime = (Time.realtimeSinceStartup - startTime) * speed + offset;
        }
    }

    public void SetStartTime(long _ticks, float _offset, float _speed, bool _isRecord = false)
    {
        ticks = _ticks;
        offset = _offset;
        AudioTime = offset;
        var dateTime = new DateTime(ticks);
        var seconds = (dateTime - DateTime.Now).TotalSeconds;
        isRecord = _isRecord;
        if (_isRecord)
        {
            startTime = Time.time + 5;
            Time.timeScale = _speed;
            Time.captureFramerate = 60;
        }
        else
        {
            startTime = Time.realtimeSinceStartup + (float)seconds;
            speed = _speed;
            Time.captureFramerate = 0;
        }

        isStart = true;
    }

    public void ResetStartTime()
    {
        offset = 0f;
        isStart = false;
    }
}