using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioTimeProvider : MonoBehaviour
{
    public float AudioTime; //notes get this value
    public List<double> SVList = new();
    public List<double> SVTime = new();
    private List<Func<double, double>> positionFunctions = new List<Func<double, double>>();
    private List<double> segmentStarts = new List<double>();
    public double ScrollDist; //表示当前状况在正常流速且无SC下的时间，没有出现scene control的时候与Audio Time相同
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
            ScrollDist = GetPositionAtTime(AudioTime);
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

    public void CalcSVPos()
    {
        double lastPosition = 0;
        double lastTime = 0;
        double lastSpeed = 1;
        positionFunctions.Add((t) => lastPosition + lastSpeed * (t - lastTime));
        segmentStarts.Add(lastPosition);
        for (int i = 0; i < SVTime.Count; i++)
        {
            double segmentDuration = SVTime[i] - lastTime;  // 当前区间持续的时间
            double speed = SVList[i];  // 当前区间的速度

            // 创建分段函数：Position(t) = Position_i + Speed_i * (t - SVTime[i])
            Func<double, double> segmentFunction = (t) =>
            {
                return lastPosition + lastSpeed * (t - lastTime);
            };

            positionFunctions.Add(segmentFunction);
            segmentStarts.Add(lastPosition);
            lastPosition += lastSpeed * segmentDuration;
            lastTime = SVTime[i];
            lastSpeed = speed;
        }
    }

    public double GetPositionAtTime(double AudioT)
    {
        // 如果 AudioT 小于第一个速度变化时刻，直接用初始速度计算位置
        if (AudioT < SVTime[0])
            return AudioT;
        // 查找对应的区间
        for (int i = 0; i < SVTime.Count; i++)
        {
            if (AudioT < SVTime[i])
            {
                // 找到对应的区间，计算位置
                return positionFunctions[i](AudioT);
            }
        }
        return positionFunctions[SVTime.Count - 1](AudioT);
    }
}