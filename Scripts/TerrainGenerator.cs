using Godot;
using System;
using System.Collections.Generic;

public class TerrainGenerator
{

    public float scale;

    private OpenSimplexNoise _osn;
    public void init()
    {
        scale = 0.5f;
        
        _osn = new OpenSimplexNoise();
        _osn.Period = 16;
    }



    public float GetValue(float x,float y,float z)
    {
        float scale2 = 1;
        var noiseVal = _osn.GetNoise3d(x*scale2,y*scale2,z*scale2);
        //var mix = Mathf.Clamp(y*scale,0,1);
        //noiseVal = Mathf.Lerp(1,noiseVal,1-mix);
        if(y*scale<=2)
            noiseVal = Mathf.Lerp(-1,noiseVal,y*scale/5);
        if(y>=16*4-1)
            noiseVal = 0;
        return noiseVal;
    }

}