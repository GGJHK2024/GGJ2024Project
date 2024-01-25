using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr {
    private static GameMgr instance;
 
    private GameMgr() {//实例化

    }    
 
    public static GameMgr Instance {
        get {
            if(instance==null) {
                instance = new GameMgr();
            }
 
            return instance;
        }
    }
 
    //各项功能
    public void Death(bool death) {

    }

}
