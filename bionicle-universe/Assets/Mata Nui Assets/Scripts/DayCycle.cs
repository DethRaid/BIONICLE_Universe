using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DayCycle : MonoBehaviour {
    /*!The time since midnight, in hours. There are 18 hours in a days*/
    public static float time = 9;
    /*!The number of days since the first of the year. There are 300 days in a year*/
    public static int day = 1;
    /*!The number of years since the Matoran moved to Mata Nui*/
    public static int year = 1;
    /*!The number of lightmaps in the scene. Be sure to update this when multiple scenes become a thing*/
    public static int lightmapNums;
    /*!Multiplied by Time.deltaTime to advance DayCycle.time*/
    public float timeScale;

    public float _time;
    public int _day;
    public int _year;

    public TimedLightmap dayLM, night;

    public void Awake() {
        dayLM = new TimedLightmap( "Ga-Koro_Day" );
        night = new TimedLightmap( "Ga-Koro_Night" );
    }

    public void Update() {
        time += Time.deltaTime * timeScale / 3600;
        if( time > 18 ) {
            time = 0;
            day++;
            if( day > 300 ) {
                day = 1;
                year++;
            }
        }
        _time = time;
        _day = day;
        _year = year;
        if( time > dayLM.startTime ) {
            LightmapSettings.lightmaps = dayLM.map;
        } else if( time > night.startTime ) {
            LightmapSettings.lightmaps = night.map;
        }
    }
}

public class TimedLightmap {
    public float startTime; //When should this lightmap start being active?
    public LightmapData[] map;

    /*!name is expected to be the name of the folder (in the Resources\Lightmaps folder) where the lightmap we want can be found*/
    public TimedLightmap( string name ) {
        List<LightmapData> data = new List<LightmapData>();
        for( int i = 0; i < DayCycle.lightmapNums; i++ ) {
            LightmapData ld = new LightmapData();
            ld.lightmapFar = Resources.Load( "Lightmaps/" + name + "/LightmapFar-" + i ) as Texture2D;
            ld.lightmapNear = Resources.Load( "Lightmaps/" + name + "/LightmapNear-" + i ) as Texture2D;
            data.Add( ld );
        }
        map = data.ToArray();
    }
}