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
        name.text = Global.players[boxid].username;
        pfp.sprite = Sprite.Create(Global.players[boxid].pfp, new Rect(0, 0, Global.players[boxid].pfp.width, Global.players[boxid].pfp.height), new Vector2(0.5f, 0.5f));
        began = true;
        e.Play();
        if(win == true)
        {
            MainController mc = Global.players[boxid].pb.mc;
            string maintext = "has won this match!";
            //maintext += "<font-weight=100>" + mc.qualifiers.text + "</font-weight>";
            //maintext += " <font-weight=500>" + mc.roundtext.text + "</font-weight>";
            //maintext += " <font-weight=700>" + mc.roundnum.text + "</font-weight>\n";
            deathtext.text = maintext;
            return;
        }
        int index = Random.Range(0, death.Length);
        deathtext.text = death[index];
    }
}
