using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public class Player
    {
        public int osuid { get; set; }
        public Texture2D pfp { get; set; }
        public string username { get; set; }
        public bool isout { get; set; }
        public PlayerBox pb { get; set; }
        public Player(int o_osuid, Texture2D o_pfp, string o_username, bool o_isout, PlayerBox o_pb)
        {
            osuid = o_osuid;
            pfp = o_pfp;
            username = o_username;
            isout = o_isout;
            pb = o_pb;
        }
    }

    public class osekaiCache
    {
        public int osuid { get; set; }
        public string response { get; set; }
        public osekaiCache(int o_osuid, string o_response)
        {
            osuid = o_osuid;
            response = o_response;
        }
    }

    public class pfpCache
    {
        public int osuid { get; set; }
        public Texture2D texture { get; set; }

        public pfpCache(int o_osuid, Texture2D o_texture)
        {
            osuid = o_osuid;
            texture = o_texture;
        }
    }

    public static List<pfpCache> pfp_cache;

    public static List<osekaiCache> cache;
    public static List<Player> players;
    
    public static void reloadPlayerBoxes()
    {
        foreach (Player x in players)
        {

            x.pb.update();
            
        }
    }
}
