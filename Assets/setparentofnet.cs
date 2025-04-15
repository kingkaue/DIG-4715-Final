using JetBrains.Annotations;
using UnityEngine;

public class setparentofnet : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private GameObject netposition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        netposition = GameObject.Find("PlayerCorrect/riggedplayermodel/root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/netholdpoint");
        gameObject.transform.SetParent(netposition.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
