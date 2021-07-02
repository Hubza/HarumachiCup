using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class PlayerBox : MonoBehaviour
{
    public int id;

    public Image pfp;
    public Text name;

    public Button itsme;

    public MainController mc;

    int osuid;

    public IEnumerator GetRequest()
    {
        osuid = Global.players[id].osuid;
        Debug.Log("UPDATING OBJECT " + id.ToString() + " WITH OBJECT NAME " + Global.players[id].pb.name + " AND OSU ID " + osuid);
        bool cached = false;
        bool pfp_cached = false;
        Texture2D cached_pfp = Texture2D.whiteTexture;
        string response = "";

        pfp.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f));
        name.text = "Loading...";

        foreach (Global.pfpCache cache in Global.pfp_cache)
        {
            if (cache.osuid == osuid)
            {
                cached = true;
                cached_pfp = cache.texture;
                Debug.Log("loading from cache...");
            }
        }


        foreach (Global.osekaiCache cache in Global.cache)
        {
            //Debug.Log("from cache: " + cache.response);
            if (cache.osuid == osuid)
            {
                pfp_cached = true;
                response = cache.response;
                Debug.Log("loading from cache...");
            }
        }

        if (cached == false)
        {
            using (UnityWebRequest www = UnityWebRequest.Get("https://osekai.net/eclipse/api/profiles/get_user.php?bypass_local=true&id=" + osuid))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    Debug.Log("uh oh the network code did a fucky wucky");
                }
                else
                {
                    response = www.downloadHandler.text;
                    Global.cache.Add(new Global.osekaiCache(osuid, response));
                }
            }
        }

        Debug.Log(response);
        JObject o = JObject.Parse(response);
        Debug.Log(o["username"]);

        Global.players[id].username = (string)o["username"];
        name.text = (string)o["username"];


        if (pfp_cached == false)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://a.ppy.sh/" + osuid);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Global.players[id].pfp = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Global.pfp_cache.Add(new Global.pfpCache(osuid, Global.players[id].pfp));
            }
        }
        else
        {
            Global.players[id].pfp = cached_pfp;
        }



        pfp.sprite = Sprite.Create(Global.players[id].pfp, new Rect(0, 0, Global.players[id].pfp.width, Global.players[id].pfp.height), new Vector2(0.5f, 0.5f));

        yield return true;
    }

    public void update()
    {
        StartCoroutine(GetRequest());
    }

    public void Start()
    {
        itsme = this.GetComponent<Button>();
        itsme.onClick.AddListener(click);
    }

    public bool dead;
    public Color32 red = new Color32(255, 5, 95, 190);


    public void click()
    {
        if(dead == true)
        {
            GetComponent<Image>().color = Color.white;
            name.color = Color.white;
            pfp.color = Color.white;
            dead = false;
            return;
        }

        bool final = false;
        int alive = Global.players.Count;

        int thisone = 0;
        int winner = 0;

        foreach (Global.Player pb in Global.players)
        {
            if(pb.pb.dead == true)
            {
                alive -= 1;
            }
        }
        
        if(alive == 2)
        {
            final = true;
        }

        if (final == false)
        {
            if (dead == false)
            {
                GetComponent<Image>().color = red;
                name.color = red;
                pfp.color = new Color(1, 1, 1, 0.5f);
                mc.die(id);
                dead = true;
            }
        }
        else
        {
            if (dead == false)
            {
                foreach (Global.Player pb in Global.players)
                {
                    if (pb.pb.dead == false)
                    {
                        if (pb.pb.id == id)
                        {
                            GetComponent<Image>().color = red;
                            name.color = red;
                            pfp.color = new Color(1, 1, 1, 0.5f);
                            dead = true;
                        }
                        else
                        {
                            winner = pb.pb.id;
                            pb.pb.GetComponent<Image>().color = Color.yellow;
                            pb.pb.pfp.color = Color.yellow;
                            pb.pb.dead = true;
                        }
                    }
                }
                mc.win(winner);
            }
        }
    }
}
