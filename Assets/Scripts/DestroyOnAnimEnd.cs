using UnityEngine;

public class DestroyOnAnimEnd : MonoBehaviour
{

    public void DestroyAnim() {
        Destroy(transform.parent.gameObject); 
    }

}
