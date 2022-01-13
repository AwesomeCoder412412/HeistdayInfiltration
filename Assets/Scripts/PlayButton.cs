using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using System;
using System.Net.Sockets;
using System.Net;
using SimpleFirebaseUnity;
using UnityEngine.Events;
using Leguar.TotalJSON;
//using Newtonsoft.Json;

public class PlayButton : MonoBehaviour
{
    public InputField minRooms;
    public InputField maxRooms;
    public InputField guard;
    public InputField teammate;
    public InputField players;
    public string minRoom = "minRoom";
    public string maxRoom = "maxRoom";
    public string guards = "guards";
    public string teammates = "teammates";
    public string players1 = "players";
    public static PlayButton instance;
    private string retry1 = "retry1";
    public List<string> ipList = new List<string>();
    public GameObject buttonPrefab;
    public GameObject ScrollThingy;
    public Firebase firebase;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GetLocalIPAddress() + " yes please");
        firebase = Firebase.CreateNew("https://heistday-9d49b-default-rtdb.firebaseio.com/");
        Debug.Log(RespawnPain.instance.docID);
        if (RespawnPain.instance.docID == "")
        {
            return;
        }
        Debug.Log("got past nullcheck");
        firebase.Child("mailbox").Child(RespawnPain.instance.docID).Delete();

    }

    public static string GetLocalIPAddress()
    {
        //var host = Dns.GetHostEntry(Dns.GetHostName());
        //foreach (var ip in host.AddressList)
        //{
        //    if (ip.AddressFamily == AddressFamily.InterNetwork)
        //    {
        //        return ip.ToString();
        //    }
        //}
        //throw new Exception("No network adapters with an IPv4 address in the system!");
        return "123";
    }

    public void Play()
    {
        PlayerPrefs.SetInt(minRoom, int.Parse(minRooms.text));
        PlayerPrefs.SetInt(maxRoom, int.Parse(maxRooms.text));
        PlayerPrefs.SetInt(guards, int.Parse(guard.text));
        PlayerPrefs.SetInt(teammates, int.Parse(teammate.text));
        PlayerPrefs.SetInt(players1, int.Parse(players.text));
        NetworkManager.singleton.maxConnections = int.Parse(players.text);
        NetworkManager.singleton.StopHost();
       string myIp = GetLocalIPAddress();
        //Firebase firebase = Firebase.CreateNew("https://heistday-9d49b-default-rtdb.firebaseio.com/");
        firebase.OnPushSuccess += PushHandler;
        firebase.OnPushFailed += PushFailedHandler;
        //firebase.Child("mailbox").Push(myIp);
        StartCoroutine(PushHandlerWait(myIp));
        //firebase.OnGetSuccess += GetHandler;
        //firebase.OnGetFailed += GetFailedHandler;
        //firebase.GetValue();
        NetworkManager.singleton.StartHost();
    }

    public void GetServers()
    {
        firebase.OnGetSuccess += GetHandler;
        firebase.OnGetFailed += GetFailedHandler;
        firebase.GetValue();
    }

    public void ScrollThing()
    {
        int yOffset = 0;

        GameObject[] gos = GameObject.FindGameObjectsWithTag("ServerButton");
        foreach (GameObject go in gos)
            Destroy(go); 
        foreach (string ip in ipList)
        {
            GameObject listing = Instantiate(buttonPrefab, ScrollThingy.transform);
            listing.GetComponentInChildren<Text>().text = ip;
            listing.transform.position += Vector3.up * yOffset;
            yOffset -= 100;
        }
    }

    public IEnumerator PushHandlerWait(string myIp)
    {
        yield return null;
        firebase.Child("mailbox", true).Push(myIp, false);
    }

    public void PushHandler(Firebase sender, DataSnapshot snapshot)
    {
        Debug.Log("push success");
        Debug.Log(snapshot.Keys[0]);
        Debug.Log(snapshot.RawJson);
        Debug.Log(snapshot.Keys);
        RespawnPain.instance.docID = ((Dictionary<string, System.Object>)snapshot.RawValue)[snapshot.Keys[0]].ToString();
    }
    public void PushFailedHandler(Firebase sender, FirebaseError error)
    {
        Debug.Log("push failed");
        Debug.Log(error.Message);
        
    }

    public void GetHandler(Firebase sender, DataSnapshot snapshot)
    {
        Debug.Log("success!");
        ipList = new List<string>();
        foreach (string key in snapshot.Keys)
        {
            Debug.Log(snapshot.RawJson);
            Debug.Log(snapshot.RawValue);
            Dictionary<string, System.Object> dict = (Dictionary<string, System.Object>)snapshot.RawValue;
            Dictionary<string, System.Object> dict1 = (Dictionary<string, System.Object>)dict[key];
            foreach (string key1 in dict1.Keys)
            {
                Debug.Log(dict1[key1]);
                ipList.Add(dict1[key1].ToString());
            }
            Debug.Log(dict[key].ToString());
            /*JSON json = JsonConvert.DeserializeObject<JSON>(snapshot.RawJson);
            Debug.Log("json " + json.CreateString());*/
        }
        ScrollThing();
        //Debug.Log(snapshot.RawValue);
    }

    public void GetFailedHandler(Firebase sender, FirebaseError error )
    {

        Debug.Log("failed");
        Debug.Log(error.Message);
    }

    public void PlayExpress()
    {
        Debug.Log("Lemon");
        NetworkManager.singleton.StopHost();
        NetworkManager.singleton.StartHost();
    }

    public void Quit()
    {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        //if (FirebaseVariables.instance.refresh)
        //{
        //    GetServers();
        //    FirebaseVariables.instance.refresh = false;
        //}
        /*if (MirrorVariables.instance.retry)
        {
            NetworkManager.singleton.StartClient();
            MirrorVariables.instance.retry = false;
        }*/
        if (PlayerPrefs.GetInt(retry1) == 1)
        {
            Debug.Log("Lemon");
            PlayerPrefs.SetInt(retry1, 0);
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.StartHost();
        }
    }
}
