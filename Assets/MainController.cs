using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public PlayerBox[] pbs;
    public int[] testids;
    public InputField[] idinput;

    public InputField qualifiers;
    public InputField roundtext;
    public InputField roundnum;
    public InputField commentator;
    public InputField overlay;

    public TMP_Text text;

    public Button showChanger;
    public Button saveButton;

    public CanvasGroup changer;

    public GameObject death_popup;
    public GameObject win_popup;
    public GameObject manicanvas;

    public void die(int boxid, bool silent = false)
    {
        if (silent == false)
        {
            // there is no reason for this bool
            DeathPopupManager e = GameObject.Instantiate(death_popup, manicanvas.transform).GetComponent<DeathPopupManager>();
            e.boxid = boxid;
            e.loadinfo();
        }
    }

    public void win(int boxid, bool silent = false)
    {
        if (silent == false)
        {
            // similarly no reason
            DeathPopupManager e = GameObject.Instantiate(win_popup, manicanvas.transform).GetComponent<DeathPopupManager>();
            e.win = true;
            e.boxid = boxid;
            e.loadinfo();
        }
    }

    public void Update()
    {
        string maintext = "";
        maintext += "<font-weight=100>" + qualifiers.text + "</font-weight>";
        maintext += " <font-weight=500>" + roundtext.text + "</font-weight>";
        maintext += " <font-weight=700>" + roundnum.text + "</font-weight>\n";
        maintext += commentator.text + "\n";
        maintext += overlay.text;
        text.text = maintext;
        // set the texts based on the inputs on the main screen
    }

    public void Start()
    {
        Camera.main.backgroundColor = new Color(0, 0, 0, 0); // did this to try window transparency, no luck

        Global.cache = new List<Global.osekaiCache>();  // set cache
        Global.pfp_cache = new List<Global.pfpCache>(); // set pfp cache
        Global.players = new List<Global.Player>();     // set players
        int id = 0;
        int osuid = 2;
        foreach(PlayerBox x in pbs)
        {
            // sets the test ids, helps with, you know, testing
            idinput[id].text = testids[id].ToString();
            Global.players.Add(new Global.Player(testids[id], Texture2D.whiteTexture, "", false, x));
            x.id = id;
            id += 1;
            //x.update();
        }
        Global.reloadPlayerBoxes();

        saveButton.onClick.AddListener(UpdateUserBoxes);
        showChanger.onClick.AddListener(showIDChanger);
    }

    public void showIDChanger()
    {
        // theres probably an easier way to do this
        if(changer.alpha == 0)
        {
            changer.alpha = 1;
            changer.interactable = true;
            changer.blocksRaycasts = true;
        }
        else
        {
            changer.alpha = 0;
            changer.interactable = false;
            changer.blocksRaycasts = false;
        }
    }

    public void UpdateUserBoxes()
    {
        int eid = 0;
        foreach(InputField e in idinput)
        {
            int id = int.Parse(e.text);
            Global.players[eid].osuid = id;
            eid += 1;
        }
        Global.reloadPlayerBoxes();
    }
}
