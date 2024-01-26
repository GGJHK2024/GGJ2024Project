using UnityEngine;

public class BaseMgr<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        if( instance == null )
        {
            GameObject father = GameObject.FindWithTag("manager");
            
            if (father == null)
            {
                father = new GameObject();
                father.name = "Managers";
            }
            father.transform.SetParent(father.transform);
            //设置对象的名字为脚本名
            instance = father.AddComponent<T>();
        }
        return instance;
    }

    public virtual void Awake()
    {
        if(instance==null) instance=this as T;
        else Destroy(gameObject);
    }

}