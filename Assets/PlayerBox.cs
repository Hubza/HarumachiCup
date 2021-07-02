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
        osuid = Global.players[id].osuid;               // gets the user's osu! id

        Debug.Log("UPDATING OBJECT " + id.ToString() + " WITH OBJECT NAME " + Global.players[id].pb.name + " AND OSU ID " + osuid);
        bool cached = false;
        bool pfp_cached = false;

        Texture2D cached_pfp = Texture2D.whiteTexture;  // sets the pfp to white. in the occasion that we cant load the pfp, we dont want to fail horribly

        string response = "";

        pfp.sprite = Sprite.Create(Texture2D.whiteTexture, 
            new Rect(0, 0, Texture2D.whiteTexture.width, 
            Texture2D.whiteTexture.height), 
            new Vector2(0.5f, 0.5f));                   // set the sprite itself to white, to mitigate errors n stuff


        name.text = "Loading...";

        foreach (Global.pfpCache cache in Global.pfp_cache)
        {
            // check the pfp cache to see if the user's pfp has already been cached
            // this isnt performant in any way but i dont think this matters in this case

            if (cache.osuid == osuid)
            {
                cached = true;
                cached_pfp = cache.texture;
                Debug.Log("loading from cache...");
            }
        }


        foreach (Global.osekaiCache cache in Global.cache)
        {
            // check cache for the response itself

            if (cache.osuid == osuid)
            {
                pfp_cached = true;
                response = cache.response;
                Debug.Log("loading from cache...");
            }
        }

        if (cached == false)
        {
            // if it's not cached we're gonna send a request to osekai's servers to get user data
            //
            // osekai is an alternative rankings system for osu! - we use their data.

            using (UnityWebRequest www = UnityWebRequest.Get("https://osekai.net/eclipse/api/profiles/get_user.php?bypass_local=true&id=" + osuid))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    Debug.Log("uh oh the network code did a fucky wucky");
                    // if this happens there are bigger issues
                }
                else
                {
                    response = www.downloadHandler.text;
                    Global.cache.Add(new Global.osekaiCache(osuid, response));
                    // cache it
                }
            }
        }

        Debug.Log(response);                // log the response
        JObject o = JObject.Parse(response);// parse it into a jobject
        Debug.Log(o["username"]);           // print the username, just to check if it worked

        Global.players[id].username = (string)o["username"];
        name.text = (string)o["username"];


        if (pfp_cached == false)
        {
            // basically the same as user data get but with pfp
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
        // update sprite

        yield return true;
    }

    public void update()
    {
        StartCoroutine(GetRequest());
        // guess
    }

    public void Start()
    {
        itsme = this.GetComponent<Button>();
        itsme.onClick.AddListener(click);
        // we're gonna get the button and then listen for clicks
        // this allows us to die when clicked
    }

    public bool dead; // rip
    public Color32 red = new Color32(255, 5, 95, 190);


    public void click()
    {
        if(dead == true)
        {
            // gonna unkill the user, then return. this stops the rest from running, as we don't want it to
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
                alive -= 1; // this removes one from the alive count.
            }
        }
        
        if(alive == 2)
        {
            // if the alive count is true then this is the final click.
            // whoever's clicked dies, and the one who isn't wins.
            final = true;
        }

        if (final == false)
        {
            if (dead == false)
            {
                // if we're not dead and its not final, we'll die
                GetComponent<Image>().color = red;      // set to red
                name.color = red;                       // set to red
                pfp.color = new Color(1, 1, 1, 0.5f);   // set to opaque red
                mc.die(id);                             // tell the maincontroller to start the animation
                dead = true;                            // die
            }
        }
        else
        {
            if (dead == false)
            {
                foreach (Global.Player pb in Global.players)
                {
                    // gonna go through each one
                    if (pb.pb.dead == false)
                    {
                        // if they're not dead
                        if (pb.pb.id == id)
                        {
                            // and they're us
                            // ... we're gonna die.

                            GetComponent<Image>().color = red;
                            name.color = red;
                            pfp.color = new Color(1, 1, 1, 0.5f);
                            dead = true;
                        }
                        else
                        {
                            // else there's only one remaining due to the requirement of final
                            // and he's the winner.
                            // so we let them win.

                            winner = pb.pb.id;
                            pb.pb.GetComponent<Image>().color = Color.yellow;
                            pb.pb.pfp.color = Color.yellow;
                            pb.pb.dead = true;
                        }
                    }
                }
                mc.win(winner); // animate win
            }
        }
    }
}
