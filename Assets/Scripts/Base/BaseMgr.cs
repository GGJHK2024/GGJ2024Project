using UnityEngine;

public class BaseMgr<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        if( instance == null )
        {
            GameObject obj = new GameObject();
            //设置对象的名字为脚本名
            obj.name = typeof(T).ToString();
            instance = obj.AddComponent<T>();
        }
        return instance;
    }

    public virtual void Awake()
    {
        if(instance==null) instance=this as T;
        else Destroy(gameObject);
    }

}