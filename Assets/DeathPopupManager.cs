using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathPopupManager : MonoBehaviour
{
    public Animation e;

    public int boxid;

    bool began;

    public Image pfp;
    public Text name;

    public bool win = false;
    public float curtime = 0;

    // assign through inspector plz
    public string[] death = new string[]
    {
        "couldn't handle the pressure",
        "couldn't handle the pressure 2"
    };

    public Text deathtext;

    // Start is called before the first frame update
    void Update()
    {
        curtime += Time.deltaTime;

        if(e.isPlaying == false && began == true && win == false)
        {
            Destroy(this.gameObject);
        }

        if(win == true){
            if(curtime == 2){
                this.GetComponent<Animation>().enabled = false;
            }
        }
    }

    public void loadinfo()
    {
        name.text = Global.players[boxid].username; // gets user's username

        pfp.sprite = Sprite.Create(Global.players[boxid].pfp, 
            new Rect(0, 0, Global.players[boxid].pfp.width, 
            Global.players[boxid].pfp.height), 
            new Vector2(0.5f, 0.5f));               // assigns the sprite of the pfp to a new sprite craeted from the player's pfp

        began = true;                               // begin
        e.Play();                                   // play the animation

        if(win == true)
        {
            // on the occasion that they win, we want to jump before the death message and replace it
            MainController mc = Global.players[boxid].pb.mc;
            string maintext = "has won this match!";
            deathtext.text = maintext;
            return; // then return
        }
        int index = Random.Range(0, death.Length);
        deathtext.text = death[index];
    }
}

