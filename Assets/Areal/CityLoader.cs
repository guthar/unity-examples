using Dummiesman;
using GeoJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class CityLoader : MonoBehaviour
{
    public TextAsset encodedGeoJSON;

    public GeoJSON.FeatureCollection collection;

    private const string AREAL_PATH = @"C:\Hackathon\rsg_hackathon2019_team6 - Copy\Assets\Areal";

    public bool hasExecuted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        GameObject lastObject = null;
        if (!hasExecuted)
        {
            GPSEncoder.SetLocalOrigin(new Vector2(48.32352f, 14.25904f));
            collection = GeoJSON.GeoJSONObject.Deserialize(encodedGeoJSON.text);
            foreach (var area in collection.features)
            //var area = collection.features[0];
            {
                if (area.properties.ContainsKey("path"))
                {
                    var areaJsonPath = AREAL_PATH + "\\" + area.properties["path"];
                    var areaPath = AREAL_PATH + "\\" + area.properties["id"];
                    var areaJsonContent = File.ReadAllText(areaJsonPath);
                    var areas = GeoJSON.GeoJSONObject.Deserialize(areaJsonContent);
                    foreach (var areaBuildings in areas.features)
                    {
                        var coordinates = areaBuildings.properties["origin"].Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
                        var longitude = float.Parse(coordinates[0]);
                        var latitude = float.Parse(coordinates[1]);

                        lastObject = new OBJLoader().Load(areaPath + "\\" + areaBuildings.properties["models"].Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries)[0]);

                        var position = GPSEncoder.GPSToUCS(latitude, longitude);
                        lastObject.transform.position = position;
                        lastObject.transform.Rotate(0, 180, 0);

                        lastObject.GetComponentInChildren<Renderer>().material.color = Random.ColorHSV(0.2f, 0.4f, 0.4f, 0.5f, 1f, 1f);

                        lastObject.AddComponent<MeshFilter>();

                        var mesh = lastObject.GetComponent<MeshFilter>().mesh;
                        if (mesh != null)
                        {
                            MeshCollider meshCollider = lastObject.AddComponent<MeshCollider>();
                            meshCollider.sharedMesh = mesh;
                        }
                    }
                }
            }
            hasExecuted = true;
        }
    }

    private void OnGUI()
    {

        if (GUI.Button(new Rect(201,0,200,200), "Prefab this"))
        {

        }

        // Ausgangspunkt für Koordinaten setzen
        GPSEncoder.SetLocalOrigin(new Vector2(48.32352f, 14.25904f));
        GameObject lastObject = null;
        if (encodedGeoJSON != null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 200), "Generate Models"))
            {
                collection = GeoJSON.GeoJSONObject.Deserialize(encodedGeoJSON.text);
                foreach (var area in collection.features)
                //var area = collection.features[0];
                {
                    if (area.properties.ContainsKey("path"))
                    {
                        var areaJsonPath = AREAL_PATH + "\\" + area.properties["path"];
                        var areaPath = AREAL_PATH + "\\" + area.properties["id"];
                        var areaJsonContent = File.ReadAllText(areaJsonPath);
                        var areas = GeoJSON.GeoJSONObject.Deserialize(areaJsonContent);
                        foreach (var areaBuildings in areas.features)
                        {
                            var coordinates = areaBuildings.properties["origin"].Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
                            var longitude = float.Parse(coordinates[0]);
                            var latitude = float.Parse(coordinates[1]);

                            lastObject = new OBJLoader().Load(areaPath + "\\" + areaBuildings.properties["models"].Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries)[0]);
                            
                            var position = GPSEncoder.GPSToUCS(latitude, longitude);
                            lastObject.transform.position = position;
                            lastObject.transform.Rotate(0, 180, 0);

                            lastObject.GetComponentInChildren<Renderer>().material.color = Random.ColorHSV(0.2f, 0.4f, 0.4f, 0.5f, 1f, 1f);

                            lastObject.AddComponent<MeshFilter>();

                            var mesh = lastObject.GetComponent<MeshFilter>().mesh;
                            if (mesh != null)
                            {
                                MeshCollider meshCollider = lastObject.AddComponent<MeshCollider>();
                                meshCollider.sharedMesh = mesh;
                            }
                        }
                    }
                }
            }
            lastObject.SetActive(true);            
        }
    }
}
